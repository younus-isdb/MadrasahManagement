using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MadrasahManagement.Dto
{
    // ------------------- CREATE DTO -------------------
    public class ExamFeeCollectionCreateDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        [Range(0, 50000)]
        public decimal ExamFeeAmount { get; set; }

        public int TotalSubject { get; set; }
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
        [Range(0, 100000)]
        public decimal ExamAmount { get; set; }

        public List<ExamFeeCollectionCreateDto> FeeCollections { get; set; } = new();
    }

    // ------------------- UPDATE DTO -------------------
    public class ExamFeeCollectionUpdateDto
    {
        public int? FeeCollectionId { get; set; } // null = new

        [Required]
        public int StudentId { get; set; }

        [Required]
        public decimal ExamFeeAmount { get; set; }

        public int TotalSubject { get; set; }
    }

    public class ExamFeesUpdateDto
    {
        [Required, MaxLength(10)]
        public string EducationYear { get; set; } = string.Empty;

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public decimal ExamAmount { get; set; }

        public List<ExamFeeCollectionUpdateDto> FeeCollections { get; set; } = new();
    }

    // ------------------- READ DTO -------------------
    public class ExamFeeCollectionReadDto
    {
        public int FeeCollectionId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public decimal ExamFeeAmount { get; set; }
        public int TotalSubject { get; set; }
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
