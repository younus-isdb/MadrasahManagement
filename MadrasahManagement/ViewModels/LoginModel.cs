using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    public class LoginModel
    {
       // [EmailAddress]
        [StringLength(50)]
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; } = default!;

        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 4)]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; } = default!;

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; } = "/";


    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }
    }

}
