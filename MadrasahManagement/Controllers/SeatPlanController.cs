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
        // ================= INDEX =================
        // ================= INDEX =================
        public IActionResult Index()
        {
            var list = _context.SeatPlans
                .Include(s => s.Student)
                .Include(s => s.Class)
                .Include(s => s.Department)
                .Include(s => s.Subject)
                .OrderBy(s => s.RoomNumber)
                .ThenBy(s => s.SeatPlanId)
                .Select(s => new SeatPlanReadDto
                {
                    SeatPlanId = s.SeatPlanId,
                    ExamDate = s.ExamDate,
                    RoomNumber = s.RoomNumber,
                    NumberOfRows = s.NumberOfRows,
                    StudentsPerBench = s.StudentsPerBench,

                    StudentId = s.StudentId,
                    RegNo = s.Student != null ? s.Student.RegNo : "",
                    StudentName = s.Student != null ? s.Student.StudentName : "",

                    ClassId = s.ClassId,
                    ClassName = s.Class != null ? s.Class.ClassName : "",

                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department != null ? s.Department.DepartmentName : "",

                    SubjectId = s.SubjectId ?? 0,
                    SubjectName = s.Subject != null ? s.Subject.SubjectName : ""
                })
                .ToList();

            return View(list);
        }

    }
}
