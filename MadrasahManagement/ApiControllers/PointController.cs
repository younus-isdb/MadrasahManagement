using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.PointConditions
                .Include(p => p.Details)
                .Select(p => new PointConditionReadDto
                {
                    PointConditionId = p.PointConditionId,
                    EducationYear = p.EducationYear,
                    ClassId = p.ClassId,
                    ExamId = p.ExamId,
                    PassMarks = p.PassMarks,
                    HighestMark = p.HighestMark,
                    Details = p.Details.Select(d => new PointConditionDetailReadDto
                    {
                        PointConditionDetailId = d.PointConditionDetailId,
                        FromMark = d.FromMark,
                        ToMark = d.ToMark,
                        Division = d.Division,
                        IsSilverColor = d.IsSilverColor
                    }).ToList()
                }).ToListAsync();

            return Ok(data);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _context.PointConditions
                .Include(pc => pc.Details)
                .FirstOrDefaultAsync(pc => pc.PointConditionId == id);

            if (p == null) return NotFound();

            var dto = new PointConditionReadDto
            {
                PointConditionId = p.PointConditionId,
                EducationYear = p.EducationYear,
                ClassId = p.ClassId,
                ExamId = p.ExamId,
                PassMarks = p.PassMarks,
                HighestMark = p.HighestMark,
                Details = p.Details.Select(d => new PointConditionDetailReadDto
                {
                    PointConditionDetailId = d.PointConditionDetailId,
                    FromMark = d.FromMark,
                    ToMark = d.ToMark,
                    Division = d.Division,
                    IsSilverColor = d.IsSilverColor
                }).ToList()
            };

            return Ok(dto);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PointConditionCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new PointCondition
            {
                EducationYear = dto.EducationYear,
                ClassId = dto.ClassId,
                ExamId = dto.ExamId,
                PassMarks = dto.PassMarks,
                HighestMark = dto.HighestMark,
                Details = dto.Details.Select(d => new PointConditionDetail
                {
                    FromMark = d.FromMark,
                    ToMark = d.ToMark,
                    Division = d.Division,
                    IsSilverColor = d.IsSilverColor
                }).ToList()
            };

            _context.PointConditions.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.PointConditionId }, entity);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PointConditionUpdateDto dto)
        {
            if (id != dto.PointConditionId) return BadRequest("Id mismatch");

            var existing = await _context.PointConditions
                .Include(p => p.Details)
                .FirstOrDefaultAsync(p => p.PointConditionId == id);

            if (existing == null) return NotFound();

            // Remove old details
            _context.PointConditionDetails.RemoveRange(existing.Details);

            // Update parent fields
            existing.EducationYear = dto.EducationYear;
            existing.ClassId = dto.ClassId;
            existing.ExamId = dto.ExamId;
            existing.PassMarks = dto.PassMarks;
            existing.HighestMark = dto.HighestMark;

            // Add new details
            existing.Details = dto.Details.Select(d => new PointConditionDetail
            {
                FromMark = d.FromMark,
                ToMark = d.ToMark,
                Division = d.Division,
                IsSilverColor = d.IsSilverColor
            }).ToList();

            await _context.SaveChangesAsync();

            // Return updated object
            var readDto = new PointConditionReadDto
            {
                PointConditionId = existing.PointConditionId,
                EducationYear = existing.EducationYear,
                ClassId = existing.ClassId,
                ExamId = existing.ExamId,
                PassMarks = existing.PassMarks,
                HighestMark = existing.HighestMark,
                Details = existing.Details.Select(d => new PointConditionDetailReadDto
                {
                    PointConditionDetailId = d.PointConditionDetailId,
                    FromMark = d.FromMark,
                    ToMark = d.ToMark,
                    Division = d.Division,
                    IsSilverColor = d.IsSilverColor
                }).ToList()
            };

            return Ok(readDto);
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.PointConditions
                .Include(p => p.Details)
                .FirstOrDefaultAsync(p => p.PointConditionId == id);

            if (existing == null) return NotFound();

            _context.PointConditionDetails.RemoveRange(existing.Details);
            _context.PointConditions.Remove(existing);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
