using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsFeeCollectionsController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExamsFeeCollectionsController(MadrasahDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        // GET: api/ExamFeeCollections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamFeeCollection>>> GetAll()
        {
            var data = await _context.ExamFeeCollections
                .Include(x => x.Examination)
                .Include(x => x.Class)
                .Include(x => x.Student)
                .ToListAsync();

            var totalAmount = data.Sum(x => x.ExamFee);

            return Ok(new { totalAmount, data });
        }

        // ================= GET BY ID =================
        // GET: api/ExamFeeCollections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamFeeCollection>> GetById(int id)
        {
            var data = await _context.ExamFeeCollections
                .Include(x => x.Examination)
                .Include(x => x.Class)
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.FeeCollectionId == id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // ================= CREATE =================
        // POST: api/ExamFeeCollections
        [HttpPost]
        public async Task<IActionResult> Create(ExamFeeCollection model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.ExamFeeCollections.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = model.FeeCollectionId }, model);
        }

        // ================= UPDATE =================
        // PUT: api/ExamFeeCollections/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ExamFeeCollection model)
        {
            if (id != model.FeeCollectionId)
                return BadRequest("Id mismatch");

            var existing = await _context.ExamFeeCollections.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Update fields
            existing.StudentId = model.StudentId;
            existing.ExamId = model.ExamId;
            existing.ClassId = model.ClassId;
            existing.ExamFee = model.ExamFee;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ================= DELETE =================
        // DELETE: api/ExamFeeCollections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.ExamFeeCollections.FindAsync(id);
            if (data == null)
                return NotFound();

            _context.ExamFeeCollections.Remove(data);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
