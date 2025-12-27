using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SalaryController : Controller
{
    private readonly MadrasahDbContext _context;

    public SalaryController(MadrasahDbContext context)
    {
        _context = context;
    }

    // List all salary payments
    public IActionResult Index()
    {
        var salaries = _context.Salaries
            .Include(s => s.Teacher)
            .Include(s => s.Staff)
            .OrderByDescending(s => s.PaymentDate)
            .ToList();
        return View(salaries);
    }

    // Pay salary form
    public IActionResult PaySalary()
    {
        ViewBag.Teachers = _context.Teachers.ToList();
        ViewBag.Staff = _context.Staffs.ToList();
        ViewBag.Months = Enum.GetValues(typeof(Month)).Cast<Month>();
        ViewBag.PaymentMethod = Enum.GetValues(typeof(PaymentMethodType)).Cast<PaymentMethodType>();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> PaySalary(Salary model)
    {
        
            // Calculate net amount
            model.NetAmount = model.BasicSalary + model.Allowances - model.Deductions;
            model.PaymentDate = DateTime.Now;
            model.PaymentStatus = PaymentStatus.Paid;

            _context.Salaries.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
       
    }

    //[HttpPost]
    //public async Task<IActionResult> PaySalary(Salary model)
    //{

    //    bool alreadyPaid = await _context.Salaries.AnyAsync(s =>
    //        s.TeacherId == model.TeacherId || s.StaffId == model.StaffId &&
    //        s.PaymentDate.Month == model.PaymentDate.Month &&
    //        s.PaymentDate.Year == model.PaymentDate.Year);

    //    if (alreadyPaid)
    //    {
    //        ModelState.AddModelError("", "Salary has already been paid for this employee this month.");
    //        return View(model);
    //    }


    //    model.NetAmount = model.BasicSalary + model.Allowances - model.Deductions;
    //    model.PaymentDate = DateTime.Now;
    //    model.PaymentStatus = PaymentStatus.Paid;


    //    _context.Salaries.Add(model);
    //    await _context.SaveChangesAsync();

    //    return RedirectToAction("Index");
    //}


    // Edit salary
    public async Task<IActionResult> Edit(int id)
    {
        var salary = await _context.Salaries
            .Include(s => s.Teacher)
            .Include(s => s.Staff)
            .FirstOrDefaultAsync(s => s.SalaryId == id);

        if (salary == null) return NotFound();

        ViewBag.Teachers = _context.Teachers.ToList();
        ViewBag.Staff = _context.Staffs.ToList();
        ViewBag.Months = Enum.GetValues(typeof(Month)).Cast<Month>();
        ViewBag.PaymentMethod = Enum.GetValues(typeof(PaymentMethodType)).Cast<PaymentMethodType>();
        return View(salary);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Salary model)
    {
        model.NetAmount = model.BasicSalary + model.Allowances - model.Deductions;

        _context.Salaries.Update(model);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // Delete salary
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var salary = await _context.Salaries.FindAsync(id);
        if (salary != null)
        {
            _context.Salaries.Remove(salary);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    // Salary slip/view
    public IActionResult Details(int id)
    {
        var salary = _context.Salaries
            .Include(s => s.Teacher)
            .Include(s => s.Staff)
            .FirstOrDefault(s => s.SalaryId == id);

        return View(salary);
    }

    //// Monthly salary report
    //public IActionResult MonthlyReport(Month? month, int? year)
    //{
    //    var query = _context.Salaries
    //        .Include(s => s.Teacher)
    //        .Include(s => s.Staff)
    //        .AsQueryable();

    //    if (month.HasValue)
    //        query = query.Where(s => s.MonthName == month.Value);

    //    if (year.HasValue)
    //        query = query.Where(s => s.PaymentDate.Year == year.Value);

    //    var salaries = query.OrderByDescending(s => s.PaymentDate).ToList();

    //    ViewBag.TotalAmount = salaries.Sum(s => s.NetAmount);
    //    ViewBag.TotalCount = salaries.Count;

    //    return View(salaries);
    //}

   
    public IActionResult SalaryReport(string searchName = "", string employeeType = "",
                                       int? month = null, int? year = null)
    {
        var query = _context.Salaries
            .Include(s => s.Teacher)
            .Include(s => s.Staff)
            .AsQueryable();

        // Search by employee name
        if (!string.IsNullOrEmpty(searchName))
        {
            query = query.Where(s =>
                (s.Teacher != null && s.Teacher.Name.Contains(searchName)) ||
                (s.Staff != null && s.Staff.StaffName.Contains(searchName))
            );
        }

        // Filter by employee type
        if (!string.IsNullOrEmpty(employeeType))
        {
            if (employeeType == "teacher")
                query = query.Where(s => s.TeacherId != null);
            else if (employeeType == "staff")
                query = query.Where(s => s.StaffId != null);
        }

        // Filter by month
        if (month.HasValue)
            query = query.Where(s => s.MonthName == (Month)month.Value);

        // Filter by year
        if (year.HasValue)
            query = query.Where(s => s.PaymentDate.Year == year.Value);

        var salaries = query.OrderByDescending(s => s.PaymentDate).ToList();

        // Pass search parameters to view
        ViewBag.SearchName = searchName;
        ViewBag.EmployeeType = employeeType;
        ViewBag.SelectedMonth = month;
        ViewBag.SelectedYear = year;

        return View(salaries);
    }
}