using MadrasahManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly MadrasahDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public StudentController(
            MadrasahDbContext context,
            UserManager<AppUser> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }


        // ---------------------------------------------------
        // GET: Student/Create
        // ---------------------------------------------------
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }


        // ---------------------------------------------------
        // POST: Student/Create
        // ---------------------------------------------------
        //     [HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Student model, IFormFile ProfileImage, IFormFile DocumentFile)
        //{
        //	// ⭐ Auto UserId Set
        //	model.UserId = Guid.NewGuid().ToString();


        //	if (!ModelState.IsValid)
        //         {
        //             await LoadDropdowns();
        //             return View(model);
        //         }

        //         // Auto Roll No
        //         model.RegNo = await GenerateRegNo(model.ClassId, model.SectionId);

        //         _context.Students.Add(model);
        //         await _context.SaveChangesAsync(); // প্রথমে StudentId পাবার জন্য সেভ

        //         // Profile Image Upload
        //         if (ProfileImage != null)
        //         {
        //             model.ProfileImageUrl = await SaveFile(ProfileImage, "photos/students", model.StudentId + ".jpg");
        //             await _context.SaveChangesAsync();
        //         }

        //         // Document Upload
        //         if (DocumentFile != null)
        //         {
        //             model.DocumentUrl = await SaveFile(DocumentFile, "documents/students", model.StudentId + "_doc.pdf");
        //             await _context.SaveChangesAsync();
        //         }

        //         return RedirectToAction("Details", new { id = model.StudentId });
        //     }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(vm);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var student = new Student
            {
                UserId = user.Id,
                StudentName = vm.StudentName,
                ArabicStudentName = vm.ArabicStudentName,
                BanglaStudentName = vm.BanglaStudentName,

                DepartmentId = vm.DepartmentId,
                ClassId = vm.ClassId,
                SectionId = vm.SectionId,

                RegNo = vm.RegNo,
                NationalId = vm.NationalId,
                AdmissionDate = DateOnly.FromDateTime(vm.AdmissionDate),

                Gender = vm.Gender,
                DOB = vm.DOB,
                BloodGroup = vm.BloodGroup,

                FatherName = vm.FatherName,
                FatherPhone = vm.FatherPhone,
                MotherName = vm.MotherName,
                MotherPhone = vm.MotherPhone,

                GuardianName = vm.GuardianName,
                GuardianPhone = vm.GuardianPhone,
                GuardianEmail = vm.GuardianEmail,

                Address = vm.Address,
                City = vm.City,
                Country = vm.Country,

                EmergencyContactName = vm.EmergencyContactName,
                EmergencyPhone = vm.EmergencyPhone,
                MedicalNotes = vm.MedicalNotes,

                PreviousSchoolName = vm.PreviousSchoolName,
                PreviousResult = vm.PreviousResult,

                IsActive = vm.IsActive,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // 🔹 Profile Image
            if (vm.ProfileImage != null)
                student.ProfileImageUrl = await SaveFile(vm.ProfileImage, "students");

            // 🔹 Document
            if (vm.DocumentFile != null)
                student.DocumentUrl = await SaveFile(vm.DocumentFile, "documents");

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            var path = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(path);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folder}/{fileName}";
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
        public IActionResult Index()
        {
            var students = _context.Students
                .Include(s => s.Department)
                .Include(s => s.Class)
                .Include(s => s.Section)
                .AsNoTracking()
                .ToList();

            return View(students);
        }






        public IActionResult Edit(int id)
        {
            var s = _context.Students.Find(id);
            if (s == null) return NotFound();

            var vm = new StudentEditVM
            {
                StudentId = s.StudentId,
                StudentName = s.StudentName,
                ArabicStudentName = s.ArabicStudentName,
                BanglaStudentName = s.BanglaStudentName,

                DepartmentId = s.DepartmentId,
                ClassId = s.ClassId,
                SectionId = s.SectionId,

                RegNo = s.RegNo,
                NationalId = s.NationalId,
                AdmissionDate = s.AdmissionDate.ToDateTime(TimeOnly.MinValue),

                Gender = s.Gender,
                DOB = s.DOB,
                BloodGroup = s.BloodGroup,

                FatherName = s.FatherName,
                FatherPhone = s.FatherPhone,
                MotherName = s.MotherName,
                MotherPhone = s.MotherPhone,

                GuardianName = s.GuardianName,
                GuardianPhone = s.GuardianPhone,
                GuardianEmail = s.GuardianEmail,

                Address = s.Address,
                City = s.City,
                Country = s.Country,

                EmergencyContactName = s.EmergencyContactName,
                EmergencyPhone = s.EmergencyPhone,
                MedicalNotes = s.MedicalNotes,

                PreviousSchoolName = s.PreviousSchoolName,
                PreviousResult = s.PreviousResult,

                ExistingProfileImageUrl = s.ProfileImageUrl,
                ExistingDocumentUrl = s.DocumentUrl,

                IsActive = s.IsActive,
                LeavingDate = s.LeavingDate?.DateTime,
                LeavingReason = s.LeavingReason
            };

            LoadDropdowns(s.DepartmentId, s.ClassId);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentEditVM vm)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(vm.DepartmentId, vm.ClassId);
                return View(vm);
            }

            var student = await _context.Students.FindAsync(vm.StudentId);
            if (student == null) return NotFound();

            student.StudentName = vm.StudentName;
            student.ArabicStudentName = vm.ArabicStudentName;
            student.BanglaStudentName = vm.BanglaStudentName;

            student.DepartmentId = vm.DepartmentId;
            student.ClassId = vm.ClassId;
            student.SectionId = vm.SectionId;

            student.RegNo = vm.RegNo;
            student.NationalId = vm.NationalId;
            student.AdmissionDate = DateOnly.FromDateTime(vm.AdmissionDate);

            student.Gender = vm.Gender;
            student.DOB = vm.DOB;
            student.BloodGroup = vm.BloodGroup;

            student.FatherName = vm.FatherName;
            student.FatherPhone = vm.FatherPhone;
            student.MotherName = vm.MotherName;
            student.MotherPhone = vm.MotherPhone;

            student.GuardianName = vm.GuardianName;
            student.GuardianPhone = vm.GuardianPhone;
            student.GuardianEmail = vm.GuardianEmail;

            student.Address = vm.Address;
            student.City = vm.City;
            student.Country = vm.Country;

            student.EmergencyContactName = vm.EmergencyContactName;
            student.EmergencyPhone = vm.EmergencyPhone;
            student.MedicalNotes = vm.MedicalNotes;

            student.PreviousSchoolName = vm.PreviousSchoolName;
            student.PreviousResult = vm.PreviousResult;

            student.IsActive = vm.IsActive;
            student.LeavingDate = vm.LeavingDate;

            // 🔹 File replace (optional)
            if (vm.ProfileImage != null)
                student.ProfileImageUrl = await SaveFile(vm.ProfileImage, "students");

            if (vm.DocumentFile != null)
                student.DocumentUrl = await SaveFile(vm.DocumentFile, "documents");

            student.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            student.IsActive = false;
            student.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private void LoadDropdowns(int? departmentId = null, int? classId = null)
        //{
        //    ViewBag.DepartmentList = new SelectList(
        //        _context.Departments.Where(d => d.IsActive),
        //        "DepartmentId",
        //        "DepartmentName",
        //        departmentId);

        //    ViewBag.ClassList = new SelectList(
        //        _context.Classes.Where(c => c.IsActive &&
        //            (!departmentId.HasValue || c.DepartmentId == departmentId)),
        //        "ClassId",
        //        "ClassName",
        //        classId);

        //    ViewBag.SectionList = new SelectList(
        //        _context.Sections.Where(s => s.IsActive &&
        //            (!classId.HasValue || s.ClassId == classId)),
        //        "SectionId",
        //        "SectionName");
        //}
        private void LoadDropdowns(int? departmentId = null, int? classId = null)
        {
            ViewBag.DepartmentList = new SelectList(
                _context.Departments,
                "DepartmentId",
                "DepartmentName",
                departmentId);

            ViewBag.ClassList = new SelectList(
                _context.Classes.Where(c =>
                    !departmentId.HasValue || c.DepartmentId == departmentId),
                "ClassId",
                "ClassName",
                classId);

            ViewBag.SectionList = new SelectList(
                _context.Sections.Where(s =>
                    !classId.HasValue || s.ClassId == classId),
                "SectionId",
                "SectionName");
        }


    }
}
