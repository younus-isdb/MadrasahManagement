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

        // ------------------- INDEX -------------------
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
            PopulateDropdowns();
            return View(new ExamFeesCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamFeesCreateDto dto)
        {
            // Make sure FeeCollections list is not null
            if (dto.FeeCollections == null) dto.FeeCollections = new List<ExamFeeCollectionCreateDto>();

            if (!ModelState.IsValid || !dto.FeeCollections.Any())
            {
                if (!dto.FeeCollections.Any())
                    ModelState.AddModelError("", "At least one Fee Collection is required.");

                PopulateDropdowns();
                return View(dto);
            }

            // Filter valid students
            var validStudentIds = await _context.Students.Select(s => s.StudentId).ToHashSetAsync();
            var feeCollections = dto.FeeCollections
                .Where(fc => validStudentIds.Contains(fc.StudentId))
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

            PopulateDropdowns();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamFeesUpdateDto dto)
        {
            if (dto.FeeCollections == null) dto.FeeCollections = new List<ExamFeeCollectionUpdateDto>();

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
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

            // Remove FeeCollections that are no longer present
            var dtoIds = dto.FeeCollections.Where(f => f.FeeCollectionId.HasValue).Select(f => f.FeeCollectionId.Value).ToHashSet();
            var toRemove = existing.FeeCollections.Where(f => !dtoIds.Contains(f.FeeCollectionId)).ToList();
            _context.Set<ExamFeeCollection>().RemoveRange(toRemove);

            // Add or update FeeCollections
            foreach (var fcDto in dto.FeeCollections)
            {
                if (fcDto.FeeCollectionId.HasValue)
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
            var existing = await _context.ExamFees
                .Include(e => e.FeeCollections)
                .FirstOrDefaultAsync(e => e.ExamFeeId == id);

            if (existing == null) return NotFound();

            _context.ExamFeeCollections.RemoveRange(existing.FeeCollections);
            _context.ExamFees.Remove(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ------------------- HELPER -------------------
        private void PopulateDropdowns()
        {
            ViewBag.Classes = _context.Classes
                .Select(c => new { c.ClassId, c.ClassName })
                .ToList();

            ViewBag.Exams = _context.Examinations
                .Select(e => new { e.ExamId, e.ExamName })
                .ToList();

            ViewBag.Students = _context.Students
                .Select(s => new { s.StudentId, s.StudentName })
                .ToList();
        }
    }
}
