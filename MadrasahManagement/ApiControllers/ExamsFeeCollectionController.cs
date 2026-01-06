using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamFeesCollectionsController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExamFeesCollectionsController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET: api/ExamFeeCollections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamFeesReadDto>>> GetAll()
        {
            var examFees = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Department)
                .Include(e => e.Examination)
                .Include(e => e.FeeCollections)
                    .ThenInclude(fc => fc.Student)
                .Select(e => new ExamFeesReadDto
                {
                    ExamFeeId = e.ExamFeeId,
                    EducationYear = e.EducationYear,
                    DepartmentId = e.DepartmentId,
                    ClassId = e.ClassId,
                    ClassName = e.Class != null ? e.Class.ClassName : "Unknown",
                    ExamId = e.ExamId,
                    ExamName = e.Examination != null ? e.Examination.ExamName : "Unknown",
                    ExamAmount = e.ExamAmount,
                    FeeCollections = e.FeeCollections.Select(fc => new ExamFeeCollectionReadDto
                    {
                        FeeCollectionId = fc.FeeCollectionId,
                        StudentId = fc.StudentId,
                        StudentName = fc.Student != null ? fc.Student.StudentName : "Unknown",
                        ExamFeeAmount = fc.ExamFeeAmount,
                        TotalSubject = fc.TotalSubject
                    }).ToList()
                })
                .OrderByDescending(e => e.ExamFeeId)
                .ToListAsync();

            return Ok(examFees);
        }

        // POST: api/ExamFeeCollections
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ExamFeesCreateDto dto)
        {
            if (dto.FeeCollections == null)
                dto.FeeCollections = new List<ExamFeeCollectionCreateDto>();

            if (!dto.FeeCollections.Any())
                return BadRequest("At least one student fee collection is required.");

            if (dto.FeeCollections.Any(fc => fc.StudentId <= 0))
                return BadRequest("Please select a valid student for all rows.");

            // Check for duplicate students in this submission
            var studentIds = dto.FeeCollections.Select(fc => fc.StudentId).ToList();
            if (studentIds.Distinct().Count() != studentIds.Count)
                return BadRequest("Duplicate students are not allowed in the same collection.");

            // Check if students already have fee collections for this exam
            var existingCollections = await _context.ExamFeeCollections
                .Include(ec => ec.ExamFee)
                .Where(ec => ec.ExamFee.ClassId == dto.ClassId
                    && ec.ExamFee.ExamId == dto.ExamId
                    && ec.ExamFee.EducationYear == dto.EducationYear
                    && studentIds.Contains(ec.StudentId))
                .Select(ec => ec.StudentId)
                .ToListAsync();

            if (existingCollections.Any())
                return BadRequest($"Some students already have fee collections for this exam: {string.Join(", ", existingCollections)}");

            // Find or create ExamFee master record
            var examFee = await _context.ExamFees
                .FirstOrDefaultAsync(e => e.EducationYear == dto.EducationYear
                    && e.ClassId == dto.ClassId
                    && e.ExamId == dto.ExamId);

            if (examFee == null)
            {
                examFee = new ExamFee
                {
                    EducationYear = dto.EducationYear,
                    DepartmentId = dto.DepartmentId,
                    ClassId = dto.ClassId,
                    ExamId = dto.ExamId,
                    ExamAmount = dto.ExamAmount
                };
                _context.ExamFees.Add(examFee);
                await _context.SaveChangesAsync(); // Save to get ExamFeeId
            }
            else if (examFee.ExamAmount != dto.ExamAmount)
            {
                examFee.ExamAmount = dto.ExamAmount;
                _context.Update(examFee);
                await _context.SaveChangesAsync();
            }

            // Add fee collections
            foreach (var fcDto in dto.FeeCollections)
            {
                var collection = new ExamFeeCollection
                {
                    ExamFeeId = examFee.ExamFeeId,
                    StudentId = fcDto.StudentId,
                    ExamFeeAmount = fcDto.ExamFeeAmount,
                    TotalSubject = fcDto.TotalSubject
                };
                _context.ExamFeeCollections.Add(collection);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Exam fee collections added successfully!" });
        }

        // GET: api/ExamFeeCollections/classes/{departmentId}
        [HttpGet("classes/{departmentId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetClassesByDepartment(int departmentId)
        {
            var classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId)
                .Select(c => new
                {
                    c.ClassId,
                    c.ClassName
                })
                .OrderBy(c => c.ClassName)
                .ToListAsync();

            return Ok(classes);
        }

        // GET: api/ExamFeeCollections/students/{classId}
        [HttpGet("students/{classId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetStudentsByClass(int classId)
        {
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .Select(s => new
                {
                    s.StudentId,
                    s.StudentName
                })
                .OrderBy(s => s.StudentName)
                .ToListAsync();

            return Ok(students);
        }

        // GET: api/ExamFeeCollections/examfee?classId=1&examId=2&educationYear=2026
        [HttpGet("examfee")]
        public async Task<ActionResult> GetExamFeeByClassAndExam(int classId, int examId, string educationYear)
        {
            var examFee = await _context.ExamFees
                .FirstOrDefaultAsync(e => e.ClassId == classId
                    && e.ExamId == examId
                    && e.EducationYear == educationYear);

            if (examFee != null)
            {
                return Ok(new
                {
                    success = true,
                    amount = examFee.ExamAmount,
                    examFeeId = examFee.ExamFeeId
                });
            }

            return NotFound(new
            {
                success = false,
                message = "No exam fee setup found. You can create one now."
            });
        }
    }
}
