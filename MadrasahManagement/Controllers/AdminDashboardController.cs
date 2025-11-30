using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
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

            // ---------------- Attendance Pie ----------------
            var attendanceToday = await _context.Attendances
                                        .Where(a => a.Date.Date == DateTime.Today)
                                        .AsNoTracking()
                                        .ToListAsync();
            model.AttendancePresent = attendanceToday.Count(a => a.Status == AttendanceStatus.Present);
            model.AttendanceAbsent = attendanceToday.Count(a => a.Status == AttendanceStatus.Absent);

            // ---------------- Users & Roles ----------------
            model.Users = await _context.Users.AsNoTracking().ToListAsync();
            model.Roles = await _context.Roles.AsNoTracking().ToListAsync();

            // ---------------- Alerts ----------------
            model.PendingFeeAlerts = model.PendingFeesList
                                        .Select(f => $"{f.Student.StudentName} owes {f.AmountPaid} ৳").ToList();

            model.LowAttendanceAlerts = model.TodaysAttendanceList
                                        .Where(a => a.Status != AttendanceStatus.Present)
                                        .Select(a => $"{a.Student.StudentName} absent").ToList();

            model.UpcomingEvents = await _context.Events
                                        .Where(e => e.StartDateTime >= DateTime.Today)
                                        .OrderBy(e => e.StartDateTime)
                                        .Select(e => $"{e.Title} on {e.StartDateTime:yyyy-MM-dd}")
                                        .ToListAsync();

            return View(model);
        }
    }
}
