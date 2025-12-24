using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    public class ExaminationFeeController : Controller
    {
        private readonly MadrasahDbContext _context;

        public ExaminationFeeController(MadrasahDbContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var data = await _context.ExamFees
                            .Include(f => f.Class)
                            .Include(f => f.Examination)
                            .ToListAsync();
            return View(data);


        }

        // CREATE (GET)
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamFee examFee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(examFee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(examFee);
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var examFee = await _context.ExamFees.FindAsync(id);
            if (examFee == null) return NotFound();

            LoadDropdowns();
            return View(examFee);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamFee examFee)
        {
            if (id != examFee.ExamFeeId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(examFee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(examFee);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var examFee = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .FirstOrDefaultAsync(m => m.ExamFeeId == id);

            if (examFee == null) return NotFound();

            return View(examFee);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examFee = await _context.ExamFees.FindAsync(id);
            if (examFee == null) return NotFound();

            _context.ExamFees.Remove(examFee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Dropdown Helper
        private void LoadDropdowns()
        {
            ViewBag.ClassId = new SelectList(_context.Classes, "ClassId", "ClassName");
            ViewBag.ExamId = new SelectList(_context.Examinations, "ExamId", "ExamName");
        }
    }
}
