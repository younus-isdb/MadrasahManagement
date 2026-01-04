using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Controllers
{
    public class ExamRoutineController : Controller
    {
        private readonly MadrasahDbContext _context;

        public ExamRoutineController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET: Index (Grouped view)
        public async Task<IActionResult> Index()
        {
            var examRoutines = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Department)
                .Include(e => e.Subject)
                .ToListAsync();

            // Group by EducationYear, ClassId, ExamId
            var grouped = examRoutines
                .GroupBy(e => new { e.EducationYear, e.ClassId, e.ExamId })
                .Select(g => new ExamRoutineGroupViewModel
                {
                    EducationYear = g.Key.EducationYear,
                    ClassId = g.Key.ClassId,
                    ClassName = g.First().Class?.ClassName ?? "Unknown",
                    ExamId = g.Key.ExamId,
                    ExamName = g.First().Examination?.ExamName ?? "Unknown",
                    DepartmentId = g.First().DepartmentId,
                    DepartmentName = g.First().Department?.DepartmentName ?? "Unknown",
                    RoomNumber = g.First().RoomNumber,
                    Subjects = g.Select(r => new RoutineSubjectViewModel
                    {
                        SubjectId = r.SubjectId,
                        SubjectCode = r.Subject?.SubjectCode ?? "",  // Add subject code
                        SubjectName = r.Subject?.SubjectName ?? "Unknown",
                        ExamDate = r.ExamDate,
                        ExamDay = r.ExamDay,
                        ExamStartTime = r.ExamStartTime,
                        ExamEndTime = r.ExamEndTime
                    }).ToList()
                })
                .OrderByDescending(g => g.EducationYear)
                .ThenBy(g => g.ClassName)
                .ThenBy(g => g.ExamName)
                .ToList();

            return View(grouped);
        }

        // GET: Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            var viewModel = new ExamRoutineBatchViewModel
            {
                EducationYear = DateTime.Now.Year.ToString(),
                Subjects = new List<RoutineSubjectViewModel>()
            };
            return View(viewModel);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamRoutineBatchViewModel viewModel)
        {
            // First, validate master fields
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(viewModel);
            }

            try
            {
                // Manually bind Subjects from form data for dynamic rows
                var subjects = new List<RoutineSubjectViewModel>();
                var subjectIds = new List<int>();

                // Find all subject indices in the form
                foreach (var key in Request.Form.Keys)
                {
                    if (key.Contains("Subjects[") && key.Contains("].SubjectId") && !key.Contains("error"))
                    {
                        // Extract index from key like "Subjects[0].SubjectId"
                        var start = key.IndexOf('[') + 1;
                        var end = key.IndexOf(']');
                        if (start > 0 && end > start)
                        {
                            var indexStr = key.Substring(start, end - start);
                            if (int.TryParse(indexStr, out var index))
                            {
                                subjectIds.Add(index);
                            }
                        }
                    }
                }

                // Remove duplicates and sort
                subjectIds = subjectIds.Distinct().OrderBy(i => i).ToList();

                // Create subject objects from form data
                foreach (var index in subjectIds)
                {
                    var subjectIdStr = Request.Form[$"Subjects[{index}].SubjectId"];
                    var examDateStr = Request.Form[$"Subjects[{index}].ExamDate"];
                    var examDayStr = Request.Form[$"Subjects[{index}].ExamDay"];
                    var startTimeStr = Request.Form[$"Subjects[{index}].ExamStartTime"];
                    var endTimeStr = Request.Form[$"Subjects[{index}].ExamEndTime"];

                    if (!string.IsNullOrEmpty(subjectIdStr) && int.TryParse(subjectIdStr, out var subjectId))
                    {
                        var subject = new RoutineSubjectViewModel
                        {
                            SubjectId = subjectId,
                            ExamDate = DateTime.TryParse(examDateStr, out var date) ? date : DateTime.Now,
                            ExamDay = examDayStr,
                            ExamStartTime = startTimeStr,
                            ExamEndTime = endTimeStr
                        };

                        // Get subject name and code from database
                        var dbSubject = await _context.Subjects.FindAsync(subject.SubjectId);
                        if (dbSubject != null)
                        {
                            subject.SubjectName = dbSubject.SubjectName;
                            subject.SubjectCode = dbSubject.SubjectCode ?? "";
                        }
                        else
                        {
                            subject.SubjectName = "Unknown";
                            subject.SubjectCode = "";
                        }

                        subjects.Add(subject);
                    }
                }

                // Assign to viewModel
                viewModel.Subjects = subjects;

                // Validate at least one subject
                if (!viewModel.Subjects.Any())
                {
                    ModelState.AddModelError("", "At least one subject routine is required.");
                    PopulateDropdowns();
                    return View(viewModel);
                }

                // Validate each subject
                foreach (var subject in viewModel.Subjects)
                {
                    if (subject.SubjectId <= 0)
                    {
                        ModelState.AddModelError("", "Please select a valid subject for all rows.");
                        PopulateDropdowns();
                        return View(viewModel);
                    }

                    // Validate time
                    if (!TimeSpan.TryParse(subject.ExamStartTime, out var start) ||
                        !TimeSpan.TryParse(subject.ExamEndTime, out var end) ||
                        end <= start)
                    {
                        ModelState.AddModelError("", $"Invalid time range for subject: {subject.SubjectName}.");
                        PopulateDropdowns();
                        return View(viewModel);
                    }

                    // Auto-calculate day from DateTime if not provided
                    if (string.IsNullOrEmpty(subject.ExamDay))
                    {
                        subject.ExamDay = subject.ExamDate.ToString("dddd");
                    }
                }

                // Check for existing routines
                var existingSubjectIds = viewModel.Subjects.Select(s => s.SubjectId).ToList();
                var existingRoutines = await _context.ExamRoutines
                    .Where(e => e.EducationYear == viewModel.EducationYear
                        && e.ClassId == viewModel.ClassId
                        && e.ExamId == viewModel.ExamId
                        && existingSubjectIds.Contains(e.SubjectId))
                    .Select(e => e.SubjectId)
                    .ToListAsync();

                if (existingRoutines.Any())
                {
                    var existingSubjects = viewModel.Subjects
                        .Where(s => existingRoutines.Contains(s.SubjectId))
                        .Select(s => s.SubjectName);

                    ModelState.AddModelError("",
                        $"Some subjects already have routines for this class and exam: {string.Join(", ", existingSubjects)}");
                    PopulateDropdowns();
                    return View(viewModel);
                }

                // Create multiple ExamRoutine records
                foreach (var subject in viewModel.Subjects)
                {
                    var examRoutine = new ExamRoutine
                    {
                        EducationYear = viewModel.EducationYear,
                        DepartmentId = viewModel.DepartmentId,
                        ClassId = viewModel.ClassId,
                        ExamId = viewModel.ExamId,
                        SubjectId = subject.SubjectId,
                        RoomNumber = viewModel.RoomNumber,  // Use master RoomNumber
                        ExamDate = subject.ExamDate,
                        ExamDay = subject.ExamDay,
                        ExamStartTime = subject.ExamStartTime,
                        ExamEndTime = subject.ExamEndTime
                    };

                    _context.ExamRoutines.Add(examRoutine);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Exam routine created successfully with {viewModel.Subjects.Count} subject(s)!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                PopulateDropdowns();
                return View(viewModel);
            }
        }

        // GET: Edit entire routine group
        public async Task<IActionResult> Edit(string educationYear, int classId, int examId)
        {
            var routines = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Department)
                .Include(e => e.Subject)
                .Where(e => e.EducationYear == educationYear
                    && e.ClassId == classId
                    && e.ExamId == examId)
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.ExamStartTime)
                .ToListAsync();

            if (!routines.Any())
                return NotFound();

            var viewModel = new ExamRoutineBatchViewModel
            {
                EducationYear = educationYear,
                DepartmentId = routines.First().DepartmentId,
                ClassId = classId,
                ExamId = examId,
                RoomNumber = routines.First().RoomNumber,
                Subjects = routines.Select(r => new RoutineSubjectViewModel
                {
                    ExamRoutineId = r.ExamRoutineId,
                    SubjectId = r.SubjectId,
                    SubjectCode = r.Subject?.SubjectCode ?? "",
                    SubjectName = r.Subject?.SubjectName ?? "Unknown",
                    ExamDate = r.ExamDate,
                    ExamDay = r.ExamDay,
                    ExamStartTime = r.ExamStartTime,
                    ExamEndTime = r.ExamEndTime
                }).ToList()
            };

            // Get display names
            var department = await _context.Departments.FindAsync(viewModel.DepartmentId);
            var classInfo = await _context.Classes.FindAsync(viewModel.ClassId);
            var exam = await _context.Examinations.FindAsync(viewModel.ExamId);

            ViewBag.DepartmentName = department?.DepartmentName ?? "Unknown";
            ViewBag.ClassName = classInfo?.ClassName ?? "Unknown";
            ViewBag.ExamName = exam?.ExamName ?? "Unknown";

            // Get all subjects for this class for the dropdown
            var subjects = await _context.Subjects
                .Where(s => s.ClassId == classId)
                .Select(s => new
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    SubjectCode = s.SubjectCode ?? ""
                })
                .ToListAsync();

            ViewBag.Subjects = subjects;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExamRoutineBatchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing routines for this group
                    var existingRoutines = await _context.ExamRoutines
                        .Where(e => e.EducationYear == viewModel.EducationYear
                            && e.ClassId == viewModel.ClassId
                            && e.ExamId == viewModel.ExamId)
                        .ToListAsync();

                    // Process each subject in the form
                    for (int i = 0; i < viewModel.Subjects.Count; i++)
                    {
                        var subject = viewModel.Subjects[i];

                        if (subject.ExamRoutineId > 0)
                        {
                            // Update existing routine
                            var existingRoutine = existingRoutines
                                .FirstOrDefault(e => e.ExamRoutineId == subject.ExamRoutineId);

                            if (existingRoutine != null)
                            {
                                existingRoutine.SubjectId = subject.SubjectId;
                                existingRoutine.RoomNumber = viewModel.RoomNumber;
                                existingRoutine.ExamDate = subject.ExamDate;
                                existingRoutine.ExamDay = subject.ExamDay;
                                existingRoutine.ExamStartTime = subject.ExamStartTime;
                                existingRoutine.ExamEndTime = subject.ExamEndTime;
                            }
                        }
                        else
                        {
                            // Add new routine
                            var newRoutine = new ExamRoutine
                            {
                                EducationYear = viewModel.EducationYear,
                                DepartmentId = viewModel.DepartmentId,
                                ClassId = viewModel.ClassId,
                                ExamId = viewModel.ExamId,
                                RoomNumber = viewModel.RoomNumber,
                                SubjectId = subject.SubjectId,
                                ExamDate = subject.ExamDate,
                                ExamDay = subject.ExamDay,
                                ExamStartTime = subject.ExamStartTime,
                                ExamEndTime = subject.ExamEndTime
                            };
                            _context.ExamRoutines.Add(newRoutine);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Exam routine updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            // If we get here, something went wrong
            // Repopulate ViewBag data
            var department = await _context.Departments.FindAsync(viewModel.DepartmentId);
            var classInfo = await _context.Classes.FindAsync(viewModel.ClassId);
            var exam = await _context.Examinations.FindAsync(viewModel.ExamId);

            ViewBag.DepartmentName = department?.DepartmentName ?? "Unknown";
            ViewBag.ClassName = classInfo?.ClassName ?? "Unknown";
            ViewBag.ExamName = exam?.ExamName ?? "Unknown";

            var subjects = await _context.Subjects
                .Where(s => s.ClassId == viewModel.ClassId)
                .Select(s => new
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    SubjectCode = s.SubjectCode ?? ""
                })
                .ToListAsync();

            ViewBag.Subjects = subjects;

            return View(viewModel);
        }

        // GET: Details for a specific group
        public async Task<IActionResult> Details(string educationYear, int classId, int examId)
        {
            var routines = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Department)
                .Include(e => e.Subject)
                .Where(e => e.EducationYear == educationYear
                    && e.ClassId == classId
                    && e.ExamId == examId)
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.ExamStartTime)
                .ToListAsync();

            if (!routines.Any())
                return NotFound();

            var viewModel = new ExamRoutineGroupViewModel
            {
                EducationYear = educationYear,
                ClassId = classId,
                ClassName = routines.First().Class?.ClassName ?? "Unknown",
                ExamId = examId,
                ExamName = routines.First().Examination?.ExamName ?? "Unknown",
                DepartmentId = routines.First().DepartmentId,
                DepartmentName = routines.First().Department?.DepartmentName ?? "Unknown",
                RoomNumber = routines.First().RoomNumber,
                Subjects = routines.Select(r => new RoutineSubjectViewModel
                {
                    SubjectId = r.SubjectId,
                    SubjectCode = r.Subject?.SubjectCode ?? "",
                    SubjectName = r.Subject?.SubjectName ?? "Unknown",
                    ExamDate = r.ExamDate,
                    ExamDay = r.ExamDay,
                    ExamStartTime = r.ExamStartTime,
                    ExamEndTime = r.ExamEndTime
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Delete entire group
        public async Task<IActionResult> Delete(string educationYear, int classId, int examId)
        {
            var routines = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Department)
                .Include(e => e.Subject) // Include Subject to get actual data
                .Where(e => e.EducationYear == educationYear
                    && e.ClassId == classId
                    && e.ExamId == examId)
                .ToListAsync();

            if (!routines.Any())
                return NotFound();

            var viewModel = new ExamRoutineGroupViewModel
            {
                EducationYear = educationYear,
                ClassId = classId,
                ClassName = routines.First().Class?.ClassName ?? "Unknown",
                ExamId = examId,
                ExamName = routines.First().Examination?.ExamName ?? "Unknown",
                DepartmentId = routines.First().DepartmentId,
                DepartmentName = routines.First().Department?.DepartmentName ?? "Unknown",
                RoomNumber = routines.First().RoomNumber,
                Subjects = routines.Select(r => new RoutineSubjectViewModel
                {
                    SubjectId = r.SubjectId,
                    SubjectCode = r.Subject?.SubjectCode ?? "",
                    SubjectName = r.Subject?.SubjectName ?? "Unknown", // Get actual subject name
                    ExamDate = r.ExamDate,
                    ExamDay = r.ExamDay,
                    ExamStartTime = r.ExamStartTime,
                    ExamEndTime = r.ExamEndTime
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Delete entire group
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string educationYear, int classId, int examId)
        {
            var routines = await _context.ExamRoutines
                .Where(e => e.EducationYear == educationYear
                    && e.ClassId == classId
                    && e.ExamId == examId)
                .ToListAsync();

            if (routines.Any())
            {
                _context.ExamRoutines.RemoveRange(routines);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exam routine deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX: Get classes by department
        [HttpGet]
        public async Task<IActionResult> GetClassesByDepartment(int departmentId)
        {
            var classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId || c.DepartmentId == null)
                .OrderBy(c => c.ClassName)
                .Select(c => new { value = c.ClassId, text = c.ClassName })
                .ToListAsync();

            return Json(classes);
        }

        // AJAX: Get subjects by class (with subject code)
        [HttpGet]
        public async Task<IActionResult> GetSubjectsByClass(int classId)
        {
            var subjects = await _context.Subjects
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.SubjectName)
                .Select(s => new {
                    value = s.SubjectId,
                    text = s.SubjectName,
                    code = s.SubjectCode ?? ""
                })
                .ToListAsync();

            return Json(subjects);
        }

        // AJAX: Get available subjects for add-row functionality
        [HttpGet]
        public async Task<IActionResult> GetAvailableSubjects(int classId, string educationYear, int examId)
        {
            if (classId <= 0 || string.IsNullOrEmpty(educationYear) || examId <= 0)
            {
                return Json(new List<object>());
            }

            // Get all subjects for the class
            var allSubjects = await _context.Subjects
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.SubjectName)
                .ToListAsync();

            // Get subjects that already have a routine for this exam
            var existingSubjectIds = await _context.ExamRoutines
                .Where(e => e.EducationYear == educationYear
                    && e.ClassId == classId
                    && e.ExamId == examId)
                .Select(e => e.SubjectId)
                .ToListAsync();

            // Filter out subjects that already have routines
            var availableSubjects = allSubjects
                .Where(s => !existingSubjectIds.Contains(s.SubjectId))
                .Select(s => new {
                    value = s.SubjectId,
                    text = s.SubjectName,
                    code = s.SubjectCode ?? ""
                })
                .ToList();

            return Json(availableSubjects);
        }

        // AJAX: Get the subject row partial view
        public IActionResult _SubjectRowPartial(int rowIndex)
        {
            ViewBag.RowIndex = rowIndex;
            return PartialView("_SubjectRowPartial");
        }

        // AJAX: Add a new subject row
        public IActionResult AddSubjectRow(int rowIndex, int classId, bool isEdit = false)
        {
            // Get subjects for the specific class
            var subjects = _context.Subjects
                .Where(s => s.ClassId == classId)
                .Select(s => new
                {
                    Value = s.SubjectId.ToString(),
                    Text = s.SubjectName,
                    Code = s.SubjectCode ?? ""
                })
                .ToList();

            ViewData["RowIndex"] = rowIndex;
            ViewData["IsEdit"] = isEdit;
            ViewBag.SubjectsList = subjects;

            // Return a new empty subject row
            var viewModel = new RoutineSubjectViewModel
            {
                ExamRoutineId = 0,
                SubjectId = 0,
                SubjectCode = "",
                SubjectName = "",
                ExamDate = DateTime.Today,
                ExamDay = DateTime.Today.ToString("dddd"),
                ExamStartTime = "09:00",
                ExamEndTime = "11:00"
            };

            return PartialView("_SubjectRowPartial", viewModel);
        }
        private void PopulateDropdowns()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments.OrderBy(d => d.DepartmentName), "DepartmentId", "DepartmentName");
            ViewData["ExamId"] = new SelectList(_context.Examinations.OrderBy(e => e.ExamName), "ExamId", "ExamName");
        }
    }
}