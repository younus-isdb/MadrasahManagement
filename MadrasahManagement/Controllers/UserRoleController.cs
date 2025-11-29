using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserRoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ----------------------------
        // Show all users with roles
        // ----------------------------
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var model = new List<UserRoleViewModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                model.Add(new UserRoleViewModel
                {
                    UserId = u.Id,
                    Email = u.Email!,
                    Roles = roles.ToList()
                });
            }

            return View("index", model); // AJAX Live View
        }

        // ----------------------------
        // AJAX: Assign Role
        // ----------------------------
        [HttpPost]
        public async Task<JsonResult> AssignRoleAjax(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Json(new { success = false, message = "User not found" });

            var userRoles = await _userManager.GetRolesAsync(user);

            // Only add if not already assigned
            if (!userRoles.Contains(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            // Return updated role list
            var updatedRoles = await _userManager.GetRolesAsync(user);
            return Json(new { success = true, roles = updatedRoles });
        }

        // ----------------------------
        // AJAX: Remove Role
        // ----------------------------
        [HttpPost]
        public async Task<JsonResult> RemoveRoleAjax(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Json(new { success = false, message = "User not found" });

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains(role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Return updated role list
            var updatedRoles = await _userManager.GetRolesAsync(user);
            return Json(new { success = true, roles = updatedRoles });
        }

        // ----------------------------
        // AJAX: Delete User
        // ----------------------------
        [HttpPost]
        public async Task<JsonResult> DeleteUserAjax(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Json(new { success = false, message = "User not found" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return Json(new { success = false, message = "Delete failed" });

            return Json(new { success = true });
        }

        // ----------------------------
        // Create User Page
        // ----------------------------
        public IActionResult CreateUser()
        {
            return View(); // Optional: Create User Form
        }
    }
}
