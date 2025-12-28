using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

[ApiController]
[Route("api/[controller]")]
public class ExpenseApiController : ControllerBase
{
    private readonly MadrasahDbContext _context;

    public ExpenseApiController(MadrasahDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetExpenses()
    {
        var expenses = _context.Expenses
            .OrderByDescending(e => e.Date)
            .ToList();
        return Ok(expenses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null) return NotFound();
        return Ok(expense);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense(Expense model)
    {
        model.Date = DateTimeOffset.Now;
        _context.Expenses.Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetExpense), new { id = model.ExpenseId }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(int id, Expense model)
    {
        if (id != model.ExpenseId)
        {
            return BadRequest();
        }

        _context.Expenses.Update(model);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("report")]
    public IActionResult GetExpenseReport(
        [FromQuery] string type = "",
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = _context.Expenses.AsQueryable();

        if (!string.IsNullOrEmpty(type))
            query = query.Where(e => e.Type == type);

        if (startDate.HasValue)
            query = query.Where(e => e.Date.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(e => e.Date.Date <= endDate.Value.Date);

        var expenses = query.OrderByDescending(e => e.Date).ToList();

        var totalAmount = expenses.Sum(e => e.Amount);
        var totalCount = expenses.Count;
        var averageAmount = expenses.Any() ? expenses.Average(e => e.Amount) : 0;

        return Ok(new
        {
            Expenses = expenses,
            TotalAmount = totalAmount,
            TotalCount = totalCount,
            AverageAmount = averageAmount,
            Type = type,
            StartDate = startDate,
            EndDate = endDate
        });
    }

    [HttpGet("monthly-report")]
    public IActionResult GetMonthlyReport([FromQuery] int? year = null)
    {
        var selectedYear = year ?? DateTime.Now.Year;

        var monthlyData = _context.Expenses
            .Where(e => e.Date.Year == selectedYear)
            .GroupBy(e => e.Date.Month)
            .Select(g => new MonthlyReportVM
            {
                Month = g.Key,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                TotalAmount = g.Sum(e => e.Amount),
                Count = g.Count()
            })
            .OrderBy(m => m.Month)
            .ToList();

        var allMonths = Enumerable.Range(1, 12).Select(m => new MonthlyReportVM
        {
            Month = m,
            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
            TotalAmount = monthlyData.FirstOrDefault(d => d.Month == m)?.TotalAmount ?? 0,
            Count = monthlyData.FirstOrDefault(d => d.Month == m)?.Count ?? 0
        }).ToList();

        var totalYear = allMonths.Sum(m => m.TotalAmount);

        var availableYears = _context.Expenses
            .Select(e => e.Date.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();

        return Ok(new
        {
            SelectedYear = selectedYear,
            MonthlyData = allMonths,
            TotalYear = totalYear,
            AvailableYears = availableYears
        });
    }

    [HttpGet("types")]
    public IActionResult GetExpenseTypes()
    {
        var types = new List<string>
        {
            "Salary & Allowances",
            "Utilities",
            "Stationary",
            "Maintenance",
            "Transport",
            "Food",
            "Cleaning",
            "Security",
            "Internet",
            "Phone",
            "Printing",
            "Event",
            "Sports",
            "Library",
            "Laboratory",
            "Teacher Training",
            "Office Supplies",
            "Furniture",
            "Electronics",
            "Medical",
            "Examination",
            "Uniform",
            "Other"
        };
        return Ok(types);
    }
}