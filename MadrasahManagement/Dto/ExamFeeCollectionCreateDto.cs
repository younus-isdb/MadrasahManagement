using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MadrasahManagement.Dto
{
    public class ExamFeeCollectionCreateDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ExamFee { get; set; }

        public string TotalSubject { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string EducationYear { get; set; } = string.Empty;
    }

    public class ExamFeesCreateDto
    {
        [Required, MaxLength(10)]
        public string EducationYear { get; set; } = string.Empty;

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public decimal ExamAmount { get; set; }

        public List<ExamFeeCollectionCreateDto> FeeCollections { get; set; } = new();
    }
    public class ExamFeeCollectionReadDto
    {
        public int FeeCollectionId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int ExamFee { get; set; }
        public string TotalSubject { get; set; } = string.Empty;
        public string EducationYear { get; set; } = string.Empty;
    }

    public class ExamFeesReadDto
    {
        public int ExamFeeId { get; set; }
        public string EducationYear { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public decimal ExamAmount { get; set; }

        public List<ExamFeeCollectionReadDto> FeeCollections { get; set; } = new();
    }
}
