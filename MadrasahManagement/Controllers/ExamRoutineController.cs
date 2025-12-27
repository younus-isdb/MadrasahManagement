using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
  // Change to your namespace
using MadrasahManagement.Models; // Your ExamRoutine model

namespace YourProjectNamespace.Controllers
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName");
            ViewData["ExamId"] = new SelectList(_context.Examinations, "ExamId", "ExamName");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName");
            return View();
        }

        // POST: ExamRoutine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExamRoutineId,EducationYear,ClassId,ExamId,SubjectId,RoomNumber,ExamDate,ExamStartTime,ExamEndTime")] ExamRoutine examRoutine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(examRoutine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", examRoutine.ClassId);
            ViewData["ExamId"] = new SelectList(_context.Examinations, "ExamId", "ExamName", examRoutine.ExamId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName", examRoutine.SubjectId);
            return View(examRoutine);
        }

        // GET: ExamRoutine/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var examRoutine = await _context.ExamRoutines.FindAsync(id);
            if (examRoutine == null) return NotFound();

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", examRoutine.ClassId);
            ViewData["ExamId"] = new SelectList(_context.Examinations, "ExamId", "ExamName", examRoutine.ExamId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName", examRoutine.SubjectId);

            return View(examRoutine);
        }

        // POST: ExamRoutine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExamRoutineId,EducationYear,ClassId,ExamId,SubjectId,RoomNumber,ExamDate,ExamStartTime,ExamEndTime")] ExamRoutine examRoutine)
        {
            if (id != examRoutine.ExamRoutineId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examRoutine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamRoutineExists(examRoutine.ExamRoutineId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", examRoutine.ClassId);
            ViewData["ExamId"] = new SelectList(_context.Examinations, "ExamId", "ExamName", examRoutine.ExamId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName", examRoutine.SubjectId);

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
            _context.ExamRoutines.Remove(examRoutine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamRoutineExists(int id)
        {
            return _context.ExamRoutines.Any(e => e.ExamRoutineId == id);
        }
    }
}
