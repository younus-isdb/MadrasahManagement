using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace MadrasahManagement.Controllers
{
    public class ExaminationController : Controller
    {
        private readonly MadrasahDbContext _context;

        public ExaminationController(MadrasahDbContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var exams = await _context.Examinations.ToListAsync();
            return View(exams);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.Examinations
                                     .FirstOrDefaultAsync(e => e.ExamId == id);

            if (exam == null) return NotFound();

            return View(exam);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Examination model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Examinations.Add(model);   // ✅ REQUIRED
            await _context.SaveChangesAsync();  // ✅ REQUIRED

            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.Examinations.FindAsync(id);
            if (exam == null) return NotFound();

            return View(exam);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Examination exam)
        {
            if (id != exam.ExamId) return NotFound();

            if (!ModelState.IsValid)
                return View(exam);

            _context.Update(exam);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.Examinations
                                     .FirstOrDefaultAsync(e => e.ExamId == id);

            if (exam == null) return NotFound();

            return View(exam);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Examinations.FindAsync(id);
            if (exam == null) return NotFound();

            _context.Examinations.Remove(exam);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

