using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    public class CreateTeacherVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact number is required")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; } = DateTime.Now;

        [Display(Name = "Qualification")]
        public string? Qualification { get; set; }

        [Display(Name = "Designation")]
        public string? Designation { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile? ImageFile { get; set; }

        //// Optional: If admin wants to set password
        //[DataType(DataType.Password)]
        //[Display(Name = "Password (Leave blank to auto-generate)")]
        //[MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        //public string? Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm Password")]
        //[Compare("Password", ErrorMessage = "Passwords do not match")]
        //public string? ConfirmPassword { get; set; }
    }
	public class EditTeacherVM
	{
		public int TeacherId { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; } = default!;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = default!;

		[Required]
		public string Contact { get; set; } = default!;

		[Required]
		public int DepartmentId { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime JoiningDate { get; set; }

		[StringLength(250)]
		public string? Qualification { get; set; }

		[StringLength(150)]
		public string? Designation { get; set; }

		[DisplayName("Profile Image")]
		public IFormFile? ImageFile { get; set; }

		public bool RemoveImage { get; set; }

		public string? ExistingImageUrl { get; set; }
	}
}
