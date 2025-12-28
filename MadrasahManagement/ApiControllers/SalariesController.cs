using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SalariesController : ControllerBase
{
    private readonly MadrasahDbContext _context;

    public SalariesController(MadrasahDbContext context)
    {
        _context = context;
    }

    // List all salary payments
    [HttpGet]
    public IActionResult GetSalaries()
    {
        var salaries = _context.Salaries
            .Include(s => s.Teacher)
            .Include(s => s.Staff)
            .OrderByDescending(s => s.PaymentDate)
            .ToList();
        return Ok(salaries);
    }

    [HttpPost("pay")]
    public async Task<IActionResult> PaySalary([FromBody] SalaryApiDto dto)
    {
        try
        {
            // Validate that either TeacherId or StaffId is provided
            if (!dto.TeacherId.HasValue && !dto.StaffId.HasValue)
            {
                return BadRequest("Either TeacherId or StaffId must be provided.");
            }

            if (dto.TeacherId.HasValue && dto.StaffId.HasValue)
            {
                return BadRequest("Only one of TeacherId or StaffId should be provided, not both.");
            }

            // Check if salary already paid for this employee this month
            bool alreadyPaid = await _context.Salaries.AnyAsync(s =>
                (s.TeacherId == dto.TeacherId || s.StaffId == dto.StaffId) &&
                s.MonthName == dto.MonthName &&
                s.Year == dto.Year);

            if (alreadyPaid)
            {
                return BadRequest("Salary has already been paid for this employee this month.");
            }

            // Create Salary entity from DTO
            var salary = new Salary
            {
                TeacherId = dto.TeacherId,
                StaffId = dto.StaffId,
                BasicSalary = dto.BasicSalary,
                Allowances = dto.Allowances,
                Deductions = dto.Deductions,
                NetAmount = dto.BasicSalary + dto.Allowances - dto.Deductions,
                MonthName = dto.MonthName,
                Year = dto.Year,
                PaymentMethod = dto.PaymentMethod,
                PaymentDate = DateTime.Now,
                PaymentStatus = PaymentStatus.Paid
            };

            _context.Salaries.Add(salary);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSalaryDetails), new { id = salary.SalaryId }, salary);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSalary(int id)
    {
        var salary = await _context.Salaries
            .Include(s => s.Teacher)
            .Include(s => s.Staff)
            .FirstOrDefaultAsync(s => s.SalaryId == id);

        if (salary == null) return NotFound();

        return Ok(salary);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSalary(int id, [FromBody] UpdateSalaryApiDto dto)
    {
        try
        {
            var existingSalary = await _context.Salaries.FindAsync(id);

            if (existingSalary == null)
            {
                return NotFound();
            }

            // Update only the fields we want to allow editing
            existingSalary.BasicSalary = dto.BasicSalary;
            existingSalary.Allowances = dto.Allowances;
            existingSalary.Deductions = dto.Deductions;
            existingSalary.MonthName = dto.MonthName;
            existingSalary.Year = dto.Year;
            existingSalary.PaymentMethod = dto.PaymentMethod;

            // Recalculate net amount
            existingSalary.NetAmount = dto.BasicSalary + dto.Allowances - dto.Deductions;

            _context.Salaries.Update(existingSalary);
            await _context.SaveChangesAsync();

            return Ok("updated !");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
        
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSalary(int id)
    {
        var salary = await _context.Salaries.FindAsync(id);
        if (salary == null)
        {
            return NotFound();
        }

        _context.Salaries.Remove(salary);
        await _context.SaveChangesAsync();

        return Ok("salary deleted successfully");
    }

    [HttpGet("details/{id}")]
    public IActionResult GetSalaryDetails(int id)
    {
        var salary = _context.Salaries
            .Include(s => s.Teacher)
            .Include(s => s.Staff)
            .FirstOrDefault(s => s.SalaryId == id);

        if (salary == null)
        {
            return NotFound();
        }

        return Ok(salary);
    }

    [HttpGet("report")]
    public IActionResult GetSalaryReport([FromQuery] int? month = null, [FromQuery] int? year = null)
    {
        // Get all salaries first to debug
        var allSalaries = _context.Salaries
            .Include(s => s.Teacher)
            .Include(s => s.Staff)
            .ToList();

        Console.WriteLine($"Total salaries in DB: {allSalaries.Count}");

        if (allSalaries.Any())
        {
            Console.WriteLine("Sample salary data:");
            foreach (var salary in allSalaries.Take(3))
            {
                Console.WriteLine($"ID: {salary.SalaryId}, Month: {salary.MonthName}, Year: {salary.Year}, PaymentDate: {salary.PaymentDate}");
            }
        }

        var query = _context.Salaries
            .Include(s => s.Teacher)
            .Include(s => s.Staff)
            .AsQueryable();

        if (month.HasValue)
        {
            Console.WriteLine($"Applying month filter: {(Month)month.Value}");
            query = query.Where(s => s.MonthName == (Month)month.Value);
        }

        if (year.HasValue)
        {
            Console.WriteLine($"Applying year filter: {year.Value}");
            query = query.Where(s => s.Year == year.Value);
        }

        var salaries = query.OrderByDescending(s => s.PaymentDate).ToList();

        Console.WriteLine($"Filtered salaries found: {salaries.Count}");

        return Ok(salaries);
    }
}