using MadrasahManagement.Models;
using MadrasahManagement.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MadrasahManagement.Controllers
{
    public class ResultProcessController : Controller
    {
        private readonly MadrasahDbContext _context;

        public ResultProcessController(MadrasahDbContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(int departmentId = 0, int classId = 0)
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Classes = await _context.Classes.ToListAsync();
            ViewBag.SelectedDepartmentId = departmentId;
            ViewBag.SelectedClassId = classId;

            List<ExamResult> results = new List<ExamResult>();

            // Only fetch results if both department & class are selected
            if (departmentId > 0 && classId > 0)
            {
                results = await _context.ExamResults
                    .Include(r => r.Student)
                    .Include(r => r.Class)
                    .Include(r => r.Examination)
                    .Include(r => r.ResultDetails)
                        .ThenInclude(d => d.Subject)
                    .Where(r => r.Student.DepartmentId == departmentId && r.ClassId == classId)
                    .ToListAsync();
            }

            return View(results);
        }



        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResultProcessCreateDto dto)
        {
            return await ProcessResult(dto, null);
        }

        // ================= EDIT =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _context.ExamResults
                .Include(r => r.ResultDetails)
                    .ThenInclude(d => d.Subject) // <-- include Subject here
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null) return NotFound();

            var dto = new ResultProcessUpdateDto
            {
                ResultProcessId = result.ResultId,
                EducationYear = result.EducationYear,
                StudentId = result.StudentId,
                ClassId = result.ClassId,
                DepartmentId = result.DepartmentId,
                ExamId = result.ExamId,
                Subjects = result.ResultDetails
                    .Select(d => new ResultSubjectInputDto
                    {
                        SubjectId = d.SubjectId,
                        SubjectName = d.Subject?.SubjectName ?? "", // safe null check
                        Marks = d.Marks
                    }).ToList()
            };

            PopulateDropdowns();
            return View(dto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ResultProcessUpdateDto dto)
        {
            return await ProcessResult(dto, dto.ResultProcessId);
        }

        // ================= CORE LOGIC =================
        private async Task<IActionResult> ProcessResult(ResultProcessCreateDto dto, int? existingResultId)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(dto);
            }

            ExamResult result;

            if (existingResultId.HasValue)
            {
                result = await _context.ExamResults
                    .Include(r => r.ResultDetails)
                    .FirstOrDefaultAsync(r => r.ResultId == existingResultId.Value);

                if (result == null) return NotFound();

                _context.ResultDetails.RemoveRange(result.ResultDetails);
                result.ResultDetails.Clear();
            }
            else
            {
                result = new ExamResult();
                _context.ExamResults.Add(result);
            }

            // -------- HEADER --------
            result.EducationYear = dto.EducationYear;
            result.StudentId = dto.StudentId;
            result.ClassId = dto.ClassId;
            result.DepartmentId = dto.DepartmentId;
            result.ExamId = dto.ExamId;
            result.IsPassed = true;

            decimal totalMarks = 0;
            decimal totalGpa = 0;

            // -------- SUBJECTS --------
            foreach (var sub in dto.Subjects)
            {
                var (division, gpa) = CalculateGpa(sub.Marks);

                if (division == "Fail")
                    result.IsPassed = false;

                result.ResultDetails.Add(new ResultDetail
                {
                    SubjectId = sub.SubjectId,
                    Marks = sub.Marks,
                    Division = division,
                    GPA = gpa,
                    IsSilverColor = division == "A+"
                });

                totalMarks += sub.Marks;
                totalGpa += gpa;
            }

            result.TotalMarks = totalMarks;
            result.TotalCGPA = result.IsPassed && dto.Subjects.Count > 0
                ? Math.Round(totalGpa / dto.Subjects.Count, 2)
                : 0;

            await _context.SaveChangesAsync();

            await UpdateMeritPosition(
                result.EducationYear,
                result.ClassId,
                result.ExamId,
                result.DepartmentId
            );

            return RedirectToAction(nameof(Index));
        }

        // ================= GPA CALCULATOR =================
        private (string division, decimal gpa) CalculateGpa(decimal marks)
        {
            if (marks >= 80) return ("A+", 5.00m);
            if (marks >= 70) return ("A", 4.00m);
            if (marks >= 60) return ("A-", 3.50m);
            if (marks >= 50) return ("B", 3.00m);
            if (marks >= 40) return ("C", 2.00m);
            if (marks >= 33) return ("D", 1.00m);
            return ("Fail", 0.00m);
        }

        // ================= MERIT =================
        private async Task UpdateMeritPosition(string year, int classId, int examId, int deptId)
        {
            var results = await _context.ExamResults
                .Where(r =>
                    r.EducationYear == year &&
                    r.ClassId == classId &&
                    r.ExamId == examId &&
                    r.DepartmentId == deptId &&
                    r.IsPassed)
                .OrderByDescending(r => r.TotalMarks)
                .ThenByDescending(r => r.TotalCGPA)
                .ToListAsync();

            for (int i = 0; i < results.Count; i++)
            {
                results[i].MeritPosition = i + 1;
            }

            await _context.SaveChangesAsync();
        }

        // ================= DROPDOWNS =================
        private void PopulateDropdowns()
        {
            ViewBag.Students = new SelectList(_context.Students, "StudentId", "StudentName");
            ViewBag.Classes = new SelectList(_context.Classes, "ClassId", "ClassName");
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewBag.Exams = new SelectList(_context.Examinations, "ExamId", "ExamName");

            ViewBag.SubjectList = _context.Subjects
                .Select(s => new { value = s.SubjectId, text = s.SubjectName })
                .ToList();

            ViewBag.Years = new List<SelectListItem>
            {
                new SelectListItem { Text = "2024", Value = "2024" },
                new SelectListItem { Text = "2025", Value = "2025" },
                new SelectListItem { Text = "2026", Value = "2026" }
            };
        }
        // GET: ResultProcess/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var result = await _context.ExamResults
                .Include(r => r.Student)
                .Include(r => r.Class)
                .Include(r => r.Department)
                .Include(r => r.Examination)
                .Include(r => r.ResultDetails)
                    .ThenInclude(d => d.Subject)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null) return NotFound();

            return View(result);
        }

        // POST: ResultProcess/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _context.ExamResults
                .Include(r => r.ResultDetails)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result != null)
            {
                // Remove child details first
                _context.ResultDetails.RemoveRange(result.ResultDetails);
                _context.ExamResults.Remove(result);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: ResultProcess/StudentResult/5
        public async Task<IActionResult> StudentResult(int studentId, int examId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound();

            var result = await _context.ExamResults
                .Include(r => r.ResultDetails)
                    .ThenInclude(d => d.Subject)
                .Include(r => r.Class)
                .Include(r => r.Department)
                .Include(r => r.Examination)
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.ExamId == examId);

            if (result == null) return NotFound();

            return View(result); // Pass the whole result to the view
        }

    }
}