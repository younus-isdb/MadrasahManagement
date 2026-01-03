using MadrasahManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadrasahManagement.Controllers
{
    public class TeacherDashboard : Controller
    {
        private readonly MadrasahDbContext _db;

        public TeacherDashboard(MadrasahDbContext db)
        {
            _db = db; 
        }
        public IActionResult Index()
        {
            return View();
        }
        // Add this to TeacherController
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _db.Teachers
                .Include(t => t.Department)
               // .Include(t => t.ClassSubjects)
                .Include(t => t.Timetables)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

    }
}
