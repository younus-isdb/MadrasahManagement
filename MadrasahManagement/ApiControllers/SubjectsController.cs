using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public SubjectsController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Subjects
                .Include(s => s.Class)
                .Include(s => s.Department)
                .Select(s => new SubjectReadDto
                {
                    SubjectId = s.SubjectId,
                    ClassId = s.ClassId,
                    ClassName = s.Class.ClassName,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.DepartmentName,
                    SubjectName = s.SubjectName,
                    SubjectCode = s.SubjectCode,
                    IsOptional = s.IsOptional
                }).ToListAsync();

            return Ok(data);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _context.Subjects
                .Include(s => s.Class)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.SubjectId == id);

            if (s == null) return NotFound();

            var dto = new SubjectReadDto
            {
                SubjectId = s.SubjectId,
                ClassId = s.ClassId,
                ClassName = s.Class.ClassName,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department.DepartmentName,
                SubjectName = s.SubjectName,
                SubjectCode = s.SubjectCode,
                IsOptional = s.IsOptional
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubjectCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Subject
            {
                ClassId = dto.ClassId,
                DepartmentId = dto.DepartmentId,
                SubjectName = dto.SubjectName,
                SubjectCode = dto.SubjectCode,
                IsOptional = dto.IsOptional
            };

            _context.Subjects.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.SubjectId }, entity);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubjectUpdateDto dto)
        {
            if (id != dto.SubjectId) return BadRequest("Id mismatch");

            var existing = await _context.Subjects.FindAsync(id);
            if (existing == null) return NotFound();

            existing.ClassId = dto.ClassId;
            existing.DepartmentId = dto.DepartmentId;
            existing.SubjectName = dto.SubjectName;
            existing.SubjectCode = dto.SubjectCode;
            existing.IsOptional = dto.IsOptional;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.Subjects.FindAsync(id);
            if (existing == null) return NotFound();

            _context.Subjects.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
