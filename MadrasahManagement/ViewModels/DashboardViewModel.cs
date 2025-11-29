using MadrasahManagement.Controllers;
using MadrasahManagement.Models;

public class DashboardViewModel
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalClasses { get; set; }
    public int TotalSubjects { get; set; }

    public List<Event> UpcomingEvents { get; set; } = new();
    public List<Notice> Notices { get; set; } = new();
    //public List<QuickLink> QuickLinks { get; set; } = new();
    //public List<ChartData> AttendanceTrend { get; set; } = new();
    //public List<ChartData> FeeCollectionTrend { get; set; } = new();
}
