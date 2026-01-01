using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
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

        // ------------------- INDEX / LIST -------------------
        public async Task<IActionResult> Index()
        {
            var examFees = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.FeeCollections)
                    .ThenInclude(fc => fc.Student)
                .Select(e => new ExamFeesReadDto
                {
                    ExamFeeId = e.ExamFeeId,
                    EducationYear = e.EducationYear,
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
                .ToListAsync();

            return View(examFees);
        }

        // ------------------- DETAILS -------------------
        public async Task<IActionResult> Details(int id)
        {
            var examFee = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.FeeCollections)
                    .ThenInclude(fc => fc.Student)
                .Where(e => e.ExamFeeId == id)
                .Select(e => new ExamFeesReadDto
                {
                    ExamFeeId = e.ExamFeeId,
                    EducationYear = e.EducationYear,
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
                .FirstOrDefaultAsync();

            if (examFee == null) return NotFound();

            return View(examFee);
        }

        // ------------------- CREATE -------------------
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Classes = _context.Classes.ToList();
            ViewBag.Exams = _context.Examinations.ToList();
            ViewBag.Students = _context.Students.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamFeesCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Classes = _context.Classes.ToList();
                ViewBag.Exams = _context.Examinations.ToList();
                ViewBag.Students = _context.Students.ToList();
                return View(dto);
            }

            // Filter out invalid students
            var validStudents = _context.Students.Select(s => s.StudentId).ToHashSet();
            var feeCollections = dto.FeeCollections
                .Where(fc => validStudents.Contains(fc.StudentId))
                .Select(fc => new ExamFeeCollection
                {
                    StudentId = fc.StudentId,
                    ExamFeeAmount = fc.ExamFeeAmount,
                    TotalSubject = fc.TotalSubject
                }).ToList();

            var examFee = new ExamFee
            {
                EducationYear = dto.EducationYear,
                ClassId = dto.ClassId,
                ExamId = dto.ExamId,
                ExamAmount = dto.ExamAmount,
                FeeCollections = feeCollections
            };

            _context.ExamFees.Add(examFee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ------------------- EDIT -------------------
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var existing = await _context.ExamFees
                .Include(e => e.FeeCollections)
                .FirstOrDefaultAsync(e => e.ExamFeeId == id);

            if (existing == null) return NotFound();

            var dto = new ExamFeesUpdateDto
            {
                EducationYear = existing.EducationYear,
                ClassId = existing.ClassId,
                ExamId = existing.ExamId,
                ExamAmount = existing.ExamAmount,
                FeeCollections = existing.FeeCollections.Select(fc => new ExamFeeCollectionUpdateDto
                {
                    FeeCollectionId = fc.FeeCollectionId,
                    StudentId = fc.StudentId,
                    ExamFeeAmount = fc.ExamFeeAmount,
                    TotalSubject = fc.TotalSubject
                }).ToList()
            };

            ViewBag.Classes = _context.Classes.ToList();
            ViewBag.Exams = _context.Examinations.ToList();
            ViewBag.Students = _context.Students.ToList();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamFeesUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Classes = _context.Classes.ToList();
                ViewBag.Exams = _context.Examinations.ToList();
                ViewBag.Students = _context.Students.ToList();
                return View(dto);
            }

            var existing = await _context.ExamFees
                .Include(e => e.FeeCollections)
                .FirstOrDefaultAsync(e => e.ExamFeeId == id);

            if (existing == null) return NotFound();

            existing.EducationYear = dto.EducationYear;
            existing.ClassId = dto.ClassId;
            existing.ExamId = dto.ExamId;
            existing.ExamAmount = dto.ExamAmount;

            // Update FeeCollections
            var existingStudentIds = existing.FeeCollections.Select(f => f.FeeCollectionId).ToHashSet();

            foreach (var fcDto in dto.FeeCollections)
            {
                if (fcDto.FeeCollectionId.HasValue && existingStudentIds.Contains(fcDto.FeeCollectionId.Value))
                {
                    // Update existing
                    var existingFc = existing.FeeCollections.First(f => f.FeeCollectionId == fcDto.FeeCollectionId.Value);
                    existingFc.StudentId = fcDto.StudentId;
                    existingFc.ExamFeeAmount = fcDto.ExamFeeAmount;
                    existingFc.TotalSubject = fcDto.TotalSubject;
                }
                else
                {
                    // Add new
                    existing.FeeCollections.Add(new ExamFeeCollection
                    {
                        StudentId = fcDto.StudentId,
                        ExamFeeAmount = fcDto.ExamFeeAmount,
                        TotalSubject = fcDto.TotalSubject
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ------------------- DELETE -------------------
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .FirstOrDefaultAsync(e => e.ExamFeeId == id);

            if (existing == null) return NotFound();
            return View(existing);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existing = await _context.ExamFees.FindAsync(id);
            if (existing == null) return NotFound();

            _context.ExamFees.Remove(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
