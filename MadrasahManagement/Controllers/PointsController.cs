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

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            var data = await _context.PointConditions
                .Include(x => x.Class)
                .Include(x => x.Examination)
                .Include(x => x.Subject)
                .Include(x => x.Details)
                .ToListAsync();

            return View(data);
        }

        // ================= CREATE (GET) =================
        public IActionResult Create()
        {
            LoadDropdowns();
            return View(new PointCondition());
        }

        // ================= CREATE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PointCondition condition)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(condition);
            }

            _context.PointConditions.Add(condition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT (GET) =================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var condition = await _context.PointConditions
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.PointConditionId == id);

            if (condition == null) return NotFound();

            LoadDropdowns();
            return View(condition);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PointCondition condition)
        {
            if (id != condition.PointConditionId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(condition);
            }

            // 🔴 Old Details remove
            var oldDetails = _context.PointConditionDetails
                .Where(d => d.PointConditionId == id);

            _context.PointConditionDetails.RemoveRange(oldDetails);

            // 🔵 Update main + new details
            _context.PointConditions.Update(condition);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE (GET) =================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var condition = await _context.PointConditions
                .Include(x => x.Class)
                .Include(x => x.Examination)
                .Include(x => x.Subject)
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.PointConditionId == id);

            if (condition == null) return NotFound();

            return View(condition);
        }

        // ================= DELETE (POST) =================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var condition = await _context.PointConditions
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.PointConditionId == id);

            if (condition == null) return NotFound();

            _context.PointConditionDetails.RemoveRange(condition.Details);
            _context.PointConditions.Remove(condition);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================= DROPDOWN HELPER =================
        private void LoadDropdowns()
        {
            ViewBag.ClassId = new SelectList(_context.Classes, "ClassId", "ClassName");
            ViewBag.ExamId = new SelectList(_context.Examinations, "ExamId", "ExamName");
            ViewBag.SubjectId = new SelectList(_context.Subjects, "SubjectId", "SubjectName");
        }
    }
}
