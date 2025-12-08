using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
	public class RegisterTeacherViewModel
	{
		[Required, StringLength(50)]
		public string Email { get; set; }

		[Required, StringLength(50)]
		public string Password { get; set; }

		[Required, Compare("Password")]
		public string ConfirmPassword { get; set; }

		[Required]
		public string FullName { get; set; }

		public string Subject { get; set; }
	}

}
