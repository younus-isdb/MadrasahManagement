using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    // Create DTO
    public class ExamFeeCollectionCreateDto
    {
        [Required, MaxLength(10)]
        public string EducationYear { get; set; } = string.Empty;

        [Required]
        public int ExamId { get; set; }

        [Required]
        public int ClassId { get; set; }

        public string TotalSubject { get; set; } = string.Empty;

        [Required]
        public int ExamFee { get; set; }

        [Required]
        public int StudentId { get; set; }
    }

    // Update DTO
    public class ExamFeeCollectionUpdateDto : ExamFeeCollectionCreateDto
    {
        [Required]
        public int FeeCollectionId { get; set; }
    }

    // Read DTO
    public class ExamFeeCollectionReadDto
    {
        public int FeeCollectionId { get; set; }
        public string EducationYear { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string TotalSubject { get; set; } = string.Empty;
        public int ExamFee { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
    }
}
