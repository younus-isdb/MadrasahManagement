using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    /* =========================
       STEP 1: SELECTION VM
       ========================= */
    public class TimetableSelectionVM
    {
        [Required(ErrorMessage = "Academic Year is required")]
        public string AcademicYear { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        public List<SelectListItem> AcademicYears { get; set; } = new();
        public List<SelectListItem> Departments { get; set; } = new();
        public List<SelectListItem> Classes { get; set; } = new();
        public List<SelectListItem> Sections { get; set; } = new();
    }

    /* =========================
       PERIOD STRUCTURE (UI)
       ========================= */
    public class PeriodVM
    {
        public string PeriodName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsBreak { get; set; }
        public bool IsAssembly { get; set; }

        public string TimeDisplay => $"{StartTime} - {EndTime}";
    }

    /* =========================
       GRID CELL
       ========================= */
    public class GridCellVM
    {
        public string Day { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;

        // Already nullable - good!
        public int? SubjectId { get; set; }
        public int? TeacherId { get; set; }

        public bool IsBreak { get; set; }
        public bool IsAssembly { get; set; }

        public string? SubjectName { get; set; }
        public string? TeacherName { get; set; }
    }

    /* =========================
       MAIN GRID VM (CREATE)
       ========================= */
    public class TimetableGridVM
    {
        public string AcademicYear { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }

        public bool IsEditMode { get; set; } = false;

        public string DepartmentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;

        public List<string> Days { get; set; } = new();
        public List<PeriodVM> Periods { get; set; } = new();

        // Day → Period → Cell
        public Dictionary<string, Dictionary<string, GridCellVM>> GridData { get; set; }
            = new Dictionary<string, Dictionary<string, GridCellVM>>();

        public List<SelectListItem> Subjects { get; set; } = new();
        public List<SelectListItem> Teachers { get; set; } = new();
    }

    /* =========================
       SAVE VM (POST)
       ========================= */
    public class SaveTimetableVM
    {
        public string AcademicYear { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }

        public List<GridCellVM> Cells { get; set; } = new();
    }

    /* =========================
       INDEX / LISTING
       ========================= */
    public class TimetableIndexVM
    {
        public string? AcademicYear { get; set; }
        public int? DepartmentId { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }

        public List<SelectListItem> AcademicYears { get; set; } = new();
        public List<SelectListItem> Departments { get; set; } = new();
        public List<SelectListItem> Classes { get; set; } = new();
        public List<SelectListItem> Sections { get; set; } = new();

        public List<TimetableSummaryVM> Timetables { get; set; } = new();
    }

    /* =========================
       SUMMARY VM
       ========================= */
    public class TimetableSummaryVM
    {
        public int Id { get; set; }
        public string AcademicYear { get; set; } = string.Empty;

        public int DepartmentId { get; set; }      // Add this
        public int ClassId { get; set; }           // Add this
        public int SectionId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;

        public int TotalPeriods { get; set; }
        public int BreakPeriods { get; set; }
        public int AssemblyPeriods { get; set; }

        public DateTime CreatedDate { get; set; }

        public string DisplayName =>
            $"{AcademicYear} - {DepartmentName} - {ClassName} - {SectionName}";
    }

    /* =========================
       VIEW-ONLY ROUTINE
       ========================= */
    public class TimetableViewVM
    {
        public string AcademicYear { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;

        public int DepartmentId { get; set; }     
        public int ClassId { get; set; }           
        public int SectionId { get; set; }

        public List<string> Days { get; set; } = new();
        public List<PeriodVM> Periods { get; set; } = new();

        public Dictionary<string, Dictionary<string, TimetableCellVM>> Grid { get; set; }
            = new Dictionary<string, Dictionary<string, TimetableCellVM>>();
    }

    public class TimetableCellVM
    {
        public string? SubjectName { get; set; }
        public string? TeacherName { get; set; }

        public bool IsBreak { get; set; }
        public bool IsAssembly { get; set; }

        public string DisplayText =>
            IsBreak ? "BREAK"
            : IsAssembly ? "ASSEMBLY"
            : (!string.IsNullOrEmpty(SubjectName) ? $"{SubjectName}\n{TeacherName}" : "FREE");
    }
}
public class PeriodBlueprintVM
{
    public static List<PeriodVM> DailyPeriods => new()
    {
        new PeriodVM
        {
           PeriodName = "Assembly", StartTime = "08:00", EndTime = "08:30", IsAssembly = true
        },
        new PeriodVM
        {
           PeriodName = "1st", StartTime = "08:30", EndTime = "09:20"
        },
        new PeriodVM
        {
            PeriodName = "2nd", StartTime = "09:20", EndTime = "10:05"
        },
        new PeriodVM
        {
           PeriodName = "3rd", StartTime = "10:05", EndTime = "10:50"
        },
        new PeriodVM
        {
           PeriodName = "4th", StartTime = "10:50", EndTime = "11:35"
        },
        new PeriodVM
        {
           PeriodName = "BREAK", StartTime = "11:35", EndTime = "12:00", IsBreak = true
        },
        new PeriodVM
        {
            PeriodName = "5th", StartTime = "12:00", EndTime = "12:45"
        },
        new PeriodVM
        {
            PeriodName = "6th", StartTime = "12:45", EndTime = "01:30"
        }
    };
}
