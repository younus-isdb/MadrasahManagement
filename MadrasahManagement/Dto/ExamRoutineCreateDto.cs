using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    // Create DTO
    public class ExamRoutineCreateDto
    {
        [Required, MaxLength(10)]
        public string EducationYear { get; set; } = string.Empty;

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int ExamId { get; set; }

        public int SubjectId { get; set; }
        public int RoomNumber { get; set; }

        [Required]
        public DateTime ExamDate { get; set; }

        public string ExamDay { get; set; } = string.Empty;
        public string ExamStartTime { get; set; } = string.Empty;
        public string ExamEndTime { get; set; } = string.Empty;
    }

    // Update DTO
    public class ExamRoutineUpdateDto : ExamRoutineCreateDto
    {
        [Required]
        public int ExamRoutineId { get; set; }
    }

    // Read DTO
    public class ExamRoutineReadDto
    {
        public int ExamRoutineId { get; set; }
        public string EducationYear { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int RoomNumber { get; set; }
        public DateTime ExamDate { get; set; }
        public string ExamDay { get; set; } = string.Empty;
        public string ExamStartTime { get; set; } = string.Empty;
        public string ExamEndTime { get; set; } = string.Empty;
    }
}
