
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MadrasahManagement.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly MadrasahDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StudentDashboardController(
            MadrasahDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Main Dashboard
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var student = await _context.Students
                .Include(s => s.AppUser)
                .Include(s => s.Class)
                .Include(s => s.Section)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student profile not found.";
                return RedirectToAction("Profile");
            }

            // Get today's attendance
            var todayAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.StudentId == student.StudentId &&
                                        a.Date.Date == DateTime.Today);

            // Get pending assignments
            //var pendingAssignments = await _context.Assignments
            //    .Where(a => a.ClassId == student.ClassId &&
            //              a.DueDate > DateTime.Now &&
            //              !a.Submissions.Any(s => s.StudentId == student.StudentId))
            //    .CountAsync();

            // Get recent notices
            var recentNotices = await _context.Notices
                .Where(n => n.DatePosted >= DateTime.Now.AddDays(-30))
                .OrderByDescending(n => n.DatePosted)
                .Take(5)
                .ToListAsync();

            var dashboardData = new DashboardViewModel
            {
                Student = student,
                TodayAttendance = todayAttendance?.Status.ToString() ?? "Not Marked",
               // PendingAssignments = pendingAssignments,
                RecentNotices = recentNotices
            };

            return View(dashboardData);
        }

        // Student Profile
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _context.Students
                .Include(s => s.AppUser)
                .Include(s => s.Class)
                .Include(s => s.Section)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student profile not found.";
                return View(new Student());
            }

            return View(student);
        }

        // Attendance Records
        public async Task<IActionResult> Attendance()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound();
            }

            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = DateTime.Now;

            var attendance = await _context.Attendances
                .Where(a => a.StudentId == student.StudentId &&
                          a.Date >= startDate &&
                          a.Date <= endDate)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            var summary = new AttendanceSummaryViewModel
            {
                StudentName = student.StudentName,
                RegNo = student.RegNo,
                AttendanceRecords = attendance,
                PresentDays = attendance.Count(a => a.Status == AttendanceStatus.Present),
                AbsentDays = attendance.Count(a => a.Status == AttendanceStatus.Absent),
                TotalDays = attendance.Count
            };

            return View(summary);
        }

        // Fee Details
        public async Task<IActionResult> Fees()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound();
            }

            var feeCollections = await _context.FeeCollections
                .Include(f => f.FeeType)
                .Where(f => f.StudentId == student.StudentId)
                .OrderByDescending(f => f.DatePaid)
                .ToListAsync();

            var totalPaid = feeCollections.Sum(f => f.AmountPaid);

            // Assuming class-based fee structure
            var classFeeTypes = await _context.FeeTypes
                .Where(f => f.ClassId == student.ClassId)
                .ToListAsync();

            var totalDue = classFeeTypes.Sum(f => f.Amount);

            var feeSummary = new FeeSummaryViewModel
            {
                StudentName = student.StudentName,
                FeeCollections = feeCollections,
                TotalPaid = totalPaid,
                TotalDue = totalDue,
                Balance = totalDue - totalPaid
            };

            return View(feeSummary);
        }

        // Assignments
        public async Task<IActionResult> Assignments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound();
            }

            var assignments = await _context.Assignments
                .Include(a => a.Subject)
                .Include(a => a.Teacher)
                .Where(a => a.ClassId == student.ClassId)
                .OrderByDescending(a => a.DueDate)
                .ToListAsync();

            var submittedIds = await _context.Submissions
                .Where(s => s.StudentId == student.StudentId)
                .Select(s => s.AssignmentId)
                .ToListAsync();

            var viewModel = new AssignmentViewModel
            {
                PendingAssignments = assignments.Where(a => !submittedIds.Contains(a.AssignmentId)).ToList(),
                SubmittedAssignments = assignments.Where(a => submittedIds.Contains(a.AssignmentId)).ToList()
            };

            return View(viewModel);
        }

        // Submit Assignment
        [HttpGet]
        public async Task<IActionResult> SubmitAssignment(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.AssignmentId == id);

            if (assignment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            // Check if already submitted
            var existing = await _context.Submissions
                .FirstOrDefaultAsync(s => s.AssignmentId == id && s.StudentId == student.StudentId);

            if (existing != null)
            {
                TempData["InfoMessage"] = "Already submitted!";
                return RedirectToAction("Assignments");
            }

            var submission = new Submission
            {
                AssignmentId = id,
                StudentId = student.StudentId
            };

            ViewBag.Assignment = assignment;
            return View(submission);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAssignment(Submission submission)
        {
            if (ModelState.IsValid)
            {
                submission.SubmittedAt = DateTime.Now;
                _context.Submissions.Add(submission);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Assignment submitted successfully!";
                return RedirectToAction("Assignments");
            }

            return View(submission);
        }

        // Change Password
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(
                    user,
                    model.CurrentPassword,
                    model.NewPassword);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Password changed successfully!";
                    return RedirectToAction("Profile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }

    // View Models
    public class DashboardViewModel
    {
        public Student Student { get; set; }
        public string TodayAttendance { get; set; }
        public int PendingAssignments { get; set; }
        public List<Notice> RecentNotices { get; set; }
    }

    public class AttendanceSummaryViewModel
    {
        public string StudentName { get; set; }
        public string RegNo { get; set; }
        public List<Attendance> AttendanceRecords { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int TotalDays { get; set; }
    }

    public class FeeSummaryViewModel
    {
        public string StudentName { get; set; }
        public List<FeeCollection> FeeCollections { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public decimal Balance { get; set; }
    }

    public class AssignmentViewModel
    {
        public List<Assignment> PendingAssignments { get; set; }
        public List<Assignment> SubmittedAssignments { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}