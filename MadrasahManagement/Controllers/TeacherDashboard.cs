using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadrasahManagement.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherDashboardController : Controller
    {
        private readonly MadrasahDbContext _db;

        public TeacherDashboardController(MadrasahDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Dashboard - Main teacher view
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _db.Teachers
                .Include(t => t.Department)
                .Include(t => t.ClassSubjects)
                    .ThenInclude(cs => cs.Class)
                .Include(t => t.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(t => t.Timetables)
                    .ThenInclude(tt => tt.Class)
                .Include(t => t.Timetables)
                    .ThenInclude(tt => tt.Subject)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("Teacher profile not found.");
            }

            // Get today's schedule - using DayName property
            var todaySchedule = GetTodaysSchedule(teacher.Timetables?.ToList());

            ViewBag.TodaySchedule = todaySchedule;
            ViewBag.Today = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

            return View(teacher);
        }

        private List<Timetable> GetTodaysSchedule(List<Timetable> timetables)
        {
            if (timetables == null || !timetables.Any())
                return new List<Timetable>();

            var today = DateTime.Now.DayOfWeek;
            var todayString = GetDayString(today);

            return timetables
                .Where(t => !string.IsNullOrEmpty(t.DayName) && CompareDays(t.DayName, todayString))
                .OrderBy(t => t.StartTime)
                .ToList();
        }

        // Profile view
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _db.Teachers
                .Include(t => t.Department)
                .Include(t => t.ClassSubjects)
                    .ThenInclude(cs => cs.Class)
                .Include(t => t.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(t => t.Timetables)
                    .ThenInclude(tt => tt.Class)
                .Include(t => t.Timetables)
                    .ThenInclude(tt => tt.Subject)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // Edit profile - GET
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _db.Teachers
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound();
            }

            var editModel = new TeacherProfileEditViewModel
            {
                Id = teacher.TeacherId,
                Name = teacher.Name,
                Email = teacher.Email,
                Contact = teacher.Contact,
                Qualification = teacher.Qualification,
                Designation = teacher.Designation,
                JoiningDate = teacher.JoiningDate,
                ImageUrl = teacher.ImageUrl
            };

            return View(editModel);
        }

        // Edit profile - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(TeacherProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _db.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId && t.TeacherId == model.Id);

            if (teacher == null)
            {
                return NotFound();
            }

            teacher.Name = model.Name;
            teacher.Email = model.Email;
            teacher.Contact = model.Contact;
            teacher.Qualification = model.Qualification;
            teacher.Designation = model.Designation;
            teacher.JoiningDate = model.JoiningDate;

            // Handle image upload
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "teachers");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(teacher.ImageUrl) && teacher.ImageUrl.StartsWith("/uploads/teachers/"))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", teacher.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                teacher.ImageUrl = $"/uploads/teachers/{fileName}";
            }

            try
            {
                _db.Teachers.Update(teacher);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(model);
            }
        }

        // Classes and Subjects
        public async Task<IActionResult> MyClasses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _db.Teachers
                .Include(t => t.ClassSubjects)
                    .ThenInclude(cs => cs.Class)
                .Include(t => t.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // Timetable
        public async Task<IActionResult> Timetable()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _db.Teachers
                .Include(t => t.Timetables)
                    .ThenInclude(tt => tt.Class)
                .Include(t => t.Timetables)
                    .ThenInclude(tt => tt.Subject)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound();
            }

            // Group and order timetable by day - using DayName property
            var timetableGroups = teacher.Timetables
                .Where(t => !string.IsNullOrEmpty(t.DayName))
                .GroupBy(t => t.DayName)
                .Select(g => new TimetableGroup
                {
                    Day = g.Key,
                    Periods = g.OrderBy(p => p.StartTime).ToList(),
                    DayOrder = GetDayOrder(g.Key)
                })
                .OrderBy(g => g.DayOrder)
                .ToList();

            ViewBag.TimetableGroups = timetableGroups;

            return View(teacher);
        }

        // Helper class for timetable grouping
        public class TimetableGroup
        {
            public string Day { get; set; }
            public List<Timetable> Periods { get; set; }
            public int DayOrder { get; set; }
        }

        // Helper methods
        public static string GetDayString(System.DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                System.DayOfWeek.Monday => "Monday",
                System.DayOfWeek.Tuesday => "Tuesday",
                System.DayOfWeek.Wednesday => "Wednesday",
                System.DayOfWeek.Thursday => "Thursday",
                System.DayOfWeek.Friday => "Friday",
                System.DayOfWeek.Saturday => "Saturday",
                System.DayOfWeek.Sunday => "Sunday",
                _ => ""
            };
        }

        public static bool CompareDays(string day1, string day2)
        {
            if (string.IsNullOrEmpty(day1) || string.IsNullOrEmpty(day2))
                return false;

            // Normalize strings
            day1 = day1.Trim().ToLower();
            day2 = day2.Trim().ToLower();

            // Handle abbreviations
            var dayMap = new Dictionary<string, string>
            {
                { "mon", "monday" },
                { "tue", "tuesday" },
                { "wed", "wednesday" },
                { "thu", "thursday" },
                { "fri", "friday" },
                { "sat", "saturday" },
                { "sun", "sunday" },
                { "m", "monday" },
                { "t", "tuesday" },
                { "w", "wednesday" },
                { "th", "thursday" },
                { "f", "friday" },
                { "s", "saturday" },
                { "su", "sunday" }
            };

            // Expand abbreviations
            if (dayMap.ContainsKey(day1)) day1 = dayMap[day1];
            if (dayMap.ContainsKey(day2)) day2 = dayMap[day2];

            return day1 == day2;
        }

        public static int GetDayOrder(string day)
        {
            if (string.IsNullOrEmpty(day))
                return 99;

            day = day.Trim().ToLower();

            return day switch
            {
                "monday" or "mon" or "m" => 1,
                "tuesday" or "tue" or "t" => 2,
                "wednesday" or "wed" or "w" => 3,
                "thursday" or "thu" or "th" => 4,
                "friday" or "fri" or "f" => 5,
                "saturday" or "sat" or "s" => 6,
                "sunday" or "sun" or "su" => 7,
                _ => 8
            };
        }
    }
}