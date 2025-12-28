using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public StudentsController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _context.Students
                .Include(s => s.Class)
                .Include(s => s.Department)
                .Include(s => s.Section)
                .Select(s => new StudentReadDto
                {
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    ArabicStudentName = s.ArabicStudentName,
                    BanglaStudentName = s.BanglaStudentName,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.DepartmentName,
                    ClassId = s.ClassId,
                    ClassName = s.Class.ClassName,
                    SectionId = s.SectionId,
                    SectionName = s.Section.SectionName,
                    RegNo = s.RegNo,
                    NationalId = s.NationalId,
                    AdmissionDate = s.AdmissionDate,
                    Gender = (int?)s.Gender,
                    DOB = s.DOB,
                    BloodGroup = s.BloodGroup,
                    IsActive = s.IsActive
                }).ToListAsync();

            return Ok(students);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _context.Students
                .Include(s => s.Class)
                .Include(s => s.Department)
                .Include(s => s.Section)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (s == null) return NotFound();

            var dto = new StudentReadDto
            {
                StudentId = s.StudentId,
                StudentName = s.StudentName,
                ArabicStudentName = s.ArabicStudentName,
                BanglaStudentName = s.BanglaStudentName,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department.DepartmentName,
                ClassId = s.ClassId,
                ClassName = s.Class.ClassName,
                SectionId = s.SectionId,
                SectionName = s.Section.SectionName,
                RegNo = s.RegNo,
                NationalId = s.NationalId,
                AdmissionDate = s.AdmissionDate,
                Gender = (int?)s.Gender,
                DOB = s.DOB,
                BloodGroup = s.BloodGroup,
                IsActive = s.IsActive
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var student = new Student
            {
                UserId = dto.UserId,
                StudentName = dto.StudentName,
                ArabicStudentName = dto.ArabicStudentName,
                BanglaStudentName = dto.BanglaStudentName,
                DepartmentId = dto.DepartmentId,
                ClassId = dto.ClassId,
                SectionId = dto.SectionId,
                RegNo = dto.RegNo,
                NationalId = dto.NationalId,
                AdmissionDate = dto.AdmissionDate ?? DateOnly.FromDateTime(DateTime.Today),
                Gender = (Gender?)dto.Gender,
                DOB = dto.DOB ?? DateTime.Today.AddYears(-5),
                BloodGroup = dto.BloodGroup,
                FatherName = dto.FatherName,
                FatherPhone = dto.FatherPhone,
                MotherName = dto.MotherName,
                MotherPhone = dto.MotherPhone,
                GuardianName = dto.GuardianName,
                GuardianPhone = dto.GuardianPhone,
                GuardianEmail = dto.GuardianEmail,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyPhone = dto.EmergencyPhone,
                MedicalNotes = dto.MedicalNotes,
                PreviousSchoolName = dto.PreviousSchoolName,
                PreviousResult = dto.PreviousResult,
                ProfileImageUrl = dto.ProfileImageUrl,
                DocumentUrl = dto.DocumentUrl,
                IsActive = dto.IsActive,
                LeavingDate = dto.LeavingDate,
                LeavingReason = dto.LeavingReason
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = student.StudentId }, student);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudentUpdateDto dto)
        {
            if (id != dto.StudentId) return BadRequest("Id mismatch");

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            student.StudentName = dto.StudentName;
            student.ArabicStudentName = dto.ArabicStudentName;
            student.BanglaStudentName = dto.BanglaStudentName;
            student.DepartmentId = dto.DepartmentId;
            student.ClassId = dto.ClassId;
            student.SectionId = dto.SectionId;
            student.RegNo = dto.RegNo;
            student.NationalId = dto.NationalId;
            student.AdmissionDate = dto.AdmissionDate ?? student.AdmissionDate;
            student.Gender = (Gender?)dto.Gender;
            student.DOB = dto.DOB ?? student.DOB;
            student.BloodGroup = dto.BloodGroup;
            student.FatherName = dto.FatherName;
            student.FatherPhone = dto.FatherPhone;
            student.MotherName = dto.MotherName;
            student.MotherPhone = dto.MotherPhone;
            student.GuardianName = dto.GuardianName;
            student.GuardianPhone = dto.GuardianPhone;
            student.GuardianEmail = dto.GuardianEmail;
            student.Address = dto.Address;
            student.City = dto.City;
            student.Country = dto.Country;
            student.EmergencyContactName = dto.EmergencyContactName;
            student.EmergencyPhone = dto.EmergencyPhone;
            student.MedicalNotes = dto.MedicalNotes;
            student.PreviousSchoolName = dto.PreviousSchoolName;
            student.PreviousResult = dto.PreviousResult;
            student.ProfileImageUrl = dto.ProfileImageUrl;
            student.DocumentUrl = dto.DocumentUrl;
            student.IsActive = dto.IsActive;
            student.LeavingDate = dto.LeavingDate;
            student.LeavingReason = dto.LeavingReason;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
