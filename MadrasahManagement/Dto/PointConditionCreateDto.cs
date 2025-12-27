using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    // Create / Update DTOs
    public class PointConditionCreateDto
    {
        [Required, MaxLength(10)]
        public string EducationYear { get; set; } = string.Empty;

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public int PassMarks { get; set; }

        [Required]
        public int HighestMark { get; set; }

        public List<PointConditionDetailCreateDto> Details { get; set; } = new();
    }

    public class PointConditionUpdateDto : PointConditionCreateDto
    {
        [Required]
        public int PointConditionId { get; set; }
    }

    public class PointConditionDetailCreateDto
    {
        [Required]
        public int FromMark { get; set; }

        [Required]
        public int ToMark { get; set; }

        [Required]
        public string Division { get; set; } = string.Empty;

        public bool IsSilverColor { get; set; } = false;
    }

    // Read DTOs
    public class PointConditionReadDto
    {
        public int PointConditionId { get; set; }
        public string EducationYear { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public int ExamId { get; set; }
        public int PassMarks { get; set; }
        public int HighestMark { get; set; }

        public List<PointConditionDetailReadDto> Details { get; set; } = new();
    }

    public class PointConditionDetailReadDto
    {
        public int PointConditionDetailId { get; set; }
        public int FromMark { get; set; }
        public int ToMark { get; set; }
        public string Division { get; set; } = string.Empty;
        public bool IsSilverColor { get; set; }
    }
}
