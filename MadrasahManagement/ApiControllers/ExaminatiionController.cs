using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExaminationController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExaminationController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET: api/Examination
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exams = await _context.Examinations
                .Select(e => new ExaminationReadDto
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    ExamFeeCount = e.ExamFees.Count,
                    PointConditionCount = e.PointConditions.Count,
                    ExamRoutineCount = e.ExamRoutine.Count
                }).ToListAsync();

            return Ok(exams);
        }

        // GET: api/Examination/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _context.Examinations
                .Include(x => x.ExamFees)
                .Include(x => x.PointConditions)
                .Include(x => x.ExamRoutine)
                .FirstOrDefaultAsync(x => x.ExamId == id);

            if (e == null) return NotFound();

            var dto = new ExaminationReadDto
            {
                ExamId = e.ExamId,
                ExamName = e.ExamName,
                ExamFeeCount = e.ExamFees.Count,
                PointConditionCount = e.PointConditions.Count,
                ExamRoutineCount = e.ExamRoutine.Count
            };

            return Ok(dto);
        }

        // POST: api/Examination
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExaminationCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exam = new Examination
            {
                ExamName = dto.ExamName
            };

            _context.Examinations.Add(exam);
            await _context.SaveChangesAsync();

            var readDto = new ExaminationReadDto
            {
                ExamId = exam.ExamId,
                ExamName = exam.ExamName,
                ExamFeeCount = 0,
                PointConditionCount = 0,
                ExamRoutineCount = 0
            };

            return CreatedAtAction(nameof(Get), new { id = exam.ExamId }, readDto);
        }

        // PUT: api/Examination/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExaminationUpdateDto dto)
        {
            if (id != dto.ExamId) return BadRequest();

            var exam = await _context.Examinations.FindAsync(id);
            if (exam == null) return NotFound();

            exam.ExamName = dto.ExamName;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Examination/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Examinations.FindAsync(id);
            if (exam == null) return NotFound();

            _context.Examinations.Remove(exam);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
