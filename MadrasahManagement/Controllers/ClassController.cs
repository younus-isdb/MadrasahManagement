using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin,SuperAdmin")]
public class ClassController : Controller
{
    private readonly MadrasahDbContext _context;

    public ClassController(MadrasahDbContext context)
    {
        _context = context;
    }

    // ===========================
    //  MODAL CREATE (AJAX)
    // ===========================

    // GET: /Class/CreateModal
    [HttpGet]
    public IActionResult CreateModal()
    {
        ViewBag.DepartmentList = _context.Departments.ToList();
        return PartialView("_CreateClassModal", new Class());
    }

    // POST: /Class/CreateModalPost (AJAX)
    [HttpPost]
    public async Task<IActionResult> CreateModalPost(Class model)
    {
        if (ModelState.IsValid)
        {
            _context.Classes.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        return Json(new { success = false });
    }


    [HttpGet]
    public IActionResult GetAll()
    {
        var data = _context.Classes
            .Select(c => new { c.ClassId, c.ClassName })
            .ToList();

        return Json(data);
    }

    // ===========================
    //  INDEX
    // ===========================
    public async Task<IActionResult> Index(int? departmentId)
    {
        var query = _context.Classes
            .Include(c => c.Department)
            .Include(c => c.Subjects) // ADD THIS
            .AsQueryable();

        // Apply department filter if provided
        if (departmentId.HasValue)
        {
            query = query.Where(c => c.DepartmentId == departmentId.Value);
            ViewBag.SelectedDepartmentId = departmentId.Value;
        }

        var classes = await query.ToListAsync();

        // Also add departments to ViewBag for filter dropdown
        ViewBag.Departments = await _context.Departments.ToListAsync();

        return View(classes);
    }

    // ===========================
    //  CREATE CLASS WITH SUBJECTS (MASTER-DETAILS)
    // ===========================
    public IActionResult Create()
    {
        ViewBag.DepartmentList = _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName
            })
            .ToList();

        var model = new ClassCreateViewModel
        {
            Subjects = new List<Subject> { new Subject() }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassCreateViewModel model)
    {
        // TEMPORARY: Clear ModelState to check what's happening
        ModelState.Clear(); // Add this line to bypass validation temporarily

        if (ModelState.IsValid)
        {
            try
            {
                // 1. Create the Class
                var newClass = new Class
                {
                    ClassName = model.ClassName,
                    SessionYear = model.SessionYear,
                    DepartmentId = model.DepartmentId
                };

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();

                // 2. Create Subjects for this class
                if (model.Subjects != null && model.Subjects.Any())
                {
                    foreach (var subject in model.Subjects)
                    {
                        // Skip empty rows
                        if (string.IsNullOrWhiteSpace(subject.SubjectName) ||
                            string.IsNullOrWhiteSpace(subject.SubjectCode))
                            continue;

                        // Check if subject code is unique
                        var existingCode = await _context.Subjects
                            .AnyAsync(s => s.SubjectCode == subject.SubjectCode);

                        if (existingCode)
                        {
                            TempData["ErrorMessage"] = $"Subject Code '{subject.SubjectCode}' already exists!";
                            ViewBag.DepartmentList = _context.Departments
                                .Select(d => new SelectListItem
                                {
                                    Value = d.DepartmentId.ToString(),
                                    Text = d.DepartmentName
                                })
                                .ToList();
                            return View(model);
                        }

                        // Create new subject - FIXED: Set ClassId and DepartmentId
                        var newSubject = new Subject
                        {
                            SubjectName = subject.SubjectName,
                            SubjectCode = subject.SubjectCode,
                            IsOptional = subject.IsOptional,
                            ClassId = newClass.ClassId, // This is set
                            DepartmentId = newClass.DepartmentId // This is set
                        };

                        _context.Subjects.Add(newSubject);
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Class '{model.ClassName}' with subjects created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving data: {ex.Message}");
            }
        }

        ViewBag.DepartmentList = _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName
            })
            .ToList();

        return View(model);
    }

    // ===========================
    //  EDIT CLASS & SUBJECTS (UPDATED LIKE CREATE)
    // ===========================
    public async Task<IActionResult> Edit(int id)
    {
        var classObj = await _context.Classes
            .Include(c => c.Department)
            .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.ClassId == id);

        if (classObj == null)
        {
            return NotFound();
        }

        ViewBag.DepartmentList = _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName
            })
            .ToList();

        // Create edit view model
        var model = new ClassEditViewModel
        {
            ClassId = classObj.ClassId,
            ClassName = classObj.ClassName,
            SessionYear = classObj.SessionYear,
            DepartmentId = classObj.DepartmentId,
            Subjects = classObj.Subjects.ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ClassEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // 1. Update the Class
                var classObj = await _context.Classes.FindAsync(model.ClassId);
                if (classObj == null)
                {
                    return NotFound();
                }

                classObj.ClassName = model.ClassName;
                classObj.SessionYear = model.SessionYear;
                classObj.DepartmentId = model.DepartmentId;

                _context.Classes.Update(classObj);

                // 2. Handle subjects
                // Remove existing subjects
                var existingSubjects = await _context.Subjects
                    .Where(s => s.ClassId == model.ClassId)
                    .ToListAsync();

                _context.Subjects.RemoveRange(existingSubjects);

                // Add updated subjects
                if (model.Subjects != null && model.Subjects.Any())
                {
                    foreach (var subject in model.Subjects)
                    {
                        if (string.IsNullOrWhiteSpace(subject.SubjectName) ||
                            string.IsNullOrWhiteSpace(subject.SubjectCode))
                            continue;

                        // Check if subject code is unique (excluding current class subjects)
                        var existingCode = await _context.Subjects
                            .AnyAsync(s => s.SubjectCode == subject.SubjectCode && s.ClassId != model.ClassId);

                        if (existingCode)
                        {
                            TempData["ErrorMessage"] = $"Subject Code '{subject.SubjectCode}' already exists in another class!";
                            ViewBag.DepartmentList = _context.Departments.ToList();
                            return View(model);
                        }

                        var newSubject = new Subject
                        {
                            SubjectName = subject.SubjectName,
                            SubjectCode = subject.SubjectCode,
                            IsOptional = subject.IsOptional,
                            ClassId = model.ClassId,
                            DepartmentId = model.DepartmentId
                        };

                        _context.Subjects.Add(newSubject);
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Class '{model.ClassName}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating data: {ex.Message}");
            }
        }

        ViewBag.DepartmentList = _context.Departments.ToList();
        return View(model);
    }

    // ===========================
    //  DETAILS
    // ===========================
    public async Task<IActionResult> Details(int id)
    {
        var data = await _context.Classes
            .Include(c => c.Department)
            .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.ClassId == id);

        if (data == null) return NotFound();

        return View(data);
    }

    // ===========================
    //  ADD MORE SUBJECTS TO EXISTING CLASS
    // ===========================
    public async Task<IActionResult> AddSubjects(int id)
    {
        var classObj = await _context.Classes
            .Include(c => c.Department)
            .FirstOrDefaultAsync(c => c.ClassId == id);

        if (classObj == null)
        {
            return NotFound();
        }

        var model = new AddSubjectsViewModel
        {
            ClassId = classObj.ClassId,
            ClassName = classObj.ClassName,
            DepartmentName = classObj.Department?.DepartmentName,
            Subjects = new List<Subject> { new Subject() }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSubjects(AddSubjectsViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var classObj = await _context.Classes.FindAsync(model.ClassId);
                if (classObj == null)
                {
                    return NotFound();
                }

                if (model.Subjects != null)
                {
                    foreach (var subject in model.Subjects)
                    {
                        if (string.IsNullOrWhiteSpace(subject.SubjectName) ||
                            string.IsNullOrWhiteSpace(subject.SubjectCode))
                            continue;

                        // Check if subject already exists in this class
                        var existsInClass = await _context.Subjects
                            .AnyAsync(s => s.ClassId == model.ClassId &&
                                         s.SubjectCode == subject.SubjectCode);

                        if (existsInClass)
                        {
                            TempData["ErrorMessage"] = $"Subject '{subject.SubjectName}' already exists in this class!";
                            return View(model);
                        }

                        // Check if subject code is unique globally
                        var existingCode = await _context.Subjects
                            .AnyAsync(s => s.SubjectCode == subject.SubjectCode);

                        if (existingCode)
                        {
                            TempData["ErrorMessage"] = $"Subject Code '{subject.SubjectCode}' already exists!";
                            return View(model);
                        }

                        var newSubject = new Subject
                        {
                            SubjectName = subject.SubjectName,
                            SubjectCode = subject.SubjectCode,
                            IsOptional = subject.IsOptional,
                            ClassId = model.ClassId,
                            DepartmentId = classObj.DepartmentId
                        };

                        _context.Subjects.Add(newSubject);
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Subjects added successfully!";
                return RedirectToAction(nameof(Details), new { id = model.ClassId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }
        }

        return View(model);
    }

    // ===========================
    //  VIEW CLASS WITH ITS SUBJECTS
    // ===========================
    public async Task<IActionResult> ClassSubjects(int id)
    {
        var classObj = await _context.Classes
            .Include(c => c.Department)
            .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.ClassId == id);

        if (classObj == null)
        {
            return NotFound();
        }

        return View(classObj);
    }

    // ===========================
    //  DELETE
    // ===========================
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.Classes
            .Include(c => c.Department)
            .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.ClassId == id);

        if (data == null) return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.Classes
            .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.ClassId == id);

        if (data != null)
        {
            // Delete subjects first
            if (data.Subjects.Any())
            {
                _context.Subjects.RemoveRange(data.Subjects);
            }

            _context.Classes.Remove(data);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // ===========================
    //  DELETE SUBJECT FROM CLASS
    // ===========================
    [HttpPost]
    public async Task<IActionResult> DeleteSubject(int classId, int subjectId)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == subjectId && s.ClassId == classId);

        if (subject == null)
        {
            return Json(new { success = false, message = "Subject not found!" });
        }

        try
        {
            var hasClassSubjects = await _context.ClassSubjects.AnyAsync(cs => cs.SubjectId == subjectId);
            var hasPointConditions = await _context.PointConditions.AnyAsync(pc => pc.SubjectId == subjectId);

            if (hasClassSubjects || hasPointConditions)
            {
                return Json(new
                {
                    success = false,
                    message = "Cannot delete subject because it has related records!"
                });
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Subject deleted successfully!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // GET: /Class/AssignTeachers/{id}
    public async Task<IActionResult> AssignTeachers(int id)
    {
        var classObj = await _context.Classes
            .Include(c => c.Department)
            .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.ClassId == id);

        if (classObj == null)
        {
            return NotFound();
        }

        // Get ALL teachers (no department filter)
        var teachers = await _context.Teachers
            .Include(t => t.Department)
            .Select(t => new TeacherOptionVM
            {
                TeacherId = t.TeacherId,
                Name = t.Name,
                Designation = t.Designation,
                Qualification = t.Qualification,
                DepartmentName = t.Department.DepartmentName,
                WorkloadCount = _context.ClassSubjects.Count(cs => cs.TeacherId == t.TeacherId)
            })
            .OrderBy(t => t.DepartmentName)
            .ThenBy(t => t.Name)
            .ToListAsync();

        // Get current assignments from ClassSubject table
        var currentAssignments = await _context.ClassSubjects
            .Where(cs => cs.ClassId == id)
            .Include(cs => cs.Teacher)
            .ToListAsync();

        var viewModel = new AssignTeachersViewModel
        {
            ClassId = classObj.ClassId,
            ClassName = classObj.ClassName,
            DepartmentName = classObj.Department?.DepartmentName ?? "N/A",
            Subjects = classObj.Subjects.Select(s => new SubjectAssignmentVM
            {
                SubjectId = s.SubjectId,
                SubjectName = s.SubjectName,
                SubjectCode = s.SubjectCode,
                IsOptional = s.IsOptional,
                CurrentTeacherId = currentAssignments
                    .FirstOrDefault(ca => ca.SubjectId == s.SubjectId)?.TeacherId,
                CurrentTeacherName = currentAssignments
                    .FirstOrDefault(ca => ca.SubjectId == s.SubjectId)?.Teacher?.Name,
                AvailableTeachers = teachers
            }).OrderBy(s => s.SubjectName).ToList()
        };

        return View(viewModel);
    }
    // POST: /Class/AssignTeachers
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignTeachers(AssignTeachersViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Remove existing assignments for this class
            var existingAssignments = await _context.ClassSubjects
                .Where(cs => cs.ClassId == model.ClassId)
                .ToListAsync();

            if (existingAssignments.Any())
            {
                _context.ClassSubjects.RemoveRange(existingAssignments);
                await _context.SaveChangesAsync();
            }

            // Add new assignments (only for subjects with selected teachers)
            foreach (var subject in model.Subjects)
            {
                if (subject.CurrentTeacherId.HasValue && subject.CurrentTeacherId.Value > 0)
                {
                    var classSubject = new ClassSubject
                    {
                        ClassId = model.ClassId,
                        SubjectId = subject.SubjectId,
                        TeacherId = subject.CurrentTeacherId.Value
                    };

                    _context.ClassSubjects.Add(classSubject);
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Teachers assigned successfully!";
            return RedirectToAction(nameof(ClassSubjects), new { id = model.ClassId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error assigning teachers: {ex.Message}";
            return RedirectToAction(nameof(AssignTeachers), new { id = model.ClassId });
        }
    }

    // AJAX: Get teachers by department
    [HttpGet]
    public async Task<JsonResult> GetTeachersByDepartment(int departmentId)
    {
        var teachers = await _context.Teachers
            .Where(t => t.DepartmentId == departmentId)
            .Select(t => new
            {
                t.TeacherId,
                t.Name,
                t.Designation,
                t.Qualification,
                AssignedSubjects = _context.ClassSubjects.Count(cs => cs.TeacherId == t.TeacherId)
            })
            .OrderBy(t => t.Name)
            .ToListAsync();

        return Json(teachers);
    }
}