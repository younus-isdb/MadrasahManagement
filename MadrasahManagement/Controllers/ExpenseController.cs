using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

public class ExpensesController : Controller
{
    private readonly MadrasahDbContext _context;

    public ExpensesController(MadrasahDbContext context)
    {
        _context = context;
    }

    // GET: ExpensesController
    public IActionResult Index()
    {
        var expenses = _context.Expenses
            .OrderByDescending(e => e.Date)
            .ToList();
        return View(expenses);
    }


    public IActionResult Create()
    {
        ViewBag.ExpenseTypes = GetExpenseTypes();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Expense model)
    {
        if (ModelState.IsValid)
        {
            model.Date = DateTimeOffset.Now;
            _context.Expenses.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        ViewBag.ExpenseTypes = GetExpenseTypes();
        return View(model);
    }


    [HttpGet]
    // GET: ExpensesController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null) return NotFound();

        ViewBag.ExpenseTypes = GetExpenseTypes();
        return View(expense);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Expense model)
    {
        if (ModelState.IsValid)
        {
            _context.Expenses.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        ViewBag.ExpenseTypes = GetExpenseTypes();
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    // GET: ExpensesController/Details/5
    public IActionResult Details(int id)
    {
        var expense = _context.Expenses.Find(id);
        if (expense == null) return NotFound();
        return View(expense);
    }

    //report
    public IActionResult Report(string type = "", DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Expenses.AsQueryable();

        // Filter by expense type
        if (!string.IsNullOrEmpty(type))
            query = query.Where(e => e.Type == type);

        // Filter by date range
        if (startDate.HasValue)
            query = query.Where(e => e.Date.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(e => e.Date.Date <= endDate.Value.Date);

        var expenses = query.OrderByDescending(e => e.Date).ToList();

        // Statistics
        ViewBag.TotalAmount = expenses.Sum(e => e.Amount);
        ViewBag.TotalCount = expenses.Count;
        ViewBag.AverageAmount = expenses.Any() ? expenses.Average(e => e.Amount) : 0;

        // For filters
        ViewBag.ExpenseTypes = GetExpenseTypes();
        ViewBag.SelectedType = type;
        ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

        return View(expenses);
    }


    // MONTHLY REPORT 
    public IActionResult MonthlyReport(int? year)
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

        // Fill in missing months with zero values
        var allMonths = Enumerable.Range(1, 12).Select(m => new MonthlyReportVM
        {
            Month = m,
            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
            TotalAmount = monthlyData.FirstOrDefault(d => d.Month == m)?.TotalAmount ?? 0,
            Count = monthlyData.FirstOrDefault(d => d.Month == m)?.Count ?? 0
        }).ToList();

        ViewBag.SelectedYear = selectedYear;
        ViewBag.TotalYear = allMonths.Sum(m => m.TotalAmount);
        ViewBag.Years = _context.Expenses
            .Select(e => e.Date.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();

        return View(allMonths);
    }


    private List<string> GetExpenseTypes()
    {
        return new List<string>
        {
            "Salary & Allowances",
            "Utilities",           // Electricity, Water, Gas
            "Stationary",          // Papers, Pens, Books
            "Maintenance",         // Building, Furniture repair
            "Transport",           // Vehicle, Fuel
            "Food",                // Canteen, Tea, Snacks
            "Cleaning",            // Cleaning supplies
            "Security",            // Security services
            "Internet",            // Internet bills
            "Phone",               // Phone bills
            "Printing",            // Photocopy, Printing
            "Event",               // School events, functions
            "Sports",              // Sports equipment
            "Library",             // Books, magazines
            "Laboratory",          // Lab equipment
            "Teacher Training",    // Training programs
            "Office Supplies",     // Office items
            "Furniture",           // Chairs, tables, etc.
            "Electronics",         // Computers, printers
            "Medical",             // First aid, medical expenses
            "Examination",         // Exam papers, hall charges
            "Uniform",             // Staff/Student uniforms
            "Other"                // Miscellaneous
        };
    }
}