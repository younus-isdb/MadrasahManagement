using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    public class ExamFeeCreateDto
    {
        [Required, MaxLength(10)]
        public string EducationYear { get; set; } = string.Empty;

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public decimal ExamAmount { get; set; }

    }
    public class ExamFeeUpdateDto
    {
        [Required]
        public int ExamFeeId { get; set; }

        [Required, MaxLength(10)]
        public string EducationYear { get; set; } = string.Empty;

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public decimal ExamAmount { get; set; }
    }
    namespace MadrasahManagement.Dto
    {
        public class ExamFeeReadDto
        {
            public int ExamFeeId { get; set; }
            public string EducationYear { get; set; } = string.Empty;
            public int ClassId { get; set; }
            public string ClassName { get; set; } = string.Empty;
            public int ExamId { get; set; }
            public string ExamName { get; set; } = string.Empty;
            public decimal ExamAmount { get; set; }
        }
    }

}
