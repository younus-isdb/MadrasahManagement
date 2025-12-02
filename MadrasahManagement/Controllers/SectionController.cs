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

        // ===========================
        // MAIN CREATE (Normal, page)
        // ===========================
        // GET: /Section/Create
        public IActionResult Create()
        {
            ViewBag.ClassList = _context.Classes.ToList();
            return View(new Section());
        }

        // POST: /Section/Create (normal form submit)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Section model)
        {
            if (ModelState.IsValid)
            {
                _context.Sections.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ClassList = _context.Classes.ToList();
            return View(model);
        }


        // ===========================
        // MODAL CREATE (AJAX)
        // ===========================
        // GET: /Section/CreateModal
        [HttpGet]
        public IActionResult CreateModal()
        {
            ViewBag.ClassList = _context.Classes.ToList();
            // Partial view MUST set Layout = null
            return PartialView("_CreateSectionPartial", new Section());
        }

        // POST: /Section/CreateModalPost  (AJAX)
        [HttpPost]
        //[ValidateAntiForgeryToken] // use antiforgery if you send token (recommended)
        //public async Task<IActionResult> CreateModalPost(Section model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Sections.Add(model);
        //        await _context.SaveChangesAsync();

        //        // You can return data to update UI (e.g., new Id)
        //        return Json(new { success = true, id = model.SectionId });
        //    }

        //    // If validation fails, re-render partial and return HTML so client can replace modal body
        //    ViewBag.ClassList = _context.Classes.ToList();
        //    var html = await this.RenderViewAsync("_CreateSectionPartial", model, true);
        //    return Json(new { success = false, html });
        //}


        // ===========================
        // INDEX
        // ===========================
        public async Task<IActionResult> Index()
        {
            var list = await _context.Sections
                .Include(s => s.Class)
                .Include(s => s.Students)   // optional if you show counts
                .Include(s => s.Timetables) // optional
                .ToListAsync();

            return View(list);
        }


        // ===========================
        // EDIT
        // ===========================
        // GET: /Section/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Sections.FindAsync(id);
            if (model == null) return NotFound();

            ViewBag.ClassList = _context.Classes.ToList();
            return PartialView("_EditSectionPartial", model); // or return View(model) if full page
        }

        // POST: /Section/Edit (modal or normal)
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditModalPost(Section model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.ClassList = _context.Classes.ToList();
        //        var html = await this.RenderViewAsync("_EditSectionPartial", model, true);
        //        return Json(new { success = false, html });
        //    }

        //    _context.Sections.Update(model);
        //    await _context.SaveChangesAsync();
        //    return Json(new { success = true });
        //}


        // ===========================
        // DETAILS
        // ===========================
        public async Task<IActionResult> Details(int id)
        {
            var data = await _context.Sections
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.SectionId == id);

            if (data == null) return NotFound();
            return View(data);
        }


        // ===========================
        // DELETE
        // ===========================
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.Sections
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.SectionId == id);

            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var data = await _context.Sections.FindAsync(id);
            if (data != null)
            {
                _context.Sections.Remove(data);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // Helper: check exists
        private bool SectionExists(int id)
        {
            return _context.Sections.Any(s => s.SectionId == id);
        }
    }
}
