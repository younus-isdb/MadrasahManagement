using MadrasahManagement.Dto;
using MadrasahManagement.Dto.MadrasahManagement.Dto;
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

        // GET: api/ExamFee
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.ExamFees
                .Include(f => f.Class)
                .Include(f => f.Examination)
                .Select(f => new ExamFeeReadDto
                {
                    ExamFeeId = f.ExamFeeId,
                    EducationYear = f.EducationYear,
                    ClassId = f.ClassId,
                    ClassName = f.Class!.ClassName,
                    ExamId = f.ExamId,
                    ExamName = f.Examination!.ExamName,
                    ExamAmount = f.ExamAmount
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/ExamFee/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var f = await _context.ExamFees
                .Include(f => f.Class)
                .Include(f => f.Examination)
                .FirstOrDefaultAsync(x => x.ExamFeeId == id);

            if (f == null)
                return NotFound(new { message = "ExamFee not found" });

            var dto = new ExamFeeReadDto
            {
                ExamFeeId = f.ExamFeeId,
                EducationYear = f.EducationYear,
                ClassId = f.ClassId,
                ClassName = f.Class!.ClassName,
                ExamId = f.ExamId,
                ExamName = f.Examination!.ExamName,
                ExamAmount = f.ExamAmount
            };

            return Ok(dto);
        }

        // POST: api/ExamFee
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExamFeeCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var examFee = new ExamFee
            {
                EducationYear = dto.EducationYear,
                ClassId = dto.ClassId,
                ExamId = dto.ExamId,
                ExamAmount = dto.ExamAmount
            };

            _context.ExamFees.Add(examFee);
            await _context.SaveChangesAsync();

            // Return read DTO
            var readDto = new ExamFeeReadDto
            {
                ExamFeeId = examFee.ExamFeeId,
                EducationYear = examFee.EducationYear,
                ClassId = examFee.ClassId,
                ClassName = examFee.Class?.ClassName ?? "",
                ExamId = examFee.ExamId,
                ExamName = examFee.Examination?.ExamName ?? "",
                ExamAmount = examFee.ExamAmount
            };

            return CreatedAtAction(nameof(Get), new { id = examFee.ExamFeeId }, readDto);
        }

        // PUT: api/ExamFee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExamFeeUpdateDto dto)
        {
            if (id != dto.ExamFeeId)
                return BadRequest(new { message = "ID mismatch" });

            var examFee = await _context.ExamFees.FindAsync(id);
            if (examFee == null)
                return NotFound(new { message = "ExamFee not found" });

            examFee.EducationYear = dto.EducationYear;
            examFee.ClassId = dto.ClassId;
            examFee.ExamId = dto.ExamId;
            examFee.ExamAmount = dto.ExamAmount;

            await _context.SaveChangesAsync();

            return Ok(dto);
        }

        // DELETE: api/ExamFee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var examFee = await _context.ExamFees.FindAsync(id);
            if (examFee == null)
                return NotFound(new { message = "ExamFee not found" });

            _context.ExamFees.Remove(examFee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "ExamFee deleted successfully" });
        }
    }
}
