using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    // Create DTO
    public class SubjectCreateDto
    {
        [Required(ErrorMessage = "Subject Name is required")]
        [StringLength(100, ErrorMessage = "Subject Name cannot exceed 100 characters")]
        public string SubjectName { get; set; }

        [Required(ErrorMessage = "Subject Code is required")]
        [StringLength(20, ErrorMessage = "Subject Code cannot exceed 20 characters")]
        public string SubjectCode { get; set; }

        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public bool IsOptional { get; set; }
    }

    // Update DTO
    public class SubjectUpdateDto 
    {
        [Required]
        public int SubjectId { get; set; }
       

        [Required]
        [Display(Name = "Subject Name")]
        [MaxLength(150)]
        public string SubjectName { get; set; } = default!;

        [Required]
        [Display(Name = "Subject Code")]
        [MaxLength(50)]
        [Remote("IsSubjectCodeUniqueEdit", "Subject", AdditionalFields = "SubjectId", ErrorMessage = "Subject Code already exists.")]
        public string SubjectCode { get; set; } = default!;

        [Required]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Is Optional Subject?")]
        public bool IsOptional { get; set; } = false;
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

    public class AssignedTeacherDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = default!;
        public string EmployeeId { get; set; } = default!;
        public string ClassName { get; set; } = default!;
    }

    public class ClassStudentCountDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = default!;
        public int StudentCount { get; set; }
    }
}
