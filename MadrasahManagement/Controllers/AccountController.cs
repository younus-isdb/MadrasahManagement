using MadrasahManagement.Models;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly MadrasahDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AccountController(
            MadrasahDbContext db,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager)
        {
            _db = db;
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
        // REGISTER (POST) - SIMPLIFIED
        // =======================
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Clean input
            model.UserName = model.UserName?.Trim();

            // Check if student exists in database (created by admin)
            var student = await _db.Students
                .FirstOrDefaultAsync(s => s.StudentName == model.UserName);

            if (student == null)
            {
                ModelState.AddModelError("UserName",
                    "Student name not found. Contact administration.");
                return View(model);
            }

            // Check if already registered
            if (!string.IsNullOrEmpty(student.UserId))
            {
                ModelState.AddModelError("UserName",
                    "Already registered. Please login.");
                return View(model);
            }

            // Create user account with student name as username
            var user = new AppUser
            {
                UserName = student.StudentName,  // Exact student name
                Email = $"{student.RegNo}@student.edu", // Fake email for Identity
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Link student with user account
                student.UserId = user.Id;
                _db.Students.Update(student);
                await _db.SaveChangesAsync();

                // Add Student role
                if (!await _roleManager.RoleExistsAsync("Student"))
                    await _roleManager.CreateAsync(new AppRole("Student"));

                await _userManager.AddToRoleAsync(user, "Student");

                // Auto login after registration
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect to student dashboard
                return RedirectToAction("Index", "StudentDashboard");
            }

            // Show errors if registration failed
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
        // LOGIN (POST) - SIMPLIFIED
        // =======================
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Simple login - Identity will handle everything
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Find user and redirect based on role
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains("Student"))
                            return RedirectToAction("Index", "StudentDashboard");
                        else if (roles.Contains("Teacher"))
                            return RedirectToAction("Dashboard", "TeacherDashboard");
                        else if (roles.Contains("Admin"))
                            return RedirectToAction("Index", "AdminDashboard");
                    }

                    return RedirectToAction("Index", "Home");
                }

                // If login failed
                ModelState.AddModelError("", "Invalid username or password.");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Login error occurred.");
            }

            return View(model);
        }

        // =======================
        // LOGOUT
        // =======================
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // =======================
        // ADMIN REGISTER (Keep only if needed)
        // =======================
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterAdminViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return RedirectToAction("Index", "AdminDashboard");
            }

            return View(model);
        }
    }
}