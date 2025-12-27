using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamFeeCollectionController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExamFeeCollectionController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.ExamFeeCollections
                .Include(x => x.Examination)
                .Include(x => x.Class)
                .Include(x => x.Student)
                .Select(x => new ExamFeeCollectionReadDto
                {
                    FeeCollectionId = x.FeeCollectionId,
                    EducationYear = x.EducationYear,
                    ExamId = x.ExamId,
                    ExamName = x.Examination!.ExamName,
                    ClassId = x.ClassId,
                    ClassName = x.Class!.ClassName,
                    TotalSubject = x.TotalSubject,
                    ExamFee = x.ExamFee,
                    StudentId = x.StudentId,
                    StudentName = x.Student!.StudentName
                }).ToListAsync();

            return Ok(data);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var x = await _context.ExamFeeCollections
                .Include(f => f.Examination)
                .Include(f => f.Class)
                .Include(f => f.Student)
                .FirstOrDefaultAsync(f => f.FeeCollectionId == id);

            if (x == null) return NotFound();

            var dto = new ExamFeeCollectionReadDto
            {
                FeeCollectionId = x.FeeCollectionId,
                EducationYear = x.EducationYear,
                ExamId = x.ExamId,
                ExamName = x.Examination!.ExamName,
                ClassId = x.ClassId,
                ClassName = x.Class!.ClassName,
                TotalSubject = x.TotalSubject,
                ExamFee = x.ExamFee,
                StudentId = x.StudentId,
                StudentName = x.Student!.StudentName
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExamFeeCollectionCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new ExamFeeCollection
            {
                EducationYear = dto.EducationYear,
                ExamId = dto.ExamId,
                ClassId = dto.ClassId,
                TotalSubject = dto.TotalSubject,
                ExamFee = dto.ExamFee,
                StudentId = dto.StudentId
            };

            _context.ExamFeeCollections.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.FeeCollectionId }, entity);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExamFeeCollectionUpdateDto dto)
        {
            if (id != dto.FeeCollectionId) return BadRequest("Id mismatch");

            var existing = await _context.ExamFeeCollections.FindAsync(id);
            if (existing == null) return NotFound();

            existing.EducationYear = dto.EducationYear;
            existing.ExamId = dto.ExamId;
            existing.ClassId = dto.ClassId;
            existing.TotalSubject = dto.TotalSubject;
            existing.ExamFee = dto.ExamFee;
            existing.StudentId = dto.StudentId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.ExamFeeCollections.FindAsync(id);
            if (existing == null) return NotFound();

            _context.ExamFeeCollections.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
