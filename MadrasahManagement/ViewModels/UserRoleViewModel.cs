namespace MadrasahManagement.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Roles { get; set; } = new();

        // Future proof:
        public string? FullName { get; set; }
        public bool IsActive { get; set; }
    }

}
