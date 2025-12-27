using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public SectionsController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sections = await _context.Sections
                .Include(s => s.Class)
                .Select(s => new SectionReadDto
                {
                    SectionId = s.SectionId,
                    ClassId = s.ClassId,
                    ClassName = s.Class.ClassName,
                    SectionName = s.SectionName
                }).ToListAsync();

            return Ok(sections);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var section = await _context.Sections
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.SectionId == id);

            if (section == null) return NotFound();

            var dto = new SectionReadDto
            {
                SectionId = section.SectionId,
                ClassId = section.ClassId,
                ClassName = section.Class.ClassName,
                SectionName = section.SectionName
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SectionCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var section = new Section
            {
                ClassId = dto.ClassId,
                SectionName = dto.SectionName
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = section.SectionId }, section);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SectionUpdateDto dto)
        {
            if (id != dto.SectionId) return BadRequest("ID mismatch");

            var section = await _context.Sections.FindAsync(id);
            if (section == null) return NotFound();

            section.ClassId = dto.ClassId;
            section.SectionName = dto.SectionName;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null) return NotFound();

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
