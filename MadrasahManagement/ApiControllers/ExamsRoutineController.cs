using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExammRoutineController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExammRoutineController(MadrasahDbContext context)
        {
            _context = context;
        }

        // =========================
        // MASTER – DETAILS (READ)
        // =========================
        [HttpGet("master-details")]
        public async Task<IActionResult> GetMasterDetails()
        {
            var data = await _context.ExamRoutines
                .Include(x => x.Class)
                .Include(x => x.Examination)
                .Include(x => x.Subject)
                .GroupBy(x => new
                {
                    x.ExamRoutineId,
                    x.EducationYear,
                    x.ClassId,
                    ClassName = x.Class!.ClassName,
                    x.ExamId,
                    ExamName = x.Examination!.ExamName
                })
                .Select(g => new ExamRoutineMasterReadDto
                {
                    ExamRoutineId = g.Key.ExamRoutineId,
                    EducationYear = g.Key.EducationYear,
                    ClassId = g.Key.ClassId,
                    ClassName = g.Key.ClassName,
                    ExamId = g.Key.ExamId,
                    ExamName = g.Key.ExamName,

                    Subjects = g.Select(x => new ExamRoutineSubjectDetailReadDto
                    {
                        SubjectId = x.SubjectId,
                        SubjectName = x.Subject!.SubjectName,
                        RoomNumber = x.RoomNumber,
                        ExamDate = x.ExamDate,
                        ExamDay = x.ExamDay,
                        ExamStartTime = x.ExamStartTime,
                        ExamEndTime = x.ExamEndTime
                    }).ToList()
                })
                .ToListAsync();

            return Ok(data);
        }
        [HttpGet("master-details/{id}")]
        public async Task<IActionResult> GetMasterDetailsById(int id)
        {
            // ১. প্রথমে ওই নির্দিষ্ট ID দিয়ে মেইন রেকর্ডটি খুঁজে বের করুন
            var target = await _context.ExamRoutines.FindAsync(id);
            if (target == null) return NotFound();

            // ২. এবার ওই একই Class এবং একই Exam এর আন্ডারে যত সাবজেক্ট আছে সব নিয়ে আসুন
            var data = await _context.ExamRoutines
                .Include(x => x.Class)
                .Include(x => x.Examination)
                .Include(x => x.Subject)
                .Where(x => x.ClassId == target.ClassId && x.ExamId == target.ExamId && x.EducationYear == target.EducationYear)
                .GroupBy(x => new
                {
                    // গ্রুপ করার জন্য একটি কমন আইডি ব্যবহার করুন (এক্ষেত্রে ক্লাস বা এক্সাম আইডি)
                    x.EducationYear,
                    x.ClassId,
                    ClassName = x.Class!.ClassName,
                    x.ExamId,
                    ExamName = x.Examination!.ExamName
                })
                .Select(g => new ExamRoutineMasterReadDto
                {
                    // এখানে মেইন আইডিটি পাস করুন যাতে এডিট পেজ কাজ করে
                    ExamRoutineId = id,
                    EducationYear = g.Key.EducationYear,
                    ClassId = g.Key.ClassId,
                    ClassName = g.Key.ClassName,
                    ExamId = g.Key.ExamId,
                    ExamName = g.Key.ExamName,
                    Subjects = g.Select(x => new ExamRoutineSubjectDetailReadDto
                    {
                        // এখানে প্রতিটি সাবজেক্টের নিজস্ব আইডি দিন যাতে আপডেট করার সময় কাজে লাগে
                        ExamRoutineId = x.ExamRoutineId,
                        SubjectId = x.SubjectId,
                        SubjectName = x.Subject!.SubjectName,
                        RoomNumber = x.RoomNumber,
                        ExamDate = x.ExamDate,
                        ExamDay = x.ExamDay,
                        ExamStartTime = x.ExamStartTime,
                        ExamEndTime = x.ExamEndTime
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return Ok(data);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExamRoutineCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = new ExamRoutine
            {
                EducationYear = dto.EducationYear,
                ClassId = dto.ClassId,
                ExamId = dto.ExamId,
                SubjectId = dto.SubjectId,
                RoomNumber = dto.RoomNumber,
                ExamDate = dto.ExamDate,
                ExamDay = dto.ExamDay,
                ExamStartTime = dto.ExamStartTime,
                ExamEndTime = dto.ExamEndTime
            };

            _context.ExamRoutines.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExamRoutineUpdateDto dto)
        {
            if (id != dto.ExamRoutineId)
                return BadRequest("Id mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.ExamRoutines.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.EducationYear = dto.EducationYear;
            existing.ClassId = dto.ClassId;
            existing.ExamId = dto.ExamId;
            existing.SubjectId = dto.SubjectId;
            existing.RoomNumber = dto.RoomNumber;
            existing.ExamDate = dto.ExamDate;
            existing.ExamDay = dto.ExamDay;
            existing.ExamStartTime = dto.ExamStartTime;
            existing.ExamEndTime = dto.ExamEndTime;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.ExamRoutines.FindAsync(id);
            if (entity == null)
                return NotFound();

            _context.ExamRoutines.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
