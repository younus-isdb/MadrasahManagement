using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly MadrasahDbContext _context;

    public DepartmentController(MadrasahDbContext context)
    {
        _context = context;
    }

    // GET: api/Department
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetDepartments()
    {
        var departments = await _context.Departments.ToListAsync();

        // Manual mapping
        var dtos = departments.Select(d => new DepartmentReadDto
        {
            DepartmentId = d.DepartmentId,
            DepartmentName = d.DepartmentName,
            Description = d.Description
        }).ToList();

        return Ok(dtos);
    }

    // GET: api/Department/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentReadDto>> GetDepartment(int id)
    {
        var d = await _context.Departments.FindAsync(id);
        if (d == null) return NotFound();

        var dto = new DepartmentReadDto
        {
            DepartmentId = d.DepartmentId,
            DepartmentName = d.DepartmentName,
            Description = d.Description
        };
        return Ok(dto);
    }

    // POST: api/Department
    [HttpPost]
    public async Task<ActionResult> CreateDepartment(DepartmentCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var d = new Department
        {
            DepartmentName = dto.DepartmentName,
            Description = dto.Description
        };

        _context.Departments.Add(d);
        await _context.SaveChangesAsync();

        var readDto = new DepartmentReadDto
        {
            DepartmentId = d.DepartmentId,
            DepartmentName = d.DepartmentName,
            Description = d.Description
        };

        return CreatedAtAction(nameof(GetDepartment), new { id = d.DepartmentId }, readDto);
    }

    // PUT: api/Department/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateDepartment(int id, DepartmentUpdateDto dto)
    {
        if (id != dto.DepartmentId) return BadRequest("Id mismatch");

        var d = await _context.Departments.FindAsync(id);
        if (d == null) return NotFound();

        d.DepartmentName = dto.DepartmentName;
        d.Description = dto.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Department/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDepartment(int id)
    {
        var d = await _context.Departments.FindAsync(id);
        if (d == null) return NotFound();

        _context.Departments.Remove(d);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
