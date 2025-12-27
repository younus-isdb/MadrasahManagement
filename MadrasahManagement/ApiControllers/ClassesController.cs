using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ClassesController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _context.Classes
                .Include(c => c.Department)
                .Select(c => new ClassReadDto
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    SessionYear = c.SessionYear,
                    DepartmentId = c.DepartmentId,
                    DepartmentName = c.Department.DepartmentName
                }).ToListAsync();

            return Ok(classes);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _context.Classes
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.ClassId == id);

            if (c == null) return NotFound();

            var dto = new ClassReadDto
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                SessionYear = c.SessionYear,
                DepartmentId = c.DepartmentId,
                DepartmentName = c.Department.DepartmentName
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var c = new Class
            {
                ClassName = dto.ClassName,
                SessionYear = dto.SessionYear,
                DepartmentId = dto.DepartmentId
            };

            _context.Classes.Add(c);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = c.ClassId }, c);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassUpdateDto dto)
        {
            if (id != dto.ClassId) return BadRequest("ID mismatch");

            var c = await _context.Classes.FindAsync(id);
            if (c == null) return NotFound();

            c.ClassName = dto.ClassName;
            c.SessionYear = dto.SessionYear;
            c.DepartmentId = dto.DepartmentId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.Classes.FindAsync(id);
            if (c == null) return NotFound();

            _context.Classes.Remove(c);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
