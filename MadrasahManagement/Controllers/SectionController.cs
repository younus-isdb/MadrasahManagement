using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;

public class SectionController : Controller
{
    private readonly MadrasahDbContext _context;

    public SectionController(MadrasahDbContext context)
    {
        _context = context;
    }

    // ===========================
    // INDEX
    // ===========================
    public async Task<IActionResult> Index()
    {
        var data = await _context.Sections
            .Include(s => s.Class)
            .ToListAsync();

        return View(data);
    }

    // ===========================
    // CREATE (Normal View)
    // ===========================
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.ClassList = _context.Classes.ToList();
        return View(new Section());
    }

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
    // AJAX MODAL CREATE
    // ===========================
    [HttpGet]
    public IActionResult CreateModal()
    {
        ViewBag.ClassList = _context.Classes.ToList();
        return PartialView("_CreateSectionModal", new Section());
    }

    [HttpPost]
    public async Task<IActionResult> CreateModalPost(Section model)
    {
        if (ModelState.IsValid)
        {
            _context.Sections.Add(model);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        return Json(new { success = false });
    }

    // ===========================
    // AJAX API → Refresh Dropdown
    // ===========================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Classes
            .Select(c => new
            {
                classId = c.ClassId,
                className = c.ClassName
            }).ToListAsync();

        return Json(data);
    }

    // ===========================
    // EDIT
    // ===========================
    public async Task<IActionResult> Edit(int id)
    {
        var data = await _context.Sections.FindAsync(id);
        if (data == null) return NotFound();

        ViewBag.ClassList = _context.Classes.ToList();
        return View(data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Section model)
    {
        if (ModelState.IsValid)
        {
            _context.Sections.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.ClassList = _context.Classes.ToList();
        return View(model);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
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
}
