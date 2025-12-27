using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    // CREATE DTO
    public class SectionCreateDto
    {
        [Required]
        public int ClassId { get; set; }

        [Required, MaxLength(50)]
        public string SectionName { get; set; } = string.Empty;
    }

    // UPDATE DTO
    public class SectionUpdateDto : SectionCreateDto
    {
        [Required]
        public int SectionId { get; set; }
    }

    // READ DTO
    public class SectionReadDto
    {
        public int SectionId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
    }
}
