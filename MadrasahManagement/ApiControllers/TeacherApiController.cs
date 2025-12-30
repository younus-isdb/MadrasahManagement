using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using MadrasahManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ApiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TeacherApiController : ControllerBase
	{
		private readonly MadrasahDbContext _db;
		private readonly IUploadService _uploadService;
		private readonly UserManager<AppUser> _userManager;

		public TeacherApiController(MadrasahDbContext db, IUploadService uploadService, UserManager<AppUser> userManager)
		{
			_db = db;
			_uploadService = uploadService;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<ActionResult> GetAll()
		{

			var teachers = await _db.Teachers.Select(t => new
			{
				t.TeacherId,
				t.Name,
				t.Contact,
				t.ImageFile,
				t.Email,
				t.Designation
			})
				//  .Include(s => s.Department)
				//.Include(s => s.AppUser)
				.ToListAsync();

			return Ok(teachers);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult> Get(int id)
		{

			var teachers = await _db.Teachers.FindAsync(id);
			if (teachers == null) return NotFound();


			return Ok(teachers);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] TeacherApiDtp model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// Check email
			if (await _userManager.FindByEmailAsync(model.Email) != null)
				return Conflict("Email already exists");

			// Create user
			var user = new AppUser
			{
				UserName = model.Email.Split('@')[0],
				Email = model.Email,
				PhoneNumber = model.Contact,
				EmailConfirmed = true
			};

			var result = await _userManager.CreateAsync(user);
			if (!result.Succeeded)
				return BadRequest(result.Errors);

			await _userManager.AddToRoleAsync(user, "Teacher");

			// Create teacher
			var teacher = new Teacher
			{
				Name = model.Name,
				Contact = model.Contact,
				Email = model.Email,
				UserId = user.Id,
				DepartmentId = model.DepartmentId,
				JoiningDate = new DateTimeOffset(model.JoiningDate),
				Qualification = model.Qualification,
				Designation = model.Designation,
				ImageUrl = model.ImageUrl
			};

			_db.Teachers.Add(teacher);
			await _db.SaveChangesAsync();

			return Ok(new
			{
				Success = true,
				TeacherId = teacher.TeacherId,
				Name = teacher.Name
			});
		}


		// PUT: api/TeacherApi/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> EditTeacher(int id, [FromBody] EditTeacherApiModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage)
						.ToList();

					return BadRequest(new
					{
						success = false,
						message = "Validation failed",
						errors = errors
					});
				}

				var teacher = await _db.Teachers
					.Include(t => t.AppUser)
					.FirstOrDefaultAsync(t => t.TeacherId == id);

				if (teacher == null)
				{
					return NotFound(new
					{
						success = false,
						message = "Teacher not found"
					});
				}

				// Check email if changed
				if (teacher.Email != model.Email)
				{
					var existingUser = await _userManager.FindByEmailAsync(model.Email);
					if (existingUser != null && existingUser.Id != teacher.AppUser?.Id)
					{
						return Conflict(new
						{
							success = false,
							message = "Email already registered by another user"
						});
					}
				}

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
							return BadRequest(new
							{
								success = false,
								message = "Failed to update user account",
								errors = updateResult.Errors.Select(e => e.Description).ToList()
							});
						}
					}
				}

				// Update teacher
				teacher.Name = model.Name;
				teacher.Email = model.Email;
				teacher.Contact = model.Contact;
				teacher.DepartmentId = model.DepartmentId;

				// Parse the date
				if (DateTime.TryParse(model.JoiningDate, out DateTime joiningDate))
				{
					teacher.JoiningDate = new DateTimeOffset(joiningDate);
				}

				teacher.Qualification = model.Qualification;
				teacher.Designation = model.Designation;

				await _db.SaveChangesAsync();

				return Ok(new
				{
					success = true,
					message = $"Teacher '{model.Name}' updated successfully!",
					data = new
					{
						teacherId = teacher.TeacherId,
						name = teacher.Name,
						email = teacher.Email,
						imageUrl = teacher.ImageUrl
					}
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					success = false,
					message = "Error updating teacher",
					error = ex.Message
				});
			}
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTeacher(int id)
		{
			// Find teacher first
			var teacher = await _db.Teachers.FindAsync(id);

			if (teacher == null)
			{
				return NotFound("Teacher not found");
			}

			try
			{
				// Check for active assignments BEFORE loading related data
				var hasAssignments = await _db.ClassSubjects
					.AnyAsync(cs => cs.TeacherId == id);

				if (hasAssignments)
				{
					return BadRequest("Cannot delete teacher with active class assignments");
				}

				// Get user separately
				var user = await _userManager.FindByIdAsync(teacher.UserId);

				// Delete image file
				if (!string.IsNullOrEmpty(teacher.ImageUrl))
				{
					await _uploadService.FileDelete(teacher.ImageUrl);
				}

				// Delete user account if exists
				if (user != null)
				{
					await _userManager.DeleteAsync(user);
				}

				// Mark teacher for deletion
				_db.Teachers.Remove(teacher);

				// Save changes
				var result = await _db.SaveChangesAsync();

				if (result > 0)
				{
					return Ok($"Teacher '{teacher.Name}' deleted successfully");
				}
				else
				{
					return StatusCode(500, "Failed to delete teacher from database");
				}
			}
			catch (DbUpdateConcurrencyException)
			{
				return Conflict("Teacher was modified or deleted by another user. Please refresh and try again.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error: {ex.Message}");
			}
		}


		[HttpPost("Upload")]
		public async Task<IActionResult> UploadTeacherImage(
   [FromServices] IUploadService upload,
   [FromForm] UploadFileModel input)
		{

			var result = await upload.FileSave(input.File);
			return Ok(result);
		}
	}
}

