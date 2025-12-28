using MadrasahManagement.Models;
using MadrasahManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
		public async Task<IActionResult> Index()
		{
			var teachers = await _db.Teachers
				.Include(s => s.Department)
				.Include(s => s.AppUser)
				.ToListAsync();

			return View(teachers);
		}
	}
}
