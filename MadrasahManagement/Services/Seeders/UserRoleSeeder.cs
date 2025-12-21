using MadrasahManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace MadrasahManagement.Services.Seeders
{
    public static class UserRoleSeeder
    {
        public static async Task SeedAsync(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            // ===========================
            // 1️⃣ Create Roles
            // ===========================
            string[] roles = { "SuperAdmin","Admin", "Teacher", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRole(role));
                }
            }


            // ===========================
            // 2️⃣ Default SuperAdmin Create
            // ===========================
            await CreateUserAsync(
                userManager,
                email: "superadmin@gmail.com",
                password: "SuperAdmin@123",
                role: "SuperAdmin"
            );

            // ===========================
            // 2️⃣ Default Admin Create
            // ===========================
            await CreateUserAsync(
                userManager,
                email: "admin@gmail.com",
                password: "Admin@123",
                role: "Admin"
            );

            // ===========================
            // 3️⃣ Default Teacher Create
            // ===========================
            await CreateUserAsync(
                userManager,
                email: "teacher@gmail.com",
                password: "Teacher@123",
                role: "Teacher"
            );

            // ===========================
            // 4️⃣ Default Student Create
            // ===========================
            await CreateUserAsync(
                userManager,
                email: "student@gmail.com",
                password: "Student@123",
                role: "Student"
            );
        }

        private static async Task CreateUserAsync(
            UserManager<AppUser> userManager,
            string email,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, password);
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
