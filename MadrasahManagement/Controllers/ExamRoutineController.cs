using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;

namespace MadrasahManagement.Controllers
{
    public class ExamRoutineController : Controller
    {
        private readonly MadrasahDbContext _context;

        public ExamRoutineController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET: ExamRoutine
        public async Task<IActionResult> Index()
        {
            var examRoutines = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Subject)
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.ExamStartTime)
                .ToListAsync();
            return View(examRoutines);
        }

        // GET: ExamRoutine/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var examRoutine = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(m => m.ExamRoutineId == id);

            if (examRoutine == null) return NotFound();

            return View(examRoutine);
        }

        // GET: ExamRoutine/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: ExamRoutine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExamRoutineId,EducationYear,ClassId,ExamId,SubjectId,RoomNumber,ExamDate,ExamDay,ExamStartTime,ExamEndTime")] ExamRoutine examRoutine)
        {
            // Auto-fill ExamDay from ExamDate
            if (examRoutine.ExamDate != default)
            {
                examRoutine.ExamDay = examRoutine.ExamDate.ToString("dddd");
            }

            // String Time Validation Logic
            ValidateTimeComparison(examRoutine);

            if (ModelState.IsValid)
            {
                _context.Add(examRoutine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(examRoutine);
            return View(examRoutine);
        }

        // GET: ExamRoutine/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var examRoutine = await _context.ExamRoutines.FindAsync(id);
            if (examRoutine == null) return NotFound();

            PopulateDropdowns(examRoutine);
            return View(examRoutine);
        }

        // POST: ExamRoutine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExamRoutineId,EducationYear,ClassId,ExamId,SubjectId,RoomNumber,ExamDate,ExamDay,ExamStartTime,ExamEndTime")] ExamRoutine examRoutine)
        {
            if (id != examRoutine.ExamRoutineId) return NotFound();

            // Auto-fill ExamDay
            if (examRoutine.ExamDate != default)
            {
                examRoutine.ExamDay = examRoutine.ExamDate.ToString("dddd");
            }

            ValidateTimeComparison(examRoutine);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examRoutine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamRoutineExists(examRoutine.ExamRoutineId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception) // Fixed: Removed 'ex' to fix unused variable warning
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(examRoutine);
            return View(examRoutine);
        }

        // GET: ExamRoutine/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var examRoutine = await _context.ExamRoutines
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(m => m.ExamRoutineId == id);

            if (examRoutine == null) return NotFound();

            return View(examRoutine);
        }

        // POST: ExamRoutine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examRoutine = await _context.ExamRoutines.FindAsync(id);
            if (examRoutine != null)
            {
                _context.ExamRoutines.Remove(examRoutine);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ExamRoutineExists(int id)
        {
            return _context.ExamRoutines.Any(e => e.ExamRoutineId == id);
        }

        private void PopulateDropdowns(ExamRoutine? routine = null)
        {
            ViewData["ClassId"] = new SelectList(_context.Classes.OrderBy(c => c.ClassName), "ClassId", "ClassName", routine?.ClassId);
            ViewData["ExamId"] = new SelectList(_context.Examinations.OrderBy(e => e.ExamName), "ExamId", "ExamName", routine?.ExamId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.OrderBy(s => s.SubjectName), "SubjectId", "SubjectName", routine?.SubjectId);
        }

        // Helper to compare string times
        private void ValidateTimeComparison(ExamRoutine routine)
        {
            if (TimeSpan.TryParse(routine.ExamStartTime, out var start) &&
                TimeSpan.TryParse(routine.ExamEndTime, out var end))
            {
                if (end <= start)
                {
                    ModelState.AddModelError("ExamEndTime", "End time must be later than start time.");
                }
            }
        }
    }
}