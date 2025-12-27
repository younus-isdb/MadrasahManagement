using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models; // ExamRoutine model
using System.Threading.Tasks;

namespace MadrasahManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsRoutineController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExamsRoutineController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET: api/ExamRoutineApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamRoutine>>> GetExamRoutines()
        {
            return await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Subject)
                .ToListAsync();
        }

        // GET: api/ExamRoutineApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamRoutine>> GetExamRoutine(int id)
        {
            var examRoutine = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.ExamRoutineId == id);

            if (examRoutine == null)
                return NotFound();

            return examRoutine;
        }

        // POST: api/ExamRoutineApi
        [HttpPost]
        public async Task<ActionResult<ExamRoutine>> PostExamRoutine(ExamRoutine examRoutine)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.ExamRoutines.Add(examRoutine);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExamRoutine), new { id = examRoutine.ExamRoutineId }, examRoutine);
        }

        // PUT: api/ExamRoutineApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamRoutine(int id, ExamRoutine examRoutine)
        {
            if (id != examRoutine.ExamRoutineId)
                return BadRequest();

            _context.Entry(examRoutine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamRoutineExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/ExamRoutineApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamRoutine(int id)
        {
            var examRoutine = await _context.ExamRoutines.FindAsync(id);
            if (examRoutine == null)
                return NotFound();

            _context.ExamRoutines.Remove(examRoutine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamRoutineExists(int id)
        {
            return _context.ExamRoutines.Any(e => e.ExamRoutineId == id);
        }
    }
}
