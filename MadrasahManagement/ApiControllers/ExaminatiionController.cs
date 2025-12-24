using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExaminatiionController : ControllerBase
    {
        private readonly MadrasahDbContext _context;
        public ExaminatiionController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET: api/Examinatiion
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exams = await _context.Examinations.ToListAsync();
            return Ok(exams);
        }

        // GET: api/Examinatiion/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var exam = await _context.Examinations.FindAsync(id);
            if (exam == null) return NotFound();
            return Ok(exam);
        }

        // POST: api/Examinatiion
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Examination exam)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Examinations.Add(exam);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = exam.ExamId }, exam);
        }

        // PUT: api/Examinatiion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Examination exam)
        {
            if (id != exam.ExamId) return BadRequest();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Entry(exam).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Examinations.Any(e => e.ExamId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Examinatiion/5
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
