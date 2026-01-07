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
        private readonly ILogger<AccountController> _logger;

        public AccountController(
             MadrasahDbContext db,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
           ILogger<AccountController> logger)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }



        // =======================
        // Student Register
        // =======================

        [HttpGet]
        public IActionResult RegisterStudent()
        {
            return View(new StudentRegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent(StudentRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = _db.Students
                .FirstOrDefault(s => s.RegNo == model.RegNo && s.StudentName == model.StudentName);

            if (student == null)
            {
                ModelState.AddModelError("", "Student not found.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(student.UserId))
            {
                ModelState.AddModelError("", "Account already registered. Please login.");
                return View(model);
            }

            var username = $"{student.RegNo}@school.com";

            var user = new AppUser
            {
                UserName = username,
                Email = username,
                FullName = student.StudentName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync("Student"))
                await _roleManager.CreateAsync(new AppRole("Student"));

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var r in roles)
                await _userManager.RemoveFromRoleAsync(user, r);

            await _userManager.AddToRoleAsync(user, "Student");

            student.UserId = user.Id;
            _db.Update(student);
            await _db.SaveChangesAsync();

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "StudentDashboard");
        }



        // =======================
        // Student Login
        // =======================
        [HttpGet]
        public IActionResult LoginStudent()
        {
            return View(new StudentLoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> LoginStudent(StudentLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = _db.Students
                .FirstOrDefault(s => s.RegNo == model.RegNo && s.StudentName == model.StudentName);

            if (student == null || string.IsNullOrEmpty(student.UserId))
            {
                ModelState.AddModelError("", "Account not found. Please register first.");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(student.UserId);

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid password.");
                return View(model);
            }

            return RedirectToAction("Index", "StudentDashboard");
        }



        public async Task CreateStudentAccount(string studentRegNo, string studentName, string email)
        {
            // Find student by RegNo and Name
            var student = _db.Students.FirstOrDefault(s => s.RegNo == studentRegNo && s.StudentName == studentName);
            if (student == null)
                throw new Exception("Student not found");

            // Check if AppUser already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                throw new Exception("User already exists");

            // Create AppUser
            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = studentName,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, "Temporary@123"); // temp password

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign Student role
            if (!await _roleManager.RoleExistsAsync("Student"))
                await _roleManager.CreateAsync(new AppRole("Student"));

            await _userManager.AddToRoleAsync(user, "Student");

            // Link AppUser to Student
            student.UserId = user.Id;
            _db.Students.Update(student);
            await _db.SaveChangesAsync();

            // Optionally, send email with temporary password / reset link
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);
            Console.WriteLine($"Reset link for student: {resetLink}");
        }


        // =======================
        // Teacher Register
        // =======================
        //[HttpGet]
        //public IActionResult RegisterTeacher()
        //{
        //	return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> RegisterTeacher(RegisterTeacherViewModel model)
        //{
        //	if (!ModelState.IsValid) return View(model);

        //	var user = new AppUser
        //	{
        //		UserName = model.Email,
        //		Email = model.Email,
        //		FullName = model.FullName
        //	};

        //	var result = await _userManager.CreateAsync(user, model.Password);

        //	if (result.Succeeded)
        //	{
        //		await _userManager.AddToRoleAsync(user, "Teacher");
        //		await _signInManager.SignInAsync(user, false);

        //		return RedirectToAction("Index", "TeacherDashboard");
        //	}

        //	return View(model);
        //}


        // =======================
        // Admin Register
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

            // Check if user already exists with this email as UserName
            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName",
                    "Email already registered. Please login instead.");
                return View(model);
            }

            // Check if email is pre-registered in Teachers or Students table
            bool isPreRegistered = IsUserPreRegistered(model.UserName);

            if (!isPreRegistered)
            {
                ModelState.AddModelError("UserName",
                    "Email not found in our system. Please contact administration.");
                return View(model);
            }

            // Create user - UserName is the email
            var user = new AppUser
            {
                UserName = model.UserName,  // Email as UserName
                Email = model.UserName,     // Also store as Email
                EmailConfirmed = true       // Since admin already verified
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                string role = "Student"; // Default role

                // Check Teacher table
                var teacher = _db.Teachers.FirstOrDefault(t => t.Email == model.UserName);
                if (teacher != null)
                {
                    role = "Teacher";
                    // Link the teacher record with AppUser
                    teacher.UserId = user.Id;
                    _db.Teachers.Update(teacher);
                }
                //else
                //{
                //    // Check Student table
                //    var student = _db.Students.FirstOrDefault(s => s.Email == model.UserName);
                //    if (student != null)
                //    {
                //        role = "Student";
                //        // Link the student record with AppUser
                //        student.UserId = user.Id;
                //        _db.Students.Update(student);
                //    }
                //}

                // Save the UserId links
                await _db.SaveChangesAsync();

                // Ensure role exists
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new AppRole(role));
                }

                // Assign role
                await _userManager.AddToRoleAsync(user, role);

                // Auto Login
                await _signInManager.SignInAsync(user, isPersistent: false);

                return await RedirectUserByRole(user);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    var user = new AppUser
        //    {
        //        UserName = model.UserName,
        //        Email = model.UserName
        //    };

        //    var result = await _userManager.CreateAsync(user, model.Password);

        //    if (result.Succeeded)
        //    {
        //        // Ensure Student role exists
        //        if (!await _roleManager.RoleExistsAsync("Student"))
        //        {
        //            await _roleManager.CreateAsync(new AppRole("Student"));
        //        }

        //        // Default Role Assign
        //        await _userManager.AddToRoleAsync(user, "Student");

        //        // Auto Login
        //        await _signInManager.SignInAsync(user, isPersistent: false);

        //        return await RedirectUserByRole(user);
        //    }

        //    // Show errors
        //    foreach (var error in result.Errors)
        //        ModelState.AddModelError("", error.Description);

        //    return View(model);
        //}


        // =======================
        // LOGIN (GET)
        // =======================
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);


                if (user == null)
                {
                    var tempUser = await _userManager.FindByNameAsync(model.UserName);

                    if (tempUser != null && await _userManager.IsInRoleAsync(tempUser, "Admin"))
                    {
                        user = tempUser;
                    }
                    else
                    {
                        var isPreRegistered = IsUserPreRegistered(model.UserName);

                        if (isPreRegistered)
                            ModelState.AddModelError("", "Account not activated. Please register first.");
                        else
                            ModelState.AddModelError("", "Account not found. Please contact administration.");

                        return View(model);
                    }
                }

                // Try to login - use PasswordSignInAsync with UserName
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,      // Use the user's UserName (which is email)
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    // Update last login if you have that property
                    if (user.GetType().GetProperty("LastLogin") != null)
                    {
                        user.GetType().GetProperty("LastLogin").SetValue(user, DateTime.Now);
                        await _userManager.UpdateAsync(user);
                    }

                    return await RedirectUserByRole(user);
                }

                // Handle different failure cases
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Account locked. Try again later.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Login not allowed.");
                }
                else if (result.RequiresTwoFactor)
                {
                    ModelState.AddModelError("", "Two-factor authentication required.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {UserName}", model.UserName);
                ModelState.AddModelError("", "An error occurred during login.");
            }

            return View(model);
        }

        private bool IsUserPreRegistered(string email)
        {
            return _db.Teachers.Any(t => t.Email == email);
        }


        // =======================
        // ROLE-BASED REDIRECTION
        // =======================
        private async Task<IActionResult> RedirectUserByRole(AppUser user)
        {
            if (user == null)
                return RedirectToAction("Login");

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "AdminDashboard");

            if (roles.Contains("Teacher"))
            {
                return RedirectToAction("Index", "TeacherDashboard");
            }

            if (roles.Contains("Student"))
                return RedirectToAction("Index", "StudentDashboard");
            if (roles.Contains("SuperAdmin"))
                return RedirectToAction("Index", "AdminDashboard");

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

        //ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // ইউজার না থাকলেও success দেখাও (security best practice)
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account",
                new { email = user.Email, token = token }, Request.Scheme);

            // এখানে email service দিয়ে resetLink পাঠাবে
            Console.WriteLine(resetLink);

            return RedirectToAction("ForgotPasswordConfirmation");
        }

    }
}
