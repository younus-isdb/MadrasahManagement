using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;

namespace MadrasahManagement.Controllers
{
    public class SectionController : Controller
    {
        private readonly MadrasahDbContext _context;

        public SectionController(MadrasahDbContext context)
        {
            _context = context;
        }

        // GET: Section
        public async Task<IActionResult> Index()
        {
            var sections = await _context.Sections.Include(s => s.Class).ToListAsync();
            return View(sections);
        }

        // GET: Section/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var section = await _context.Sections
                .Include(s => s.Class)
                .FirstOrDefaultAsync(m => m.SectionId == id);

            if (section == null) return NotFound();

            return View(section);
        }

        // GET: Section/Create
        public IActionResult Create()
        {
            ViewBag.Classes = _context.Classes.ToList();
            return View();
        }

        // POST: Section/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Section section)
        {
            if (ModelState.IsValid)
            {
                _context.Add(section);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Classes = _context.Classes.ToList();
            return View(section);
        }

        // GET: Section/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var section = await _context.Sections.FindAsync(id);
            if (section == null) return NotFound();

            ViewBag.Classes = _context.Classes.ToList();
            return View(section);
        }

        // POST: Section/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Section section)
        {
            if (id != section.SectionId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(section);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionExists(section.SectionId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Classes = _context.Classes.ToList();
            return View(section);
        }

        // GET: Section/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var section = await _context.Sections
                .Include(s => s.Class)
                .FirstOrDefaultAsync(m => m.SectionId == id);

            if (section == null) return NotFound();

            return View(section);
        }

        // POST: Section/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section != null)
            {
                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SectionExists(int id)
        {
            return _context.Sections.Any(e => e.SectionId == id);
        }
    }
}
