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

        ////Get: TeacherController
        //public async Task<IActionResult> Index()
        //{
        //    var teachers = await _db.Teachers
        //       // .Include(s => s.Department)
        //      //  .Include(s => s.AppUser)
        //        .ToListAsync();

        //    return View(teachers);
        //}

        public async Task<IActionResult> Index(int? departmentId)
        {
            // Eager loading of Department
            var query = _db.Teachers
                .Include(t => t.Department)  // Include Department navigation property
                .AsQueryable();

            // Apply filter if departmentId is provided
            if (departmentId.HasValue)
            {
                query = query.Where(t => t.DepartmentId == departmentId.Value);
                ViewBag.SelectedDepartmentId = departmentId.Value;
            }

            // Get all departments for dropdown
            var allDepartments = await _db.Departments
                .Select(d => new { d.DepartmentId, d.DepartmentName })
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            ViewBag.Departments = allDepartments;

            var teachers = await query
                .OrderBy(t => t.Department.DepartmentName)
                .ThenBy(t => t.Name)
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
            var teachers = await _db.Teachers.Include(a => a.Department).Include(a => a.ClassSubjects).ThenInclude(c=>c.Class).Include(a => a.ClassSubjects).ThenInclude(c=>c.Subject).Include(a => a.Salaries).Include(a => a.Assignments).Include(a => a.MarkedAttendances).Include(a => a.TeacherAttendances).FirstOrDefaultAsync(a => a.TeacherId == id);

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

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeacherVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await GetDepartmentsAsync();
                return View(model);
            }

            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    ViewBag.Departments = await GetDepartmentsAsync();
                    return View(model);
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Sequence contains more than one element"))
            {
                // Handle duplicate email scenario
                ModelState.AddModelError("Email", "Email already registered. (Duplicate found)");
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

        [Authorize(Roles = "Admin,SuperAdmin")]
        // GET: Teacher/Edit/5
        public async Task<IActionResult> Edit(int id)
		{
			var teacher = await _db.Teachers
				.Include(t => t.AppUser)
				.FirstOrDefaultAsync(t => t.TeacherId == id);

			if (teacher == null)
			{
				return NotFound();
			}

			var model = new EditTeacherVM
			{
				TeacherId = teacher.TeacherId,
				Name = teacher.Name,
				Email = teacher.Email ?? "",
				Contact = teacher.Contact,
				DepartmentId = teacher.DepartmentId,
				JoiningDate = teacher.JoiningDate.DateTime, // Convert to DateTime
				Qualification = teacher.Qualification,
				Designation = teacher.Designation,
				ExistingImageUrl = teacher.ImageUrl
			};

			ViewBag.Departments = await GetDepartmentsAsync();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, EditTeacherVM model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					ViewBag.Departments = await GetDepartmentsAsync();
					return View(model);
				}

				var teacher = await _db.Teachers
					.Include(t => t.AppUser)
					.FirstOrDefaultAsync(t => t.TeacherId == id);

				if (teacher == null)
				{
					TempData["Error"] = "Teacher not found.";
					return RedirectToAction(nameof(Index));
				}

				// Check email if changed
				if (teacher.Email != model.Email)
				{
					var existingUser = await _userManager.FindByEmailAsync(model.Email);
					if (existingUser != null && existingUser.Id != teacher.AppUser?.Id)
					{
						ModelState.AddModelError("Email", "This email is already registered.");
						ViewBag.Departments = await GetDepartmentsAsync();
						return View(model);
					}
				}

				using var transaction = await _db.Database.BeginTransactionAsync();

				try
				{
					// Handle image upload
					if (model.ImageFile != null && model.ImageFile.Length > 0)
					{
						// Delete old image if exists
						if (!string.IsNullOrEmpty(teacher.ImageUrl))
						{
							await _uploadService.FileDelete(teacher.ImageUrl);
						}

						// Save new image
						teacher.ImageUrl = await _uploadService.FileSave(model.ImageFile);
					}
					else if (model.RemoveImage && !string.IsNullOrEmpty(teacher.ImageUrl))
					{
						await _uploadService.FileDelete(teacher.ImageUrl);
						teacher.ImageUrl = null;
					}

					// Update AppUser if email changed
					if (teacher.AppUser != null)
					{
						bool userUpdated = false;

						if (teacher.AppUser.Email != model.Email)
						{
							teacher.AppUser.Email = model.Email;
							teacher.AppUser.UserName = model.Email.Split('@')[0];
							userUpdated = true;
						}

						if (teacher.AppUser.PhoneNumber != model.Contact)
						{
							teacher.AppUser.PhoneNumber = model.Contact;
							userUpdated = true;
						}

						if (userUpdated)
						{
							var updateResult = await _userManager.UpdateAsync(teacher.AppUser);
							if (!updateResult.Succeeded)
							{
								foreach (var error in updateResult.Errors)
								{
									ModelState.AddModelError("", error.Description);
								}
								ViewBag.Departments = await GetDepartmentsAsync();
								return View(model);
							}
						}
					}

					// Update teacher
					teacher.Name = model.Name;
					teacher.Email = model.Email;
					teacher.Contact = model.Contact;
					teacher.DepartmentId = model.DepartmentId;
					teacher.JoiningDate = new DateTimeOffset(model.JoiningDate);
					teacher.Qualification = model.Qualification;
					teacher.Designation = model.Designation;

					await _db.SaveChangesAsync();
					await transaction.CommitAsync();

					TempData["Success"] = $"Teacher '{model.Name}' updated successfully!";
					return RedirectToAction(nameof(Index));
				}
				catch
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Error updating teacher: {ex.Message}";
				ViewBag.Departments = await GetDepartmentsAsync();
				return View(model);
			}
		}
		



        // GET: Teacher/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _db.Teachers
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }


        // POST: Teacher/Delete/5
        [HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var teacher = await _db.Teachers
				.Include(t => t.AppUser)
				.FirstOrDefaultAsync(t => t.TeacherId == id);

			if (teacher == null)
			{
				TempData["Error"] = "Teacher not found.";
				return RedirectToAction(nameof(Index));
			}

			try
			{
				// Delete associated image file
				if (!string.IsNullOrEmpty(teacher.ImageUrl))
				{
					await _uploadService.FileDelete(teacher.ImageUrl);
				}

				// Delete AppUser if exists
				if (teacher.AppUser != null)
				{
					await _userManager.DeleteAsync(teacher.AppUser);
				}

				// Delete teacher
				_db.Teachers.Remove(teacher);
				await _db.SaveChangesAsync();

				TempData["Success"] = $"Teacher '{teacher.Name}' deleted successfully!";
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Error deleting teacher: {ex.Message}";
			}

			return RedirectToAction(nameof(Index));
		}


	}
}
