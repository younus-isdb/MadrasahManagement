using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    public class StudentRegisterViewModel
    {
        [Required] public string StudentName { get; set; }
        [Required] public string RegNo { get; set; }
        [Required][DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)][Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    // For Login
    public class StudentLoginViewModel
    {
        [Required(ErrorMessage = "Registration Number is required")]
        [Display(Name = "Registration Number")]
        public string RegNo { get; set; }

        [Required(ErrorMessage = "Student Name is required")]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

}
