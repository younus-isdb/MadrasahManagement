using MadrasahManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    public class ClassCreateViewModel
    {
        [Required(ErrorMessage = "Class name is required")]
        [Display(Name = "Class Name")]
        [MaxLength(100)]
        public string ClassName { get; set; } = default!;

        [Display(Name = "Session Year")]
        [MaxLength(9)]
        public string? SessionYear { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public List<Subject> Subjects { get; set; } = new List<Subject>();
    }

    public class ClassEditViewModel
    {
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Class name is required")]
        [Display(Name = "Class Name")]
        [MaxLength(100)]
        public string ClassName { get; set; } = default!;

        [Display(Name = "Session Year")]
        [MaxLength(9)]
        public string? SessionYear { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public List<Subject> Subjects { get; set; } = new List<Subject>();
    }

    public class AddSubjectsViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = default!;
        public string DepartmentName { get; set; } = default!;
        public List<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
