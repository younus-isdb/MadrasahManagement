using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    public class ExaminationCreateDto
    {
        [Required, MaxLength(150)]
        public string ExamName { get; set; } = string.Empty;
    }
    public class ExaminationUpdateDto
    {
        [Required]
        public int ExamId { get; set; }

        [Required, MaxLength(150)]
        public string ExamName { get; set; } = string.Empty;
    }
    public class ExaminationReadDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;

        // Optional: just count instead of full collection
        public int ExamFeeCount { get; set; }
        public int PointConditionCount { get; set; }
        public int ExamRoutineCount { get; set; }
    }
}
