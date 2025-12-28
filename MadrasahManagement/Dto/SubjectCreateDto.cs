using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    // Create DTO
    public class SubjectCreateDto
    {
        [Required]
        public int ClassId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required, MaxLength(150)]
        public string SubjectName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string SubjectCode { get; set; } = string.Empty;

        public bool IsOptional { get; set; } = false;
    }

    // Update DTO
    public class SubjectUpdateDto : SubjectCreateDto
    {
        [Required]
        public int SubjectId { get; set; }
    }

    // Read DTO
    public class SubjectReadDto
    {
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public bool IsOptional { get; set; }
    }
}
