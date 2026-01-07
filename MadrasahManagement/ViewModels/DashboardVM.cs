using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    public class TeacherProfileEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Contact { get; set; }

        public string Address { get; set; }

        [StringLength(500)]
        public string Introduction { get; set; }

        public string Qualification { get; set; }

        public string Designation { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public DateTimeOffset JoiningDate { get; set; }
        public string ImageUrl { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile ImageFile { get; set; }
    }
}
