using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamFeeController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExamFeeController(MadrasahDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: api/ExamFee
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.ExamFees
                .Include(f => f.Class)
                .Include(f => f.Examination)
                .ToListAsync();

            return Ok(data);
        }

        // 🔹 GET: api/ExamFee/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var examFee = await _context.ExamFees
                .Include(f => f.Class)
                .Include(f => f.Examination)
                .FirstOrDefaultAsync(x => x.ExamFeeId == id);

            if (examFee == null)
                return NotFound(new { message = "ExamFee not found" });

            return Ok(examFee);
        }

        // 🔹 POST: api/ExamFee
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExamFee examFee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.ExamFees.Add(examFee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = examFee.ExamFeeId }, examFee);
        }

        // 🔹 PUT: api/ExamFee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExamFee examFee)
        {
            if (id != examFee.ExamFeeId)
                return BadRequest(new { message = "ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _context.ExamFees.AnyAsync(x => x.ExamFeeId == id);
            if (!exists)
                return NotFound();

            _context.Entry(examFee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(examFee);
        }

        // 🔹 DELETE: api/ExamFee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var examFee = await _context.ExamFees.FindAsync(id);
            if (examFee == null)
                return NotFound();

            _context.ExamFees.Remove(examFee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "ExamFee deleted successfully" });
        }
    }
}
