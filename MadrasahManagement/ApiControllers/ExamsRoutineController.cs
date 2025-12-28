using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamRoutineController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExamRoutineController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Subject)
                .Select(x => new ExamRoutineReadDto
                {
                    ExamRoutineId = x.ExamRoutineId,
                    EducationYear = x.EducationYear,
                    ClassId = x.ClassId,
                    ClassName = x.Class!.ClassName,
                    ExamId = x.ExamId,
                    ExamName = x.Examination!.ExamName,
                    SubjectId = x.SubjectId,
                    SubjectName = x.Subject!.SubjectName,
                    RoomNumber = x.RoomNumber,
                    ExamDate = x.ExamDate,
                    ExamDay = x.ExamDay,
                    ExamStartTime = x.ExamStartTime,
                    ExamEndTime = x.ExamEndTime
                }).ToListAsync();

            return Ok(data);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var x = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.ExamRoutineId == id);

            if (x == null) return NotFound();

            var dto = new ExamRoutineReadDto
            {
                ExamRoutineId = x.ExamRoutineId,
                EducationYear = x.EducationYear,
                ClassId = x.ClassId,
                ClassName = x.Class!.ClassName,
                ExamId = x.ExamId,
                ExamName = x.Examination!.ExamName,
                SubjectId = x.SubjectId,
                SubjectName = x.Subject!.SubjectName,
                RoomNumber = x.RoomNumber,
                ExamDate = x.ExamDate,
                ExamDay = x.ExamDay,
                ExamStartTime = x.ExamStartTime,
                ExamEndTime = x.ExamEndTime
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExamRoutineCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new ExamRoutine
            {
                EducationYear = dto.EducationYear,
                ClassId = dto.ClassId,
                ExamId = dto.ExamId,
                SubjectId = dto.SubjectId,
                RoomNumber = dto.RoomNumber,
                ExamDate = dto.ExamDate,
                ExamDay = dto.ExamDay,
                ExamStartTime = dto.ExamStartTime,
                ExamEndTime = dto.ExamEndTime
            };

            _context.ExamRoutines.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.ExamRoutineId }, entity);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExamRoutineUpdateDto dto)
        {
            if (id != dto.ExamRoutineId) return BadRequest("Id mismatch");

            var existing = await _context.ExamRoutines.FindAsync(id);
            if (existing == null) return NotFound();

            existing.EducationYear = dto.EducationYear;
            existing.ClassId = dto.ClassId;
            existing.ExamId = dto.ExamId;
            existing.SubjectId = dto.SubjectId;
            existing.RoomNumber = dto.RoomNumber;
            existing.ExamDate = dto.ExamDate;
            existing.ExamDay = dto.ExamDay;
            existing.ExamStartTime = dto.ExamStartTime;
            existing.ExamEndTime = dto.ExamEndTime;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.ExamRoutines.FindAsync(id);
            if (existing == null) return NotFound();

            _context.ExamRoutines.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
