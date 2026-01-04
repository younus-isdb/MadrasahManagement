using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    public class ExamFeeCollectionsController : Controller
    {
        private readonly MadrasahDbContext _context;

        public ExamFeeCollectionsController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            var examFees = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Department)
                .Include(e => e.Examination)
                .Include(e => e.FeeCollections)
                    .ThenInclude(fc => fc.Student)
                .Select(e => new ExamFeesReadDto
                {
                    ExamFeeId = e.ExamFeeId,
                    EducationYear = e.EducationYear,
                    DepartmentId = e.DepartmentId,
                    ClassId = e.ClassId,
                    ClassName = e.Class != null ? e.Class.ClassName : "Unknown",
                    ExamId = e.ExamId,
                    ExamName = e.Examination != null ? e.Examination.ExamName : "Unknown",
                    ExamAmount = e.ExamAmount,
                    FeeCollections = e.FeeCollections.Select(fc => new ExamFeeCollectionReadDto
                    {
                        FeeCollectionId = fc.FeeCollectionId,
                        StudentId = fc.StudentId,
                        StudentName = fc.Student != null ? fc.Student.StudentName : "Unknown",
                        ExamFeeAmount = fc.ExamFeeAmount,
                        TotalSubject = fc.TotalSubject
                    }).ToList()
                })
                .OrderByDescending(e => e.ExamFeeId)
                .ToListAsync();

            return View(examFees);
        }

        // GET: Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new ExamFeesCreateDto());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamFeesCreateDto dto)
        {
            // Always repopulate dropdowns with submitted values
            PopulateDropdownsWithValues(dto);

            if (dto.FeeCollections == null)
                dto.FeeCollections = new List<ExamFeeCollectionCreateDto>();

            // Validate at least one student
            if (!dto.FeeCollections.Any())
            {
                ModelState.AddModelError("", "At least one student fee collection is required.");
                return View(dto);
            }

            // Check for invalid student selections
            bool hasInvalidStudent = false;
            foreach (var fc in dto.FeeCollections)
            {
                if (fc.StudentId <= 0)
                {
                    hasInvalidStudent = true;
                    break;
                }
            }

            if (hasInvalidStudent)
            {
                ModelState.AddModelError("", "Please select a valid student for all rows.");
                return View(dto);
            }

            if (ModelState.IsValid)
            {
                // Check for duplicate students in this submission
                var studentIds = dto.FeeCollections.Select(fc => fc.StudentId).ToList();
                if (studentIds.Distinct().Count() != studentIds.Count)
                {
                    ModelState.AddModelError("", "Duplicate students are not allowed in the same collection.");
                    return View(dto);
                }

                // Check if students already have fee collections for this exam
                var existingCollections = await _context.ExamFeeCollections
                    .Include(ec => ec.ExamFee)
                    .Where(ec => ec.ExamFee.ClassId == dto.ClassId
                        && ec.ExamFee.ExamId == dto.ExamId
                        && ec.ExamFee.EducationYear == dto.EducationYear
                        && studentIds.Contains(ec.StudentId))
                    .Select(ec => ec.StudentId)
                    .ToListAsync();

                if (existingCollections.Any())
                {
                    ModelState.AddModelError("", $"Some students already have fee collections for this exam: {string.Join(", ", existingCollections)}");
                    return View(dto);
                }

                // Find or create ExamFee master record
                var examFee = await _context.ExamFees
                    .FirstOrDefaultAsync(e => e.EducationYear == dto.EducationYear
                        && e.ClassId == dto.ClassId
                        && e.ExamId == dto.ExamId);

                if (examFee == null)
                {
                    // Create new ExamFee master record
                    examFee = new ExamFee
                    {
                        EducationYear = dto.EducationYear,
                        DepartmentId = dto.DepartmentId,
                        ClassId = dto.ClassId,
                        ExamId = dto.ExamId,
                        ExamAmount = dto.ExamAmount
                    };
                    _context.ExamFees.Add(examFee);
                    await _context.SaveChangesAsync(); // Save to get ExamFeeId
                }
                else
                {
                    // Update existing ExamFee amount if different
                    if (examFee.ExamAmount != dto.ExamAmount)
                    {
                        examFee.ExamAmount = dto.ExamAmount;
                        _context.Update(examFee);
                        await _context.SaveChangesAsync();
                    }
                }

                // Add fee collections
                foreach (var fcDto in dto.FeeCollections)
                {
                    var collection = new ExamFeeCollection
                    {
                        ExamFeeId = examFee.ExamFeeId, // Link to the ExamFee
                        StudentId = fcDto.StudentId,
                        ExamFeeAmount = fcDto.ExamFeeAmount,
                        TotalSubject = fcDto.TotalSubject
                    };
                    _context.ExamFeeCollections.Add(collection);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Exam fee collections added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Return with validation errors
            return View(dto);
        }

        // AJAX: Get classes by department
        [HttpGet]
        public async Task<IActionResult> GetClassesByDepartment(int departmentId)
        {
            var classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId)
                .Select(c => new
                {
                    c.ClassId,
                    c.ClassName
                })
                .OrderBy(c => c.ClassName)
                .ToListAsync();

            return Json(classes);
        }

        // AJAX: Get students by class
        [HttpGet]
        public async Task<IActionResult> GetStudentsByClass(int classId)
        {
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .Select(s => new
                {
                    s.StudentId,
                    s.StudentName
                })
                .OrderBy(s => s.StudentName)
                .ToListAsync();

            return Json(students);
        }

        // AJAX: Get exam fee by class and exam
        [HttpGet]
        public async Task<IActionResult> GetExamFeeByClassAndExam(int classId, int examId, string educationYear)
        {
            var examFee = await _context.ExamFees
                .FirstOrDefaultAsync(e => e.ClassId == classId
                    && e.ExamId == examId
                    && e.EducationYear == educationYear);

            if (examFee != null)
            {
                return Json(new
                {
                    success = true,
                    amount = examFee.ExamAmount,
                    examFeeId = examFee.ExamFeeId
                });
            }

            return Json(new
            {
                success = false,
                message = "No exam fee setup found. You can create one now."
            });
        }

        // Helper method to populate dropdowns
        private void PopulateDropdowns()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewBag.Classes = new SelectList(_context.Classes, "ClassId", "ClassName");
            ViewBag.Exams = new SelectList(_context.Examinations, "ExamId", "ExamName");

            // Get all students for initial load
            ViewBag.AllStudents = _context.Students
                .Select(s => new { s.StudentId, s.StudentName })
                .ToList();
        }

        // Helper method to populate dropdowns with submitted values
        private void PopulateDropdownsWithValues(ExamFeesCreateDto dto)
        {
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", dto.DepartmentId);
            ViewBag.Classes = new SelectList(_context.Classes, "ClassId", "ClassName", dto.ClassId);
            ViewBag.Exams = new SelectList(_context.Examinations, "ExamId", "ExamName", dto.ExamId);

            // Get all students for initial load
            ViewBag.AllStudents = _context.Students
                .Select(s => new { s.StudentId, s.StudentName })
                .ToList();
        }
    }
}