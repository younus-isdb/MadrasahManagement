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
            await CreateUserAsync(userManager, roleManager, "superadmin@gmail.com", "SuperAdmin@123", "SuperAdmin");
            await CreateUserAsync(userManager, roleManager, "admin@gmail.com", "Admin@123", "Admin");
            await CreateUserAsync(userManager, roleManager, "teacher@gmail.com", "Teacher@123", "Teacher");
            await CreateUserAsync(userManager, roleManager, "student@gmail.com", "Student@123", "Student");

        }

        private static async Task CreateUserAsync(
     UserManager<AppUser> userManager,
     RoleManager<AppRole> roleManager,
     string email,
     string password,
     string role)
        {
            // 1️⃣ Ensure the role exists
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new AppRole(role));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to create role {role}: " +
                                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            // 2️⃣ Check if the user exists
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // User doesn't exist — create new
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    throw new Exception($"Failed to create user {email}: " +
                                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                // User exists — reset password
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await userManager.ResetPasswordAsync(user, token, password);

                if (!resetResult.Succeeded)
                {
                    throw new Exception($"Failed to reset password for user {email}: " +
                                        string.Join(", ", resetResult.Errors.Select(e => e.Description)));
                }
            }

            // 3️⃣ Assign role if not already assigned
            if (!await userManager.IsInRoleAsync(user, role))
            {
                var roleAssignResult = await userManager.AddToRoleAsync(user, role);
                if (!roleAssignResult.Succeeded)
                {
                    throw new Exception($"Failed to assign role {role} to user {email}: " +
                                        string.Join(", ", roleAssignResult.Errors.Select(e => e.Description)));
                }
            }
        }

    }
}
