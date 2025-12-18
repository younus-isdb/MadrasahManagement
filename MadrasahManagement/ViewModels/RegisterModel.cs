using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    public class RegisterModel
    {
        [EmailAddress]
        [StringLength(50)]
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; } = default!;

        [Required]
        public string Role { get; set; }

        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 4)]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; } = default!;


        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = default!;
        
        public string ReturnUrl { get; set; } = "/";


    }
}
