using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminDashboardController : Controller
    {
        private readonly MadrasahDbContext _context;

        public AdminDashboardController(MadrasahDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardVM();
            var today = DateTime.Today;
            var currentMonthStart = new DateTime(today.Year, today.Month, 1);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

            // ---------------- Financial Summary ----------------
            // Current Month Income (Fee Collections)
            model.CurrentMonthIncome = await _context.FeeCollections
                .Where(fc => fc.DatePaid.Date >= currentMonthStart && fc.DatePaid.Date <= currentMonthEnd)
                .SumAsync(fc => (decimal?)fc.AmountPaid) ?? 0;

            // Current Month Salary Expenses
            model.CurrentMonthSalaryExpense = await _context.Salaries
                .Where(sp => sp.PaymentDate.Date >= currentMonthStart && sp.PaymentDate.Date <= currentMonthEnd)
                .SumAsync(sp => (decimal?)sp.NetAmount) ?? 0;

            // Current Month Other Expenses
            model.CurrentMonthOtherExpense = await _context.Expenses
                .Where(e => e.Date.Date >= currentMonthStart && e.Date.Date <= currentMonthEnd)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;

            // Calculate Net Income for current month
            model.CurrentMonthNetIncome = model.CurrentMonthIncome - (model.CurrentMonthSalaryExpense + model.CurrentMonthOtherExpense);

            // Today's Collection (already have)
            model.TodayCollection = await _context.FeeCollections
                .Where(f => f.DatePaid.Date == today)
                .SumAsync(f => (decimal?)f.AmountPaid) ?? 0;

            // Monthly Expense (already have - renamed for clarity)
            model.MonthlyExpense = await _context.Expenses
                .Where(e => e.Date.Month == today.Month && e.Date.Year == today.Year)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;

            // ---------------- Summary Cards ----------------
            model.TotalStudents = await _context.Students.AsNoTracking().CountAsync();
            model.TotalTeachers = await _context.Teachers.AsNoTracking().CountAsync();
            model.TotalUsers = await _context.Users.AsNoTracking().CountAsync();
            model.TodaysAttendance = await _context.Attendances
                                                 .Where(a => a.Date.Date == DateTime.Today)
                                                 .AsNoTracking().CountAsync();
            model.TotalActiveCourses = await _context.Classes.AsNoTracking().CountAsync();

            model.PendingFees = await _context.FeeCollections
                                     .Where(f => f.Status == PaymentStatus.Pending)
                                     .AsNoTracking()
                                     .SumAsync(f => (decimal?)f.AmountPaid) ?? 0;

            model.TotalExpenses = await _context.Expenses.AsNoTracking()
                                         .SumAsync(e => (decimal?)e.Amount) ?? 0;

            model.TotalFeeCollected = await _context.FeeCollections.AsNoTracking()
                                             .SumAsync(f => (decimal?)f.AmountPaid) ?? 0;

            // ---------------- Tables ----------------
            model.TodayPayments = await _context.FeeCollections
                                         .Include(f => f.Student)
                                         .Include(f => f.FeeType)
                                         .Where(f => f.DatePaid.Date == DateTime.Today)
                                         .AsNoTracking().ToListAsync();

            model.PendingFeesList = await _context.FeeCollections
                                         .Include(f => f.Student)
                                         .Where(f => f.Status == PaymentStatus.Pending)
                                         .AsNoTracking().ToListAsync();

            model.TodaysAttendanceList = await _context.Attendances
                                             .Include(a => a.Student)
                                             .Where(a => a.Date.Date == DateTime.Today)
                                             .AsNoTracking().ToListAsync();

            model.TodayExpenses = await _context.Expenses
                                         .Where(e => e.Date.Date == DateTime.Today)
                                         .AsNoTracking().ToListAsync();

            // ---------------- Charts ----------------
            var now = DateTime.Now;

            var feeChartData = await _context.FeeCollections
                                     .Where(f => f.DatePaid.Year == now.Year)
                                     .GroupBy(f => f.DatePaid.Month)
                                     .Select(g => new
                                     {
                                         Month = g.Key,
                                         Total = g.Sum(f => f.AmountPaid)
                                     }).OrderBy(g => g.Month).ToListAsync();

            model.ChartMonths = feeChartData.Select(c => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(c.Month)).ToList();
            model.FeeCollectionTotals = feeChartData.Select(c => c.Total).ToList();

            var expenseChartData = await _context.Expenses
                                         .Where(e => e.Date.Year == now.Year)
                                         .GroupBy(e => e.Date.Month)
                                         .Select(g => new
                                         {
                                             Month = g.Key,
                                             Total = g.Sum(e => e.Amount)
                                         }).OrderBy(g => g.Month).ToListAsync();

            model.ExpenseTotals = expenseChartData.Select(c => c.Total).ToList();

            // ---------------- Financial Progress Bars Data ----------------
            // For progress bars showing income vs expenses
            if (model.CurrentMonthIncome > 0)
            {
                model.SalaryExpensePercentage = (model.CurrentMonthSalaryExpense / model.CurrentMonthIncome) * 100;
                model.OtherExpensePercentage = (model.CurrentMonthOtherExpense / model.CurrentMonthIncome) * 100;
                model.NetIncomePercentage = (Math.Abs(model.CurrentMonthNetIncome) / model.CurrentMonthIncome) * 100;
            }

            // ---------------- Attendance Pie ----------------
            var attendanceToday = await _context.Attendances
                                         .Where(a => a.Date.Date == DateTime.Today)
                                         .AsNoTracking()
                                         .ToListAsync();
            model.AttendancePresent = attendanceToday.Count(a => a.Status == AttendanceStatus.Present);
            model.AttendanceAbsent = attendanceToday.Count(a => a.Status == AttendanceStatus.Absent);

            // ---------------- Users & Roles ----------------
            var users = await _context.Users.AsNoTracking().ToListAsync();
            var userRoleList = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Join(_context.Roles,
                          ur => ur.RoleId,
                          r => r.Id,
                          (ur, r) => r.Name)
                    .ToListAsync();

                userRoleList.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            model.Users = userRoleList;

            // ---------------- Alerts ----------------
            model.PendingFeeAlerts = model.PendingFeesList
                                         .Select(f => $"{f.Student?.StudentName} owes {f.AmountPaid} ৳").ToList();

            model.LowAttendanceAlerts = model.TodaysAttendanceList
                                         .Where(a => a.Status != AttendanceStatus.Present)
                                         .Select(a => $"{a.Student?.StudentName} absent").ToList();

            model.UpcomingEvents = await _context.Events
                                         .Where(e => e.StartDateTime >= DateTime.Today)
                                         .OrderBy(e => e.StartDateTime)
                                         .Select(e => $"{e.Title} on {e.StartDateTime:yyyy-MM-dd}")
                                         .ToListAsync();

            return View(model);
        }
    }

    //public class DashboardController : Controller
    //{
    //    private readonly MadrasahDbContext _context;
    //    public IActionResult Index()
    //    {
    //        var today = DateTime.Today;
    //        var model = new DashboardVM
    //        {
    //            TodayCollection = _context.FeeCollections
    //                .Where(f => f.DatePaid.Date == today)
    //                .Sum(f => f.AmountPaid),
    //            MonthlyExpense = _context.Expenses
    //                .Where(e => e.Date.Month == today.Month && e.Date.Year == today.Year)
    //                .Sum(e => e.Amount),
    //            // Other metrics
    //        };

    //        return View(model);
    //    }
    //}
}
