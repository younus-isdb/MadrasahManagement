using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using MadrasahManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            _userManager=userManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {

            var teachers = await _db.Teachers.Select(t => new
            {
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
                Designation = model.Designation
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
    }
}

