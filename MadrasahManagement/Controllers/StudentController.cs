using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;

namespace MadrasahManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly MadrasahDbContext _context;
        private readonly IWebHostEnvironment _env;

        public StudentController(MadrasahDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ---------------------------------------------------
        // GET: Student/Create
        // ---------------------------------------------------
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        // ---------------------------------------------------
        // POST: Student/Create
        // ---------------------------------------------------
        	[HttpPost]
			[ValidateAntiForgeryToken]
			public async Task<IActionResult> Create(Student model, IFormFile ProfileImage, IFormFile DocumentFile)
			{
				// ⭐ Auto UserId Set
				model.UserId = Guid.NewGuid().ToString();

			
				if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            // Auto Roll No
            model.RegNo = await GenerateRegNo(model.ClassId, model.SectionId);

            _context.Students.Add(model);
            await _context.SaveChangesAsync(); // প্রথমে StudentId পাবার জন্য সেভ

            // Profile Image Upload
            if (ProfileImage != null)
            {
                model.ProfileImageUrl = await SaveFile(ProfileImage, "photos/students", model.StudentId + ".jpg");
                await _context.SaveChangesAsync();
            }

            // Document Upload
            if (DocumentFile != null)
            {
                model.DocumentUrl = await SaveFile(DocumentFile, "documents/students", model.StudentId + "_doc.pdf");
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = model.StudentId });
        }

        // ---------------------------------------------------
        // AJAX: Get Classes by Department
        // ---------------------------------------------------
        public async Task<IActionResult> GetClasses(int departmentId)
        {
            var classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId)
                .Select(c => new { classId = c.ClassId, className = c.ClassName })
                .ToListAsync();

            return Json(classes);
        }

        // ---------------------------------------------------
        // AJAX: Get Sections by Class
        // ---------------------------------------------------
        public async Task<IActionResult> GetSections(int classId)
        {
            var sections = await _context.Sections
                .Where(s => s.ClassId == classId)
                .Select(s => new { sectionId = s.SectionId, sectionName = s.SectionName })
                .ToListAsync();

            return Json(sections);
        }

        // ---------------------------------------------------
        // Load Dropdown ViewBags
        // ---------------------------------------------------
        private async Task LoadDropdowns()
        {
            ViewBag.DepartmentList = new SelectList(
                await _context.Departments.ToListAsync(),
                "DepartmentId",
                "DepartmentName");

            ViewBag.ClassList = new SelectList(
                await _context.Classes.ToListAsync(),
                "ClassId",
                "ClassName");

            ViewBag.SectionList = new SelectList(
                await _context.Sections.ToListAsync(),
                "SectionId",
                "SectionName");
        }

        // ---------------------------------------------------
        // Auto Roll Generator
        // ---------------------------------------------------
        private async Task<string> GenerateRegNo(int classId, int sectionId)
        {
            int count = await _context.Students
                .CountAsync(s => s.ClassId == classId && s.SectionId == sectionId);

            return (count + 1).ToString("D3"); // 001, 002, 003...
        }

        // ---------------------------------------------------
        // File Upload Helper
        // ---------------------------------------------------
        private async Task<string> SaveFile(IFormFile file, string folderName, string fileName)
        {
            string folderPath = Path.Combine(_env.WebRootPath, folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderName}/{fileName}";
        }

        // ---------------------------------------------------
        // GET: Student/Details/5
        // ---------------------------------------------------
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.Class)
                .Include(s => s.Section)
                .FirstOrDefaultAsync(m => m.StudentId == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        // ---------------------------------------------------
        // GET: Student/Index
        // ---------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.Class)
                .Include(s => s.Section)
                .ToListAsync();

            return View(students);
        }
    }
}
