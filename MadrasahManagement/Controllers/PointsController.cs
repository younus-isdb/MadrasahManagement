using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    public class PointsController : Controller
    {
        private readonly MadrasahDbContext _context;

        public PointsController(MadrasahDbContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var data = await _context.PointConditions
                            .Include(f => f.Class)
                            .Include(f => f.Examination)
                            .Include(f=>f.Subject)

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
        public async Task<IActionResult> Create(PointCondition condition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(condition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(condition);
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var condition = await _context.PointConditions.FindAsync(id);
            if (condition == null) return NotFound();

            LoadDropdowns();
            return View(condition);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PointCondition condition)
        {
            if (id != condition.PointConditionId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(condition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(condition);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var condition = await _context.PointConditions
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e=>e.Subject)
                .FirstOrDefaultAsync(m => m.PointConditionId == id);

            if (condition == null) return NotFound();

            return View(condition);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var condition = await _context.PointConditions.FindAsync(id);
            if (condition == null) return NotFound();

            _context.PointConditions.Remove(condition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Dropdown Helper
        private void LoadDropdowns()
        {
            ViewBag.ClassId = new SelectList(_context.Classes, "ClassId", "ClassName");
            ViewBag.ExamId = new SelectList(_context.Examinations, "ExamId", "ExamName");
            ViewBag.ExamId = new SelectList(_context.Examinations, "SubjectId", "SubjectName");
        }
    }
}
    
