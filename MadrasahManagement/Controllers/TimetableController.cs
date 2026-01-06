using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using System.Text.Json;

namespace MadrasahManagement.Controllers
{
    public class TimetableController : Controller
    {
        private readonly MadrasahDbContext _context;

        public TimetableController(MadrasahDbContext context)
        {
            _context = context;
        }

        /* =========================
           INDEX (List all timetables)
           ========================= */
        public async Task<IActionResult> Index()
        {
            var timetables = await _context.Timetables
                .Include(t => t.Department)
                .Include(t => t.Class)
                .Include(t => t.Section)
                .GroupBy(t => new
                {
                    t.AcademicYear,
                    t.DepartmentId,    
                    t.ClassId,          
                    t.SectionId,       
                    t.Department.DepartmentName,
                    t.Class.ClassName,
                    t.Section.SectionName
                })
                .Select(g => new TimetableSummaryVM
                {
                    AcademicYear = g.Key.AcademicYear,
                    DepartmentId = g.Key.DepartmentId,    
                    ClassId = g.Key.ClassId,              
                    SectionId = g.Key.SectionId,
                    DepartmentName = g.First().Department.DepartmentName,
                    ClassName = g.First().Class.ClassName,
                    SectionName = g.First().Section.SectionName,
                    TotalPeriods = g.Count(),
                    BreakPeriods = g.Count(x => x.IsBreak),
                    AssemblyPeriods = g.Count(x => x.IsAssembly),
                    CreatedDate = g.Max(x => x.CreatedDate)
                })
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();

            return View(timetables);
        }

        /* =========================
           CREATE - STEP 1: SELECTION
           ========================= */
        public async Task<IActionResult> Create()
        {
            var vm = new TimetableSelectionVM
            {
                AcademicYears = await GetAcademicYearsAsync(),
                Departments = await GetDepartmentsAsync()
            };

            return View(vm);
        }

        /* =========================
           CREATE - STEP 2: GRID EDITOR
           ========================= */
        [HttpGet]
        public async Task<IActionResult> CreateGrid(string academicYear, int departmentId, int classId, int sectionId)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(academicYear) || departmentId <= 0 || classId <= 0 || sectionId <= 0)
            {
                TempData["Error"] = "Please select all required fields";
                return RedirectToAction("Create");
            }

            var vm = new TimetableGridVM
            {
                AcademicYear = academicYear,
                DepartmentId = departmentId,
                ClassId = classId,
                SectionId = sectionId
            };

            // Get display names
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);
            var classInfo = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassId == classId);
            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.SectionId == sectionId);

            if (department == null || classInfo == null || section == null)
            {
                TempData["Error"] = "Invalid selection. Please try again.";
                return RedirectToAction("Create");
            }

            vm.DepartmentName = department.DepartmentName;
            vm.ClassName = classInfo.ClassName;
            vm.SectionName = section.SectionName;

            // Setup days and periods
            vm.Days = new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
            vm.Periods = PeriodBlueprintVM.DailyPeriods;

            // Load filtered subjects and teachers
            await LoadFilteredData(vm);

            // Initialize grid
            InitializeGrid(vm);

            // Load existing timetable if any
            await LoadExistingTimetable(vm);

            return View(vm);
        }

        /* =========================
           SAVE TIMETABLE (POST)
           ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SaveTimetableVM vm)
        {
            Console.WriteLine($"Save called with: {vm.AcademicYear}, Dept:{vm.DepartmentId}, Class:{vm.ClassId}, Section:{vm.SectionId}");
            Console.WriteLine($"Number of cells: {vm.Cells?.Count}");

            if (vm.Cells != null)
            {
                foreach (var cell in vm.Cells)
                {
                    Console.WriteLine($"Cell: {cell.Day} - {cell.Period}, Subject: {cell.SubjectId}, Teacher: {cell.TeacherId}, Break: {cell.IsBreak}, Assembly: {cell.IsAssembly}");
                }
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState Invalid:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }

                // Return with errors
                return await LoadGridForEditing(vm);
            }

            try
            {
                // Debug: Check database connection
                Console.WriteLine("Database connection test...");
                var test = await _context.Departments.AnyAsync();
                Console.WriteLine($"Database accessible: {test}");

                using var transaction = await _context.Database.BeginTransactionAsync();

                // Remove existing
                var existing = await _context.Timetables
                    .Where(t =>
                        t.AcademicYear == vm.AcademicYear &&
                        t.DepartmentId == vm.DepartmentId &&
                        t.ClassId == vm.ClassId &&
                        t.SectionId == vm.SectionId)
                    .ToListAsync();

                Console.WriteLine($"Removing {existing.Count} existing entries");
                _context.Timetables.RemoveRange(existing);

                // Add new entries
                int addedCount = 0;
                foreach (var cell in vm.Cells)
                {
                    var periodInfo = PeriodBlueprintVM.DailyPeriods
                        .FirstOrDefault(p => p.PeriodName == cell.Period);

                    if (periodInfo == null)
                    {
                        Console.WriteLine($"Warning: Period {cell.Period} not found in blueprint");
                        continue;
                    }

                    // Validate for regular periods
                    if (!cell.IsBreak && !cell.IsAssembly)
                    {
                        if (!cell.SubjectId.HasValue)
                        {
                            ModelState.AddModelError("", $"Subject is required for {cell.Day} - {cell.Period}");
                            return await LoadGridForEditing(vm);
                        }

                        if (!cell.TeacherId.HasValue)
                        {
                            ModelState.AddModelError("", $"Teacher is required for {cell.Day} - {cell.Period}");
                            return await LoadGridForEditing(vm);
                        }

                        // Validate teacher is assigned to this subject
                        var isValidAssignment = await _context.ClassSubjects
                            .AnyAsync(cs => cs.ClassId == vm.ClassId &&
                                           cs.SubjectId == cell.SubjectId &&
                                           cs.TeacherId == cell.TeacherId);

                        if (!isValidAssignment)
                        {
                            Console.WriteLine($"Warning: Teacher {cell.TeacherId} not assigned to subject {cell.SubjectId} for class {vm.ClassId}");
                            // Continue anyway? Or add error?
                            // For now, let's continue but log it
                        }
                    }

                    var timetable = new Timetable
                    {
                        AcademicYear = vm.AcademicYear,
                        DepartmentId = vm.DepartmentId,
                        ClassId = vm.ClassId,
                        SectionId = vm.SectionId,
                        DayName = cell.Day,
                        PeriodName = cell.Period,
                        SubjectId = cell.IsBreak || cell.IsAssembly ? null : cell.SubjectId,
                        TeacherId = cell.IsBreak || cell.IsAssembly ? null : cell.TeacherId,
                        IsBreak = cell.IsBreak,
                        IsAssembly = cell.IsAssembly,
                        StartTime = TimeSpan.Parse(periodInfo.StartTime),
                        EndTime = TimeSpan.Parse(periodInfo.EndTime),
                        CreatedDate = DateTime.Now
                    };

                    Console.WriteLine($"Adding: {timetable.DayName} {timetable.PeriodName}, Subject: {timetable.SubjectId}, Teacher: {timetable.TeacherId}");

                    _context.Timetables.Add(timetable);
                    addedCount++;
                }

                Console.WriteLine($"Attempting to save {addedCount} entries to database...");

                var saved = await _context.SaveChangesAsync();

                Console.WriteLine($"SaveChangesAsync returned: {saved} rows affected");

                await transaction.CommitAsync();

                Console.WriteLine("Transaction committed successfully");

                TempData["Success"] = $"Timetable saved successfully! ({addedCount} periods)";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", $"Error saving timetable: {ex.Message}");
                return await LoadGridForEditing(vm);
            }
        }


        /* =========================
   EDIT TIMETABLE (Direct to Grid)
   ========================= */
        [HttpGet]
        public async Task<IActionResult> Edit(string academicYear, int departmentId, int classId, int sectionId)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(academicYear) || departmentId <= 0 || classId <= 0 || sectionId <= 0)
            {
                TempData["Error"] = "Invalid parameters for editing timetable";
                return RedirectToAction("Index");
            }

            // Check if timetable exists
            var timetableExists = await _context.Timetables
                .AnyAsync(t => t.AcademicYear == academicYear &&
                              t.DepartmentId == departmentId &&
                              t.ClassId == classId &&
                              t.SectionId == sectionId);

            if (!timetableExists)
            {
                TempData["Error"] = "Timetable not found. Create it first.";
                return RedirectToAction("CreateGrid", new { academicYear, departmentId, classId, sectionId });
            }

            // Use the same CreateGrid logic but with "Edit" mode
            var vm = new TimetableGridVM
            {
                AcademicYear = academicYear,
                DepartmentId = departmentId,
                ClassId = classId,
                SectionId = sectionId,
                IsEditMode = true  // Add this flag to ViewModel
            };

            // Get display names
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);
            var classInfo = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassId == classId);
            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.SectionId == sectionId);

            if (department == null || classInfo == null || section == null)
            {
                TempData["Error"] = "Invalid selection. Please try again.";
                return RedirectToAction("Index");
            }

            vm.DepartmentName = department.DepartmentName;
            vm.ClassName = classInfo.ClassName;
            vm.SectionName = section.SectionName;

            // Setup days and periods
            vm.Days = new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
            vm.Periods = PeriodBlueprintVM.DailyPeriods;

            // Load filtered subjects and teachers
            await LoadFilteredData(vm);

            // Initialize grid
            InitializeGrid(vm);

            // Load existing timetable
            await LoadExistingTimetable(vm);

            return View("CreateGrid", vm);  // Use same view but in edit mode
        }

        /* =========================
           VIEW TIMETABLE DETAILS
           ========================= */
        public async Task<IActionResult> Details(string academicYear, int departmentId, int classId, int sectionId)
        {
            var data = await _context.Timetables
                .Include(t => t.Subject)
                .Include(t => t.Teacher)
                .Include(t => t.Department)
                .Include(t => t.Class)
                .Include(t => t.Section)
                .Where(t =>
                    t.AcademicYear == academicYear &&
                    t.DepartmentId == departmentId &&
                    t.ClassId == classId &&
                    t.SectionId == sectionId)
                .OrderBy(t => t.DayName)
                .ThenBy(t => t.StartTime)
                .ToListAsync();

            if (!data.Any())
                return NotFound();

            var vm = new TimetableViewVM
            {
                AcademicYear = academicYear,
                DepartmentName = data.First().Department.DepartmentName,
                ClassName = data.First().Class.ClassName,
                SectionName = data.First().Section.SectionName,
                Days = data.Select(x => x.DayName).Distinct().OrderBy(d => d).ToList(),
                Periods = data.Select(x => new PeriodVM
                {
                    PeriodName = x.PeriodName,
                    StartTime = x.StartTime.ToString(@"hh\:mm"),
                    EndTime = x.EndTime.ToString(@"hh\:mm"),
                    IsBreak = x.IsBreak,
                    IsAssembly = x.IsAssembly
                }).DistinctBy(p => p.PeriodName)
                  .OrderBy(p => p.StartTime)
                  .ToList()
            };

            // Build grid
            foreach (var day in vm.Days)
            {
                vm.Grid[day] = new Dictionary<string, TimetableCellVM>();

                foreach (var period in vm.Periods)
                {
                    var cell = data.FirstOrDefault(x => x.DayName == day && x.PeriodName == period.PeriodName);

                    vm.Grid[day][period.PeriodName] = new TimetableCellVM
                    {
                        SubjectName = cell?.Subject?.SubjectName ?? "",
                        TeacherName = cell?.Teacher?.Name ?? "",
                        IsBreak = cell?.IsBreak ?? false,
                        IsAssembly = cell?.IsAssembly ?? false
                    };
                }
            }

            return View(vm);
        }

        /* =========================
           DELETE CONFIRMATION
           ========================= */
        public async Task<IActionResult> Delete(string academicYear, int departmentId, int classId, int sectionId)
        {
            var exists = await _context.Timetables.AnyAsync(t =>
                t.AcademicYear == academicYear &&
                t.DepartmentId == departmentId &&
                t.ClassId == classId &&
                t.SectionId == sectionId);

            if (!exists)
                return NotFound();

            var info = await _context.Timetables
                .Include(t => t.Department)
                .Include(t => t.Class)
                .Include(t => t.Section)
                .Where(t =>
                    t.AcademicYear == academicYear &&
                    t.DepartmentId == departmentId &&
                    t.ClassId == classId &&
                    t.SectionId == sectionId)
                .FirstOrDefaultAsync();

            ViewBag.TimetableInfo = $"{academicYear} - {info?.Department?.DepartmentName} - {info?.Class?.ClassName} - {info?.Section?.SectionName}";
            ViewBag.AcademicYear = academicYear;
            ViewBag.DepartmentId = departmentId;
            ViewBag.ClassId = classId;
            ViewBag.SectionId = sectionId;

            return View();
        }

        /* =========================
           DELETE CONFIRMED
           ========================= */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string academicYear, int departmentId, int classId, int sectionId)
        {
            var timetableEntries = await _context.Timetables
                .Where(t =>
                    t.AcademicYear == academicYear &&
                    t.DepartmentId == departmentId &&
                    t.ClassId == classId &&
                    t.SectionId == sectionId)
                .ToListAsync();

            if (!timetableEntries.Any())
            {
                TempData["Error"] = "Timetable not found";
                return RedirectToAction("Index");
            }

            _context.Timetables.RemoveRange(timetableEntries);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Timetable deleted successfully!";
            return RedirectToAction("Index");
        }

        /* =========================
           AJAX METHODS FOR DROPDOWNS
           ========================= */
        [HttpGet]
        public async Task<JsonResult> GetClassesByDepartment(int departmentId)
        {
            var classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId)
                .OrderBy(c => c.ClassName)
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName
                })
                .ToListAsync();

            return Json(classes);
        }

        [HttpGet]
        public async Task<JsonResult> GetSectionsByClass(int classId)
        {
            var sections = await _context.Sections
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.SectionName)
                .Select(s => new SelectListItem
                {
                    Value = s.SectionId.ToString(),
                    Text = s.SectionName
                })
                .ToListAsync();

            return Json(sections);
        }

        [HttpGet]
        public async Task<JsonResult> GetTeachersBySubject(int subjectId, int classId)
        {
            var teachers = await _context.ClassSubjects
                .Where(cs => cs.SubjectId == subjectId && cs.ClassId == classId)
                .Select(cs => new SelectListItem
                {
                    Value = cs.TeacherId.ToString(),
                    Text = cs.Teacher.Name
                })
                .Distinct()
                .ToListAsync();

            return Json(teachers);
        }

        /* =========================
           HELPER METHODS
           ========================= */
        private async Task<List<SelectListItem>> GetAcademicYearsAsync()
        {
            var currentYear = DateTime.Now.Year;
            var years = new List<SelectListItem>();

            for (int i = -2; i <= 2; i++)
            {
                var year = $"{currentYear + i}-{currentYear + i + 1}";
                years.Add(new SelectListItem { Value = year, Text = year });
            }

            return years;
        }

        private async Task<List<SelectListItem>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .OrderBy(d => d.DepartmentName)
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .ToListAsync();
        }

        private async Task LoadFilteredData(TimetableGridVM vm)
        {
            // Get subjects for this class
            vm.Subjects = await _context.Subjects
                .Where(s => s.ClassId == vm.ClassId)
                .OrderBy(s => s.SubjectName)
                .Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = s.SubjectName
                })
                .ToListAsync();

            // Get all teachers (or filter by department if needed)
            vm.Teachers = await _context.Teachers
                .OrderBy(t => t.Name)
                .Select(t => new SelectListItem
                {
                    Value = t.TeacherId.ToString(),
                    Text = $"{t.Name} ({t.Department.DepartmentName})"
                })
                .ToListAsync();

            // Add empty options
            vm.Subjects.Insert(0, new SelectListItem { Value = "", Text = "-- Select Subject --" });
            vm.Teachers.Insert(0, new SelectListItem { Value = "", Text = "-- Select Teacher --" });
        }

        private void InitializeGrid(TimetableGridVM vm)
        {
            vm.GridData = new Dictionary<string, Dictionary<string, GridCellVM>>();

            foreach (var day in vm.Days)
            {
                vm.GridData[day] = new Dictionary<string, GridCellVM>();

                foreach (var period in vm.Periods)
                {
                    vm.GridData[day][period.PeriodName] = new GridCellVM
                    {
                        Day = day,
                        Period = period.PeriodName,
                        IsBreak = period.IsBreak,
                        IsAssembly = period.IsAssembly,
                        SubjectId = null,
                        TeacherId = null
                    };
                }
            }
        }

        private async Task LoadExistingTimetable(TimetableGridVM vm)
        {
            var existing = await _context.Timetables
                .Where(t =>
                    t.AcademicYear == vm.AcademicYear &&
                    t.DepartmentId == vm.DepartmentId &&
                    t.ClassId == vm.ClassId &&
                    t.SectionId == vm.SectionId)
                .ToListAsync();

            foreach (var entry in existing)
            {
                if (vm.GridData.ContainsKey(entry.DayName) &&
                    vm.GridData[entry.DayName].ContainsKey(entry.PeriodName))
                {
                    vm.GridData[entry.DayName][entry.PeriodName] = new GridCellVM
                    {
                        Day = entry.DayName,
                        Period = entry.PeriodName,
                        SubjectId = entry.SubjectId,
                        TeacherId = entry.TeacherId,
                        IsBreak = entry.IsBreak,
                        IsAssembly = entry.IsAssembly
                    };
                }
            }
        }

        private async Task<IActionResult> LoadGridForEditing(SaveTimetableVM vm)
        {
            var gridVm = new TimetableGridVM
            {
                AcademicYear = vm.AcademicYear,
                DepartmentId = vm.DepartmentId,
                ClassId = vm.ClassId,
                SectionId = vm.SectionId,
                Days = new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" },
                Periods = PeriodBlueprintVM.DailyPeriods
            };

            // Get names
            var department = await _context.Departments.FindAsync(vm.DepartmentId);
            var classInfo = await _context.Classes.FindAsync(vm.ClassId);
            var section = await _context.Sections.FindAsync(vm.SectionId);

            gridVm.DepartmentName = department?.DepartmentName ?? "";
            gridVm.ClassName = classInfo?.ClassName ?? "";
            gridVm.SectionName = section?.SectionName ?? "";

            await LoadFilteredData(gridVm);
            InitializeGrid(gridVm);

            // Populate with submitted data
            foreach (var cell in vm.Cells)
            {
                if (gridVm.GridData.ContainsKey(cell.Day) &&
                    gridVm.GridData[cell.Day].ContainsKey(cell.Period))
                {
                    gridVm.GridData[cell.Day][cell.Period] = cell;
                }
            }

            return View("CreateGrid", gridVm);
        }
        [HttpPost]
        public IActionResult TestSave(SaveTimetableVM vm)
        {
            // Return JSON to see what was received
            return Json(new
            {
                AcademicYear = vm.AcademicYear,
                DepartmentId = vm.DepartmentId,
                ClassId = vm.ClassId,
                SectionId = vm.SectionId,
                CellCount = vm.Cells?.Count,
                Cells = vm.Cells
            });
        }
    }

}