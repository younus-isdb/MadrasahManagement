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
        private readonly RoleManager<AppRole> _roleManager; // ✅ AppRole instead of IdentityRole

        public UserRoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ✅ Show all users with assigned roles
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();

            var model = users.Select(u => new UserRoleViewModel
            {
                UserId = u.Id, // int or string depending on model
                Email = u.Email!,
                Roles = _userManager.GetRolesAsync(u).Result.ToList()
            }).ToList();

            return View(model);
        }

        // GET: UserRole/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = new List<string>(await _userManager.GetRolesAsync(user))
            };

            // ✅ Make sure AllRoles is not null
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.AllRoles = roles.Select(r => r.Name).ToList();

            return View(model);
        }

        // POST: UserRole/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            // Roles to add
            var rolesToAdd = model.Roles.Except(userRoles);
            // Roles to remove
            var rolesToRemove = userRoles.Except(model.Roles);

            await _userManager.AddToRolesAsync(user, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            return RedirectToAction(nameof(Index));
        }

    }
}
