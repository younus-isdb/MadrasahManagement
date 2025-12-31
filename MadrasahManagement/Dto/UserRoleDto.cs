using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    public class UserRoleDto
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
