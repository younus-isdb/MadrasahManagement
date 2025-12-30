using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    public class UserDto
    {



        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }

    }
}
