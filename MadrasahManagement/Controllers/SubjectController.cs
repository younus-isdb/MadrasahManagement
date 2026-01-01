using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{

    public class SubjectController : Controller
    {
        private readonly MadrasahDbContext _context;

        public SubjectController(MadrasahDbContext context)
        {
            _context = context;
        }

        //Get: Subject
        public async Task<IActionResult> Index(int? departmentId, int? classId, string search = "")
        {
            // Get all departments for dropdown
            ViewBag.Departments = await _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .OrderBy(d => d.Text)
                .ToListAsync();

            // Get all classes for initial dropdown (when no department is selected)
            ViewBag.AllClasses = await _context.Classes
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName
                })
                .OrderBy(c => c.Text)
                .ToListAsync();

            // Store selected values in ViewBag
            ViewBag.SelectedDepartmentId = departmentId;
            ViewBag.SelectedClassId = classId;

            // Initialize query
            var query = _context.Subjects
                .Include(s => s.Class)
                .Include(s => s.Department)
                .AsQueryable();

            // Apply department filter if selected
            if (departmentId.HasValue)
            {
                // Get classes only for the selected department
                ViewBag.Classes = await _context.Classes
                    .Where(c => c.DepartmentId == departmentId.Value)
                    .Select(c => new SelectListItem
                    {
                        Value = c.ClassId.ToString(),
                        Text = c.ClassName
                    })
                    .OrderBy(c => c.Text)
                    .ToListAsync();

                query = query.Where(s => s.DepartmentId == departmentId.Value);
            }

            // Apply class filter if selected
            if (classId.HasValue)
            {
                query = query.Where(s => s.ClassId == classId.Value);
            }

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.SubjectName.Contains(search) ||
                                        s.SubjectCode.Contains(search));
                ViewBag.SearchTerm = search;
            }

            // Get filtered subjects
            var subjects = await query
                .Select(s => new SubjectVM
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    SubjectCode = s.SubjectCode,
                    ClassName = s.Class.ClassName,
                    DepartmentName = s.Department.DepartmentName
                })
                .OrderBy(s => s.DepartmentName)
                .ThenBy(s => s.ClassName)
                .ThenBy(s => s.SubjectName)
                .ToListAsync();

            return View(subjects);
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Class)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.SubjectId == id);

            if (subject == null)
            {
                TempData["ErrorMessage"] = "Subject not found!";
                return RedirectToAction(nameof(Index));
            }

            // Get assigned teachers
            var assignedTeachers = await _context.ClassSubjects
                .Where(cs => cs.SubjectId == id)
                .Include(cs => cs.Teacher)
                .Include(cs => cs.Class)
                .Select(cs => new AssignedTeacherDto
                {
                    TeacherId = cs.TeacherId,
                    TeacherName = cs.Teacher.Name,

                    ClassName = cs.Class.ClassName
                })
                .ToListAsync();

            // Get student count by class
            var studentCounts = await _context.Classes
                .Where(c => c.ClassId == subject.ClassId)
                .Select(c => new ClassStudentCountDto
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    StudentCount = c.Students.Count
                })
                .ToListAsync();

            var viewModel = new SubjectDetailsViewModel
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                SubjectCode = subject.SubjectCode,
                ClassName = subject.Class.ClassName,
                DepartmentName = subject.Department.DepartmentName,
                IsOptional = subject.IsOptional,
                AssignedTeachers = assignedTeachers,
                StudentCountByClass = studentCounts,
                TotalAssignedTeachers = assignedTeachers.Count,
                TotalStudents = studentCounts.Sum(sc => sc.StudentCount)
            };

            return View(viewModel);
        }

        // GET: Subject/Create
        public async Task<IActionResult> Create(int? departmentId)
        {
            // Get all departments
            ViewBag.Departments = await _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .OrderBy(d => d.Text)
                .ToListAsync();

            // If department is selected
            if (departmentId.HasValue)
            {
                await ReloadCreateViewData(departmentId.Value);

                // Initialize model for the form
                var model = new SubjectCreateDto
                {
                    DepartmentId = departmentId.Value
                };
                return View(model);
            }

            // No department selected yet - return empty model
            return View(new SubjectCreateDto());
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if subject code already exists
                    var existingCode = await _context.Subjects
                        .AnyAsync(s => s.SubjectCode == dto.SubjectCode);

                    if (existingCode)
                    {
                        ModelState.AddModelError("SubjectCode", "Subject Code already exists.");

                        // Reload the form with selected department
                        await ReloadCreateViewData(dto.DepartmentId);
                        return View(dto);
                    }

                    // Check if class belongs to selected department
                    var classBelongsToDept = await _context.Classes
                        .AnyAsync(c => c.ClassId == dto.ClassId && c.DepartmentId == dto.DepartmentId);

                    if (!classBelongsToDept)
                    {
                        ModelState.AddModelError("ClassId", "Selected class does not belong to the chosen department.");

                        await ReloadCreateViewData(dto.DepartmentId);
                        return View(dto);
                    }

                    // Check if subject with same name already exists in same class
                    var duplicateSubject = await _context.Subjects
                        .AnyAsync(s => s.SubjectName == dto.SubjectName && s.ClassId == dto.ClassId);

                    if (duplicateSubject)
                    {
                        ModelState.AddModelError("SubjectName",
                            $"A subject named '{dto.SubjectName}' already exists in this class.");

                        await ReloadCreateViewData(dto.DepartmentId);
                        return View(dto);
                    }

                    // Create new Subject entity
                    var subject = new Subject
                    {
                        SubjectName = dto.SubjectName,
                        SubjectCode = dto.SubjectCode,
                        ClassId = dto.ClassId,
                        DepartmentId = dto.DepartmentId,
                        IsOptional = dto.IsOptional
                    };

                    // Save to database
                    _context.Subjects.Add(subject);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Subject '{subject.SubjectName}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the error
                    Console.WriteLine($"Error creating subject: {ex.Message}");

                    ModelState.AddModelError("", "An error occurred while saving the subject. Please try again.");

                    await ReloadCreateViewData(dto.DepartmentId);
                    return View(dto);
                }
            }

            // If validation fails, reload the form with selected department
            if (dto.DepartmentId > 0)
            {
                await ReloadCreateViewData(dto.DepartmentId);
            }
            else
            {
                // Reload all departments
                ViewBag.Departments = await _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.DepartmentName
                    })
                    .OrderBy(d => d.Text)
                    .ToListAsync();
            }

            return View(dto);
        }

        // Helper method to reload ViewData for Create view
        private async Task ReloadCreateViewData(int departmentId)
        {
            ViewBag.SelectedDepartmentId = departmentId;
            ViewBag.SelectedDepartmentName = await _context.Departments
                .Where(d => d.DepartmentId == departmentId)
                .Select(d => d.DepartmentName)
                .FirstOrDefaultAsync();

            ViewBag.Classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId)
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName
                })
                .OrderBy(c => c.Text)
                .ToListAsync();
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Class)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.SubjectId == id);

            if (subject == null)
            {
                TempData["ErrorMessage"] = "Subject not found!";
                return RedirectToAction(nameof(Index));
            }

            // Store the original class and department for display
            ViewBag.ClassName = subject.Class?.ClassName;
            ViewBag.DepartmentName = subject.Department?.DepartmentName;
            ViewBag.ClassId = subject.ClassId;
            ViewBag.DepartmentId = subject.DepartmentId;

            var dto = new SubjectUpdateDto
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                SubjectCode = subject.SubjectCode,
                ClassId = subject.ClassId,
                DepartmentId = subject.DepartmentId,
                IsOptional = subject.IsOptional
            };

            return View(dto);
        }

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubjectUpdateDto dto)
        {
            if (id != dto.SubjectId)
            {
                TempData["ErrorMessage"] = "Subject ID mismatch!";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                // Reload display values if validation fails
                var subject = await _context.Subjects
                    .Include(s => s.Class)
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.SubjectId == id);

                if (subject != null)
                {
                    ViewBag.ClassName = subject.Class?.ClassName;
                    ViewBag.DepartmentName = subject.Department?.DepartmentName;
                    ViewBag.ClassId = subject.ClassId;
                    ViewBag.DepartmentId = subject.DepartmentId;
                }

                return View(dto);
            }

            try
            {
                var subject = await _context.Subjects.FindAsync(id);
                if (subject == null)
                {
                    TempData["ErrorMessage"] = "Subject not found!";
                    return RedirectToAction(nameof(Index));
                }

                // Check if subject code already exists (excluding current subject)
                var existingCode = await _context.Subjects
                    .AnyAsync(s => s.SubjectCode == dto.SubjectCode && s.SubjectId != id);

                if (existingCode)
                {
                    ModelState.AddModelError("SubjectCode", "Subject Code already exists.");

                    // Reload display values
                    var subjectWithDetails = await _context.Subjects
                        .Include(s => s.Class)
                        .Include(s => s.Department)
                        .FirstOrDefaultAsync(s => s.SubjectId == id);

                    if (subjectWithDetails != null)
                    {
                        ViewBag.ClassName = subjectWithDetails.Class?.ClassName;
                        ViewBag.DepartmentName = subjectWithDetails.Department?.DepartmentName;
                        ViewBag.ClassId = subjectWithDetails.ClassId;
                        ViewBag.DepartmentId = subjectWithDetails.DepartmentId;
                    }

                    return View(dto);
                }

                // Only update subject name and code (not class or department)
                subject.SubjectName = dto.SubjectName;
                subject.SubjectCode = dto.SubjectCode;
                subject.IsOptional = dto.IsOptional;
                // Note: ClassId and DepartmentId are NOT updated

                _context.Subjects.Update(subject);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Subject updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(id))
                {
                    TempData["ErrorMessage"] = "Subject not found!";
                    return RedirectToAction(nameof(Index));
                }
                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");

                // Reload display values
                var subject = await _context.Subjects
                    .Include(s => s.Class)
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.SubjectId == id);

                if (subject != null)
                {
                    ViewBag.ClassName = subject.Class?.ClassName;
                    ViewBag.DepartmentName = subject.Department?.DepartmentName;
                    ViewBag.ClassId = subject.ClassId;
                    ViewBag.DepartmentId = subject.DepartmentId;
                }

                return View(dto);
            }
        }

        // GET: Subject/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Class)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.SubjectId == id);

            if (subject == null)
            {
                TempData["ErrorMessage"] = "Subject not found!";
                return RedirectToAction(nameof(Index));
            }

            // Check if subject has dependencies
            var hasDependencies = await CheckSubjectDependencies(id);
            ViewBag.HasDependencies = hasDependencies;
            ViewBag.DependencyMessage = hasDependencies ?
                "This subject cannot be deleted because it has associated records." :
                "Are you sure you want to delete this subject?";

            return View(subject);
        }

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                TempData["ErrorMessage"] = "Subject not found!";
                return RedirectToAction(nameof(Index));
            }

            // Check dependencies before deleting
            var hasDependencies = await CheckSubjectDependencies(id);
            if (hasDependencies)
            {
                TempData["ErrorMessage"] = "Cannot delete subject because it has associated records.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Subject deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // AJAX: Check if subject code is unique
        [AcceptVerbs("GET", "POST")]
        public async Task<JsonResult> IsSubjectCodeUnique(string subjectCode)
        {
            var exists = await _context.Subjects.AnyAsync(s => s.SubjectCode == subjectCode);
            return Json(!exists);
        }

        // AJAX: Check if subject code is unique for edit
        [AcceptVerbs("GET", "POST")]
        public async Task<JsonResult> IsSubjectCodeUniqueEdit(string subjectCode, int subjectId)
        {
            var exists = await _context.Subjects.AnyAsync(s => s.SubjectCode == subjectCode && s.SubjectId != subjectId);
            return Json(!exists);
        }

        // AJAX: Get subjects by class
        [HttpGet]
        public async Task<JsonResult> GetSubjectsByClass(int classId)
        {
            var subjects = await _context.Subjects
                .Where(s => s.ClassId == classId)
                .Select(s => new
                {
                    s.SubjectId,
                    s.SubjectName,
                    s.SubjectCode,
                    s.IsOptional
                })
                .OrderBy(s => s.SubjectName)
                .ToListAsync();

            return Json(subjects);
        }

        // AJAX: Get departments by class
        [HttpGet]
        public async Task<JsonResult> GetDepartmentsByClass(int classId)
        {
            var classEntity = await _context.Classes
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (classEntity?.Department == null)
                return Json(new { });

            return Json(new
            {
                DepartmentId = classEntity.Department.DepartmentId,
                DepartmentName = classEntity.Department.DepartmentName
            });
        }

        private async Task ReloadDropdowns()
        {
            ViewBag.Classes = await _context.Classes
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName
                })
                .ToListAsync();

            ViewBag.Departments = await _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .ToListAsync();
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.SubjectId == id);
        }

        private async Task<bool> CheckSubjectDependencies(int subjectId)
        {
            var hasClassSubjects = await _context.ClassSubjects.AnyAsync(cs => cs.SubjectId == subjectId);
            var hasClassSubjectRelations = await _context.ClassSubjects.AnyAsync(cs => cs.SubjectId == subjectId);
            var hasPointConditions = await _context.PointConditions.AnyAsync(pc => pc.SubjectId == subjectId);
            //var hasExamFeeCollections = await _context.ExamFeeCollections.AnyAsync(efc => efc.SubjectId == subjectId);
            var hasExamRoutines = await _context.ExamRoutines.AnyAsync(er => er.SubjectId == subjectId);

            return hasClassSubjects || hasClassSubjectRelations || hasPointConditions
                    //  || hasExamFeeCollections
                    || hasExamRoutines;
        }
    }
}
