using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MadrasahManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;

        // 🔥 Constructor Injection (এটাই ছিল missing)
        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // =======================
        // REGISTER (GET)
        // =======================
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }


        // =======================
        // REGISTER (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Ensure Student role exists
                if (!await _roleManager.RoleExistsAsync("Student"))
                {
                    await _roleManager.CreateAsync(new AppRole("Student"));
                }

                // Default Role Assign
                await _userManager.AddToRoleAsync(user, "Student");

                // Auto Login
                await _signInManager.SignInAsync(user, isPersistent: false);

                return await RedirectUserByRole(user);
            }

            // Show errors
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


        // =======================
        // LOGIN (GET)
        // =======================
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        // =======================
        // LOGIN (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                false,
                false
            );

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.UserName);
                return await RedirectUserByRole(user);
            }

            ModelState.AddModelError("", "Invalid Login Attempt");
            return View(model);
        }

        // =======================
        // ROLE-BASED REDIRECTION
        // =======================
        private async Task<IActionResult> RedirectUserByRole(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "AdminDashboard");

            if (roles.Contains("Teacher"))
                return RedirectToAction("Index", "TeacherDashboard");

            if (roles.Contains("Student"))
                return RedirectToAction("Index", "StudentDashboard");

            // Default fallback
            return RedirectToAction("Index", "Home");
        }

        // =======================
        // LOGOUT
        // =======================
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}
