using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    public class RoutineSubjectViewModel
    {
        public int ExamRoutineId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;  // Add SubjectCode
        public string SubjectName { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; } = DateTime.Now;
        public string ExamDay { get; set; } = string.Empty;
        public string ExamStartTime { get; set; } = "09:00";
        public string ExamEndTime { get; set; } = "11:00";
       
    }

    public class ExamRoutineGroupViewModel
    {
        public string EducationYear { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int RoomNumber { get; set; }  // Move RoomNumber here
        public List<RoutineSubjectViewModel> Subjects { get; set; } = new();
    }

    public class ExamRoutineBatchViewModel
    {
        public int ExamRoutineId { get; set; }
        [Required(ErrorMessage = "Education Year is required")]
        public string EducationYear { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Class is required")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Exam is required")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "Room Number is required")]
        [Range(1, 999, ErrorMessage = "Room number must be between 1 and 999")]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }


        public List<RoutineSubjectViewModel> Subjects { get; set; } = new();
    }
}