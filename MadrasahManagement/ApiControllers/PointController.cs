using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public PointController(MadrasahDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        // GET: api/points
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointCondition>>> GetAll()
        {
            var data = await _context.PointConditions
                .Include(x => x.Class)
                .Include(x => x.Examination)
                .Include(x => x.Details)
                .ToListAsync();

            return Ok(data);
        }

        // ================= GET BY ID =================
        // GET: api/points/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PointCondition>> GetById(int id)
        {
            var condition = await _context.PointConditions
                .Include(x => x.Class)
                .Include(x => x.Examination)
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.PointConditionId == id);

            if (condition == null)
                return NotFound();

            return Ok(condition);
        }

        // ================= CREATE =================
        // POST: api/points
        [HttpPost]
        public async Task<IActionResult> Create(PointCondition condition)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.PointConditions.Add(condition);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = condition.PointConditionId }, condition);
        }

        // ================= UPDATE =================
        // PUT: api/points/5
       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PointCondition model)
        {
            if (id != model.PointConditionId)
                return BadRequest("Id mismatch");

            var existing = await _context.PointConditions
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.PointConditionId == id);

            if (existing == null)
                return NotFound();

            // 🔴 remove old details
            _context.PointConditionDetails.RemoveRange(existing.Details);

            // 🔵 update parent (ALL fields)
            existing.EducationYear = model.EducationYear;
            existing.ClassId = model.ClassId;
            existing.ExamId = model.ExamId;
            existing.PassMarks = model.PassMarks;      // ✅ FIX
            existing.HighestMark = model.HighestMark;  // ✅ FIX

            // 🔵 add new details
            existing.Details = model.Details;

            await _context.SaveChangesAsync();

            return Ok(existing); // or NoContent()
        }

        // ================= DELETE =================
        // DELETE: api/points/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var condition = await _context.PointConditions
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.PointConditionId == id);

            if (condition == null)
                return NotFound();

            _context.PointConditionDetails.RemoveRange(condition.Details);
            _context.PointConditions.Remove(condition);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
