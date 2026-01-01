using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadrasahManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamFeeCollectionController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExamFeeCollectionController(MadrasahDbContext context)
        {
            _context = context;
        }

        // POST: api/ExamFee
        [HttpPost]
        public async Task<IActionResult> Create(ExamFeesCreateDto dto)
        {
            // Manual mapping DTO -> Entity
            var examFee = new ExamFee
            {
                EducationYear = dto.EducationYear,
                ClassId = dto.ClassId,
                ExamId = dto.ExamId,
                ExamAmount = dto.ExamAmount,
                ExamFeeCollections = dto.FeeCollections.Select(f => new ExamFeeCollection
                {
                    StudentId = f.StudentId,
                    ExamFee = f.ExamFee,
                    TotalSubject = f.TotalSubject,
                    EducationYear = f.EducationYear
                }).ToList()
            };

            _context.ExamFees.Add(examFee);
            await _context.SaveChangesAsync();

            return Ok(new { examFee.ExamFeeId });
        }

        // GET: api/ExamFee/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamFeesReadDto>> Get(int id)
        {
            var fee = await _context.ExamFees
                .Include(e => e.ExamFeeCollections)
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .FirstOrDefaultAsync(e => e.ExamFeeId == id);

            if (fee == null) return NotFound();

            // Manual mapping Entity -> Read DTO
            var result = new ExamFeesReadDto
            {
                ExamFeeId = fee.ExamFeeId,
                EducationYear = fee.EducationYear,
                ClassId = fee.ClassId,
                ClassName = fee.Class?.ClassName ?? string.Empty,
                ExamId = fee.ExamId,
                ExamName = fee.Examination?.ExamName ?? string.Empty,
                ExamAmount = fee.ExamAmount,
                FeeCollections = fee.ExamFeeCollections.Select(fc => new ExamFeeCollectionReadDto
                {
                    FeeCollectionId = fc.FeeCollectionId,
                    StudentId = fc.StudentId,
                    StudentName = fc.Student?.StudentName ?? string.Empty,
                    ExamFee = fc.ExamFee,
                    TotalSubject = fc.TotalSubject,
                    EducationYear = fc.EducationYear
                }).ToList()
            };

            return Ok(result);
        }

        // Optional: GET all
        [HttpGet]
        public async Task<ActionResult<List<ExamFeesReadDto>>> GetAll()
        {
            var fees = await _context.ExamFees
                .Include(e => e.ExamFeeCollections)
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .ToListAsync();

            var result = fees.Select(fee => new ExamFeesReadDto
            {
                ExamFeeId = fee.ExamFeeId,
                EducationYear = fee.EducationYear,
                ClassId = fee.ClassId,
                ClassName = fee.Class?.ClassName ?? string.Empty,
                ExamId = fee.ExamId,
                ExamName = fee.Examination?.ExamName ?? string.Empty,
                ExamAmount = fee.ExamAmount,
                FeeCollections = fee.ExamFeeCollections.Select(fc => new ExamFeeCollectionReadDto
                {
                    FeeCollectionId = fc.FeeCollectionId,
                    StudentId = fc.StudentId,
                    StudentName = fc.Student?.StudentName ?? string.Empty,
                    ExamFee = fc.ExamFee,
                    TotalSubject = fc.TotalSubject,
                    EducationYear = fc.EducationYear
                }).ToList()
            }).ToList();

            return Ok(result);
        }
    }
}
