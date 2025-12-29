using MadrasahManagement.Models;
using MadrasahManagement.Services;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadrasahManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly MadrasahDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IUploadService _uploadService;

        public TeacherController(
            MadrasahDbContext db,
            UserManager<AppUser> userManager,
            IWebHostEnvironment env,
            IUploadService uploadService)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
            _uploadService = uploadService;
        }

        //Get: TeacherController
        public async Task<IActionResult> Index()
        {
            var teachers = await _db.Teachers
               // .Include(s => s.Department)
              //  .Include(s => s.AppUser)
                .ToListAsync();

            return View(teachers);
        }

        //Get: TeacherController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var teachers = await _db.Teachers.Include(a => a.Department).Include(a => a.ClassSubjects).Include(a => a.Salaries).Include(a => a.Assignments).Include(a => a.MarkedAttendances).Include(a => a.TeacherAttendances).FirstOrDefaultAsync(a => a.TeacherId == id);
            if (teachers == null) return NotFound();


            return View(teachers);


        }

        // GET: TeacherController/Create
        public IActionResult Create()
        {
            // Populate departments dropdown
            ViewBag.Departments = _db.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeacherVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await GetDepartmentsAsync();
                return View(model);
            }

            // Check email
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Email already registered.");
                ViewBag.Departments = await GetDepartmentsAsync();
                return View(model);
            }

            // Create user
            var appUser = new AppUser
            {
                UserName = model.Email.Split('@')[0],
                Email = model.Email,
                PhoneNumber = model.Contact,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(appUser);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => ModelState.AddModelError("", e.Description));
                ViewBag.Departments = await GetDepartmentsAsync();
                return View(model);
            }

            await _userManager.AddToRoleAsync(appUser, "Teacher");

            // Create teacher
            var teacher = new Teacher
            {
                Name = model.Name,
                Contact = model.Contact,
                Email = model.Email,
                UserId = appUser.Id,
                DepartmentId = model.DepartmentId,
                JoiningDate = new DateTimeOffset(model.JoiningDate),
                Qualification = model.Qualification,
                Designation = model.Designation,
                ImageUrl = model.ImageFile?.Length > 0
                    ? await _uploadService.FileSave(model.ImageFile)
                    : null
            };

            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Teacher {model.Name} created!";
            return RedirectToAction(nameof(Index));
        }
        private async Task<List<SelectListItem>> GetDepartmentsAsync()
        {
            return await _db.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .ToListAsync();
        }





























        //private string GenerateRandomPassword()
        //{
        //    // Simple random password generator
        //    const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
        //    var random = new Random();
        //    return new string(Enumerable.Repeat(chars,6)
        //        .Select(s => s[random.Next(s.Length)]).ToArray());
        //}



        //private async Task SendWelcomeEmail(string email, string name)
        //{
        //    try
        //    {
        //        var subject = "Welcome to School Management System";

        //        var body = $@"
        //Dear {name},

        //Your teacher account has been created successfully.

        //Login Details:
        //Email: {email}


        //Please login and change your password immediately.

        //Login URL: https://localhost:7113/Account/Login

        //Regards,
        //School Administration";

        //        Console.WriteLine($"Email sent to {email}");
        //    }
        //    catch
        //    {         
        //        Console.WriteLine($"Failed to send email to {email}");
        //    }
        //}

    }
}
