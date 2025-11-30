using MadrasahManagement.Models;

namespace MadrasahManagement.ViewModels
{
    public class AdminDashboardVM
    {
        // ---------------- Summary Cards ----------------
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalUsers { get; set; }
        public int TodaysAttendance { get; set; }
        public decimal PendingFees { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalFeeCollected { get; set; }
        public int TotalActiveCourses { get; set; }

        // ---------------- Tables ----------------
        public IEnumerable<FeeCollection>? TodayPayments { get; set; }
        public IEnumerable<FeeCollection>? PendingFeesList { get; set; }
        public IEnumerable<Attendance>? TodaysAttendanceList { get; set; }
        public IEnumerable<Expense>? TodayExpenses { get; set; }

        // ---------------- Charts ----------------
        public List<string> ChartMonths { get; set; } = new List<string>();
        public List<string> ChartTotals { get; set; } = new List<string>();
        public List<decimal> FeeCollectionTotals { get; set; } = new List<decimal>();
        public List<decimal> ExpenseTotals { get; set; } = new List<decimal>();

    
        public int AttendancePresent { get; set; }
        public int AttendanceAbsent { get; set; }

        // ---------------- Users ----------------
        public IEnumerable<AppUser>? Users { get; set; }
        public IEnumerable<AppRole>? Roles { get; set; }

        // ---------------- Quick Actions & Notifications ----------------
        public IEnumerable<string>? PendingFeeAlerts { get; set; }
        public IEnumerable<string>? LowAttendanceAlerts { get; set; }
        public IEnumerable<string>? UpcomingEvents { get; set; }
    }
   
}
