using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    // Create DTO
    public class ClassCreateDto
    {
        [Required, MaxLength(100)]
        public string ClassName { get; set; } = string.Empty;

        [MaxLength(9)]
        public string? SessionYear { get; set; }

        [Required]
        public int DepartmentId { get; set; }
    }

    // Update DTO
    public class ClassUpdateDto : ClassCreateDto
    {
        [Required]
        public int ClassId { get; set; }
    }

    // Read DTO
    public class ClassReadDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string? SessionYear { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
