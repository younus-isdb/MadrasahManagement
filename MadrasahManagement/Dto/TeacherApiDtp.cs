using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    public class TeacherApiDtp
    {
       
            [Required] public string Name { get; set; } = default!;
            [Required][EmailAddress] public string Email { get; set; } = default!;
            [Required] public string Contact { get; set; } = default!;
            [Required] public int DepartmentId { get; set; }
            [Required] public DateTime JoiningDate { get; set; }
            public string? Qualification { get; set; }
            public string? Designation { get; set; }
        
    }
}
