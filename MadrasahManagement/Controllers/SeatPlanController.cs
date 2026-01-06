using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;
using MadrasahManagement.Dto;

namespace MadrasahManagement.Controllers
{
    public class SeatPlanController : Controller
    {
        private readonly MadrasahDbContext _context;

        public SeatPlanController(MadrasahDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var seatPlans = _context.SeatPlans
                .Include(s => s.Student)
                .Include(s => s.Class)
                .Include(s => s.Department)
                .Include(s => s.Subject)
                .AsNoTracking()
                .ToList(); // Age data niye ashte hobe grouping er jonno

            // DTO mapping and ordering
            var result = seatPlans.Select(s => new SeatPlanReadDto
            {
                SeatPlanId = s.SeatPlanId,
                ExamDate = s.ExamDate,
                RoomNumber = s.RoomNumber,
                NumberOfRows = s.NumberOfRows,
                StudentsPerBench = s.StudentsPerBench,
                StudentName = s.Student.StudentName,
                RegNo = s.Student.RegNo,
                ClassName = s.Class.ClassName,
                DepartmentName = s.Department.DepartmentName,
                SubjectName = s.Subject?.SubjectName ?? "N/A"
            })
            .OrderBy(s => s.ExamDate)
            .ThenBy(s => s.RoomNumber)
            .ToList();

            return View(result);
        }
        // ================= CREATE (GET) =================
        public IActionResult Create()
        {
            ViewBag.Students = _context.Students
                .Select(s => new
                {
                    studentId = s.StudentId,
                    regNo = s.RegNo,
                    studentName = s.StudentName
                }).ToList();

            ViewBag.Classes = _context.Classes
                .Select(c => new
                {
                    classId = c.ClassId,
                    className = c.ClassName
                }).ToList();

            ViewBag.Departments = _context.Departments
                .Select(d => new
                {
                    departmentId = d.DepartmentId,
                    departmentName = d.DepartmentName
                }).ToList();

            ViewBag.Subjects = _context.Subjects
                .Select(s => new
                {
                    subjectId = s.SubjectId,
                    subjectName = s.SubjectName
                }).ToList();

            return View();
        }

        // ================= CREATE (POST) =================
        [HttpPost]
        public IActionResult Create(SeatPlanCreateDto dto)
        {
            if (dto.StudentIds.Count == 0)
                return BadRequest("No students selected");

            for (int i = 0; i < dto.StudentIds.Count; i++)
            {
                var seat = new SeatPlan
                {
                    ExamDate = dto.ExamDate,
                    RoomNumber = dto.RoomNumber,
                    NumberOfRows = dto.NumberOfRows,
                    StudentsPerBench = dto.StudentsPerBench,

                    StudentId = dto.StudentIds[i],
                    ClassId = dto.ClassIds[i],
                    DepartmentId = dto.DepartmentIds[i],
                    SubjectId = dto.SubjectIds[i]
                };

                _context.SeatPlans.Add(seat);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        // ================= DELETE =================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var seat = _context.SeatPlans.Find(id);
            if (seat != null)
            {
                _context.SeatPlans.Remove(seat);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT (GET) =================
        public IActionResult Edit(int id)
        {
            var seat = _context.SeatPlans.Find(id);
            if (seat == null) return NotFound();

            // Dropdown গুলোর জন্য ডাটা পাঠানো
            ViewBag.Students = _context.Students.Select(s => new { studentId = s.StudentId, studentName = s.StudentName }).ToList();
            ViewBag.Classes = _context.Classes.Select(c => new { classId = c.ClassId, className = c.ClassName }).ToList();
            ViewBag.Departments = _context.Departments.Select(d => new { departmentId = d.DepartmentId, departmentName = d.DepartmentName }).ToList();
            ViewBag.Subjects = _context.Subjects.Select(s => new { subjectId = s.SubjectId, subjectName = s.SubjectName }).ToList();

            return View(seat);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        public IActionResult Edit(SeatPlan seat)
        {
            if (ModelState.IsValid)
            {
                _context.SeatPlans.Update(seat);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(seat);
        }
    }
}
