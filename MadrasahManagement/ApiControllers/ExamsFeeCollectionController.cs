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

            if (model.SubjectIds == null || model.SubjectIds.Count == 0)
                return BadRequest("At least one subject is required");

            var insertedIds = new List<int>();

            foreach (var subjectId in model.SubjectIds)
            {
                var entity = new ExamFeeCollection
                {
                    EducationYear = model.EducationYear,
                    ExamId = model.ExamId,
                    ClassId = model.ClassId,
                    StudentId = model.StudentId,
                    ExamFee = model.ExamFee,
                    SubjectId = subjectId,
                    TotalSubject = model.SubjectIds.Count.ToString()
                };

                _context.ExamFeeCollections.Add(entity);
                insertedIds.Add(entity.FeeCollectionId);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Exam fee collection created successfully",
                totalSubject = model.SubjectIds.Count
            });
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
