using MadrasahManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadrasahManagement.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class AttendanceController : Controller
    {
        private readonly MadrasahDbContext _context;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(MadrasahDbContext context, ILogger<AttendanceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Main attendance page
        public async Task<IActionResult> Index(int? departmentId, int? classId, int? sectionId, DateTime? date)
        {
            // Default to today
            var selectedDate = date ?? DateTime.Today;

            // Get current teacher
            var teacherId = await GetCurrentTeacherIdAsync();

            // Get data for dropdowns
            var model = new AttendanceViewModel
            {
                SelectedDate = selectedDate,
                DepartmentId = departmentId,
                ClassId = classId,
                SectionId = sectionId
            };

            // Get departments
            model.Departments = await GetTeacherDepartmentsAsync(teacherId);

            // If department selected, get classes
            if (departmentId.HasValue)
            {
                model.Classes = await GetClassesByDepartmentAsync(teacherId, departmentId.Value);

                // If class selected, get sections
                if (classId.HasValue)
                {
                    model.Sections = await GetSectionsByClassAsync(departmentId.Value, classId.Value);

                    // If section selected, get attendance data
                    if (sectionId.HasValue)
                    {
                        model.Students = await GetClassAttendanceAsync(
                            departmentId.Value,
                            classId.Value,
                            sectionId.Value,
                            selectedDate);

                        model.Summary = await GetAttendanceSummaryAsync(
                            departmentId.Value,
                            classId.Value,
                            sectionId.Value,
                            selectedDate);

                        // Get names for display
                        var className = await _context.Classes
                            .Where(c => c.ClassId == classId.Value)
                            .Select(c => c.ClassName)
                            .FirstOrDefaultAsync();
                        model.ClassName = className ?? "N/A";

                        var departmentName = await _context.Departments
                            .Where(d => d.DepartmentId == departmentId.Value)
                            .Select(d => d.DepartmentName)
                            .FirstOrDefaultAsync();
                        model.DepartmentName = departmentName ?? "N/A";

                        var sectionName = await _context.Sections
                            .Where(s => s.SectionId == sectionId.Value)
                            .Select(s => s.SectionName)
                            .FirstOrDefaultAsync();
                        model.SectionName = sectionName ?? "N/A";
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAttendance(int departmentId, int classId, int sectionId, DateTime date)
        {
            try
            {
                var teacherId = await GetCurrentTeacherIdAsync();

                // DEBUG: Log the teacher ID
                _logger.LogInformation($"Current teacher ID: {teacherId}");

                // Check if teacher exists in database
                var teacherExists = await _context.Teachers.AnyAsync(t => t.TeacherId == teacherId);
                if (!teacherExists)
                {
                    _logger.LogError($"Teacher with ID {teacherId} does not exist in database!");
                    TempData["ErrorMessage"] = $"Error: Teacher with ID {teacherId} not found in database. Please contact administrator.";
                    return RedirectToAction("Index", new { departmentId, classId, sectionId, date });
                }

                var form = await Request.ReadFormAsync();
                var todayDate = date.Date;

                // Process each student
                foreach (var key in form.Keys)
                {
                    if (key.StartsWith("attendance_"))
                    {
                        var studentIdStr = key.Replace("attendance_", "");
                        if (int.TryParse(studentIdStr, out int studentId))
                        {
                            if (int.TryParse(form[key], out int statusValue))
                            {
                                var status = (AttendanceStatus)statusValue;

                                // Check if attendance already exists
                                var existing = await _context.Attendances
                                    .FirstOrDefaultAsync(a => a.StudentId == studentId
                                        && a.Date.Date == todayDate
                                        && a.DepartmentId == departmentId
                                        && a.ClassId == classId
                                        && a.SectionId == sectionId);

                                if (existing != null)
                                {
                                    // Update existing
                                    existing.Status = status;
                                    existing.TeacherId = teacherId;
                                    _context.Attendances.Update(existing);
                                }
                                else
                                {
                                    // Create new
                                    var attendance = new Attendance
                                    {
                                        StudentId = studentId,
                                        ClassId = classId,
                                        DepartmentId = departmentId,
                                        SectionId = sectionId,
                                        Date = date,
                                        Status = status,
                                        TeacherId = teacherId
                                    };
                                    await _context.Attendances.AddAsync(attendance);
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Attendance saved successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving attendance");

                // Check for specific foreign key errors
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FOREIGN KEY"))
                {
                    if (ex.InnerException.Message.Contains("Teachers"))
                    {
                        TempData["ErrorMessage"] = "Database error: Teacher not found. Please make sure you are logged in as a valid teacher.";
                    }
                    else if (ex.InnerException.Message.Contains("Students"))
                    {
                        TempData["ErrorMessage"] = "Database error: One or more students not found.";
                    }
                    else if (ex.InnerException.Message.Contains("Classes"))
                    {
                        TempData["ErrorMessage"] = "Database error: Class not found.";
                    }
                    else if (ex.InnerException.Message.Contains("Departments"))
                    {
                        TempData["ErrorMessage"] = "Database error: Department not found.";
                    }
                    else if (ex.InnerException.Message.Contains("Sections"))
                    {
                        TempData["ErrorMessage"] = "Database error: Section not found.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Database constraint error: {ex.InnerException.Message}";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"Save failed: {ex.Message}";
                }
            }

            return RedirectToAction("Index", new { departmentId, classId, sectionId, date });
        }


        [HttpGet]
        public async Task<IActionResult> DebugAttendance()
        {
            var debugInfo = new System.Text.StringBuilder();

            try
            {
                // Check if Attendance table exists and has data
                var attendanceCount = await _context.Attendances.CountAsync();
                debugInfo.AppendLine($"Attendance table has {attendanceCount} records");

                // Check column names
                debugInfo.AppendLine("\nTrying to describe Attendance table structure:");

                // Try to add a test record
                var testAttendance = new Attendance
                {
                    StudentId = 1,
                    ClassId = 1,
                    DepartmentId = 1,
                    SectionId = 1,
                    Date = DateTime.Now,
                    Status = AttendanceStatus.Present,
                    TeacherId = 1
                };

                try
                {
                    await _context.Attendances.AddAsync(testAttendance);
                    await _context.SaveChangesAsync();
                    debugInfo.AppendLine("✓ Test record added successfully");

                    // Remove the test record
                    _context.Attendances.Remove(testAttendance);
                    await _context.SaveChangesAsync();
                    debugInfo.AppendLine("✓ Test record removed successfully");
                }
                catch (Exception ex)
                {
                    debugInfo.AppendLine($"✗ Error adding test record: {GetInnermostExceptionMessage(ex)}");
                }

                // Check if Teacher with ID 1 exists
                var teacherExists = await _context.Teachers.AnyAsync(t => t.TeacherId == 1);
                debugInfo.AppendLine($"\nTeacher with ID 1 exists: {teacherExists}");

                // Check if Student with ID 1 exists
                var studentExists = await _context.Students.AnyAsync(s => s.StudentId == 1);
                debugInfo.AppendLine($"Student with ID 1 exists: {studentExists}");

                // Check if Class with ID 1 exists
                var classExists = await _context.Classes.AnyAsync(c => c.ClassId == 1);
                debugInfo.AppendLine($"Class with ID 1 exists: {classExists}");

                // Check if Department with ID 1 exists
                var deptExists = await _context.Departments.AnyAsync(d => d.DepartmentId == 1);
                debugInfo.AppendLine($"Department with ID 1 exists: {deptExists}");

                // Check if Section with ID 1 exists
                var sectionExists = await _context.Sections.AnyAsync(s => s.SectionId == 1);
                debugInfo.AppendLine($"Section with ID 1 exists: {sectionExists}");
            }
            catch (Exception ex)
            {
                debugInfo.AppendLine($"Error during debug: {ex.Message}");
            }

            return Content(debugInfo.ToString(), "text/plain");
        }


        // Helper method to get the innermost exception message
        private string GetInnermostExceptionMessage(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex.Message;
        }

        // Add these model classes at the end of your controller file
        public class SaveAttendanceModel
        {
            public int DepartmentId { get; set; }
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public DateTime Date { get; set; }
            public List<AttendanceItem> AttendanceItems { get; set; } = new List<AttendanceItem>();
        }

        public class AttendanceItem
        {
            public int StudentId { get; set; }
            public AttendanceStatus Status { get; set; }
        }

        // Monthly report page
        public async Task<IActionResult> MonthlyReport(int departmentId, int classId, int sectionId, int month, int year)
        {
            if (month < 1 || month > 12)
                month = DateTime.Today.Month;

            if (year < 2000 || year > DateTime.Today.Year + 1)
                year = DateTime.Today.Year;

            // Validate inputs
            if (departmentId <= 0 || classId <= 0 || sectionId <= 0)
            {
                TempData["ErrorMessage"] = "Please select department, class, and section";
                return RedirectToAction("Index");
            }

            DateTime startDate;
            DateTime endDate;

            try
            {
                startDate = new DateTime(year, month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Fallback to current month
                startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
                month = DateTime.Today.Month;
                year = DateTime.Today.Year;
            }

            // Get students
            var students = await _context.Students
                .Include(s => s.Class)
                .Include(s => s.Section)
                .Where(s => s.DepartmentId == departmentId
                    && s.ClassId == classId
                    && s.SectionId == sectionId
                    && s.IsActive)
                .OrderBy(s => s.RegNo)
                .ToListAsync();

            var studentIds = students.Select(s => s.StudentId).ToList();

            // Get attendance for this period
            var attendances = await _context.Attendances
                .Where(a => a.Date >= startDate && a.Date <= endDate
                    && studentIds.Contains(a.StudentId)
                    && a.DepartmentId == departmentId
                    && a.ClassId == classId
                    && a.SectionId == sectionId)
                .ToListAsync();

            // Create report
            var report = new List<MonthlyAttendanceReport>();

            foreach (var student in students)
            {
                var studentAttendances = attendances.Where(a => a.StudentId == student.StudentId).ToList();

                var item = new MonthlyAttendanceReport
                {
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    RegNo = student.RegNo,
                    TotalDays = (endDate - startDate).Days + 1,
                    PresentDays = studentAttendances.Count(a => a.Status == AttendanceStatus.Present),
                    AbsentDays = studentAttendances.Count(a => a.Status == AttendanceStatus.Absent),
                    LateDays = studentAttendances.Count(a => a.Status == AttendanceStatus.Late),
                    LeaveDays = 0
                };

                item.AttendancePercentage = item.TotalDays > 0
                    ? (item.PresentDays * 100.0m) / item.TotalDays
                    : 0;

                report.Add(item);
            }

            var model = new MonthlyReportViewModel
            {
                DepartmentId = departmentId,
                ClassId = classId,
                SectionId = sectionId,
                Month = month,
                Year = year,
                Reports = report
            };

            // Get additional info
            var classInfo = await _context.Classes
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.SectionId == sectionId);

            model.ClassName = classInfo?.ClassName ?? "N/A";
            model.DepartmentName = classInfo?.Department?.DepartmentName ?? "N/A";
            model.SectionName = section?.SectionName ?? "N/A";
            model.MonthYear = $"{new DateTime(year, month, 1):MMMM yyyy}";

            return View(model);
        }

        // Helper methods
        private async Task<List<Department>> GetTeacherDepartmentsAsync(int teacherId)
        {
            if (User.IsInRole("Admin"))
            {
                // Admin can see all departments
                return await _context.Departments
                    .OrderBy(d => d.DepartmentName)
                    .ToListAsync();
            }
            else
            {
                // Teacher only sees departments they teach in
                return await _context.ClassSubjects
                    .Include(cs => cs.Class)
                        .ThenInclude(c => c.Department)
                    .Where(cs => cs.TeacherId == teacherId)
                    .Select(cs => cs.Class.Department)
                    .Distinct()
                    .OrderBy(d => d.DepartmentName)
                    .ToListAsync();
            }
        }

        private async Task<List<Class>> GetClassesByDepartmentAsync(int teacherId, int departmentId)
        {
            if (User.IsInRole("Admin"))
            {
                // Admin can see all classes in department
                return await _context.Classes
                    .Where(c => c.DepartmentId == departmentId)
                    .OrderBy(c => c.ClassName)
                    .ToListAsync();
            }
            else
            {
                // Teacher only sees classes they teach
                return await _context.ClassSubjects
                    .Include(cs => cs.Class)
                    .Where(cs => cs.TeacherId == teacherId && cs.Class.DepartmentId == departmentId)
                    .Select(cs => cs.Class)
                    .Distinct()
                    .OrderBy(c => c.ClassName)
                    .ToListAsync();
            }
        }

        private async Task<List<Section>> GetSectionsByClassAsync(int departmentId, int classId)
        {
            return await _context.Sections
                .Where(s => s.DepartmentId == departmentId && s.ClassId == classId)
                .OrderBy(s => s.SectionName)
                .ToListAsync();
        }

        private async Task<List<StudentAttendanceView>> GetClassAttendanceAsync(int departmentId, int classId, int sectionId, DateTime date)
        {
            // Get active students
            var students = await _context.Students
                .Include(s => s.Class)
                    .ThenInclude(c => c.Department)
                .Include(s => s.Section)
                .Where(s => s.DepartmentId == departmentId
                    && s.ClassId == classId
                    && s.SectionId == sectionId
                    && s.IsActive)
                .OrderBy(s => s.RegNo)
                .Select(s => new StudentAttendanceView
                {
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    RegNo = s.RegNo,
                    Gender = s.Gender != null ? s.Gender.ToString() : "Unknown",
                    ClassName = s.Class.ClassName,
                    SectionName = s.Section.SectionName,
                    DepartmentName = s.Class.Department.DepartmentName,
                    DepartmentId = s.DepartmentId,
                    ClassId = s.ClassId,
                    SectionId = s.SectionId,
                    RollNumber = s.RegNo,
                    Status = AttendanceStatus.Absent, // Default
                    AttendanceId = null,
                    MarkedBy = null,
                    MarkedAt = null
                })
                .ToListAsync();

          
            var studentIds = students.Select(s => s.StudentId).ToList();
            var todayAttendances = await _context.Attendances
                .Include(a => a.Teacher)
                .Where(a => a.Date.Date == date.Date
                    && a.DepartmentId == departmentId
                    && a.ClassId == classId
                    && a.SectionId == sectionId 
                    && studentIds.Contains(a.StudentId))
                .ToListAsync();

            var attendanceDict = todayAttendances.ToDictionary(a => a.StudentId);

          
            foreach (var student in students)
            {
                if (attendanceDict.TryGetValue(student.StudentId, out var attendance))
                {
                    student.Status = attendance.Status;
                    student.AttendanceId = attendance.AttendanceId;
                    student.MarkedBy = attendance.Teacher?.Name;
                    student.MarkedAt = attendance.Date;
                }
            }

            return students;
        }

        private async Task<AttendanceSummary> GetAttendanceSummaryAsync(int departmentId, int classId, int sectionId, DateTime date)
        {
            // Count total active students
            var totalStudents = await _context.Students
                .CountAsync(s => s.DepartmentId == departmentId
                    && s.ClassId == classId
                    && s.SectionId == sectionId
                    && s.IsActive);

            // Get student IDs
            var studentIds = await _context.Students
                .Where(s => s.DepartmentId == departmentId
                    && s.ClassId == classId
                    && s.SectionId == sectionId
                    && s.IsActive)
                .Select(s => s.StudentId)
                .ToListAsync();

            // Get today's attendance
            var attendances = await _context.Attendances
                .Where(a => a.Date.Date == date.Date
                    && a.DepartmentId == departmentId
                    && a.ClassId == classId
                    && a.SectionId == sectionId
                    && studentIds.Contains(a.StudentId))
                .ToListAsync();

            // Create summary
            return new AttendanceSummary
            {
                TotalStudents = totalStudents,
                Present = attendances.Count(a => a.Status == AttendanceStatus.Present),
                Absent = attendances.Count(a => a.Status == AttendanceStatus.Absent),
                Late = attendances.Count(a => a.Status == AttendanceStatus.Late),
                Leave = 0,
                NotMarked = totalStudents - attendances.Count
            };
        }

        private async Task<int> GetCurrentTeacherIdAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("No user ID found in claims");
                    return 0;
                }

                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.UserId == userId);

                if (teacher == null)
                {
                    _logger.LogWarning($"No teacher found for user ID: {userId}");

                    // If admin user, try to get any teacher ID or use a default
                    if (User.IsInRole("Admin"))
                    {
                        var firstTeacher = await _context.Teachers.FirstOrDefaultAsync();
                        if (firstTeacher != null)
                        {
                            _logger.LogInformation($"Admin using teacher ID: {firstTeacher.TeacherId}");
                            return firstTeacher.TeacherId;
                        }
                    }

                    return 0;
                }

                _logger.LogInformation($"Found teacher ID: {teacher.TeacherId} for user: {userId}");
                return teacher.TeacherId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teacher ID");
                return 0;
            }
        }
    }

    // View Models
    public class AttendanceViewModel
    {
        public DateTime SelectedDate { get; set; }
        public int? DepartmentId { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }

        public string? DepartmentName { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }

        public List<Department> Departments { get; set; } = new List<Department>();
        public List<Class> Classes { get; set; } = new List<Class>();
        public List<Section> Sections { get; set; } = new List<Section>();

        public List<StudentAttendanceView> Students { get; set; } = new List<StudentAttendanceView>();
        public AttendanceSummary? Summary { get; set; }
    }

    public class StudentAttendanceView
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = default!;
        public string RegNo { get; set; } = default!;
        public string Gender { get; set; } = default!;
        public string ClassName { get; set; } = default!;
        public string SectionName { get; set; } = default!;
        public string DepartmentName { get; set; } = default!;
        public int DepartmentId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string RollNumber { get; set; } = default!;
        public AttendanceStatus Status { get; set; }
        public int? AttendanceId { get; set; }
        public string? MarkedBy { get; set; }
        public DateTimeOffset? MarkedAt { get; set; }
    }

    public class AttendanceSummary
    {
        public int TotalStudents { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Leave { get; set; }
        public int NotMarked { get; set; }

        public decimal PresentPercentage => TotalStudents > 0 ? (Present * 100.0m) / TotalStudents : 0;
    }

    public class MonthlyAttendanceReport
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = default!;
        public string RegNo { get; set; } = default!;
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public int LeaveDays { get; set; }
        public decimal AttendancePercentage { get; set; }
    }

    public class MonthlyReportViewModel
    {
        public int DepartmentId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? DepartmentName { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public string? MonthYear { get; set; }
        public List<MonthlyAttendanceReport> Reports { get; set; } = new List<MonthlyAttendanceReport>();
    }
}