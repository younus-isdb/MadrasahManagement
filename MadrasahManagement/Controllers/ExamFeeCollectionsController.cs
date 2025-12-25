using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ExamFeeCollectionsController : Controller
{
    private readonly MadrasahDbContext _context;

    public ExamFeeCollectionsController(MadrasahDbContext context)
    {
        _context = context;
    }

    // =============== INDEX =================
    public async Task<IActionResult> Index()
    {
        var data = await _context.ExamFeeCollections
            .Include(x => x.Student)
            .Include(x => x.Examination)
            .Include(x => x.Class)
            .ToListAsync();

        // 🔥 Just ADD (SUM)
        ViewBag.TotalAmount = data.Sum(x => x.ExamFee);

        return View(data);
    }

    // =============== CREATE (GET) =================
    public IActionResult Create()
    {
        ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentName");
        ViewData["ExamId"] = new SelectList(_context.Examinations, "ExamId", "ExamName");
        ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName");

        return View();
    }

    // =============== CREATE (POST) =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamFeeCollection model)
    {
        if (ModelState.IsValid)
        {
            _context.ExamFeeCollections.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentName" /*model.StudentId*/);
        ViewData["ExamId"] = new SelectList(_context.Examinations, "ExamId", "ExamName", model.ExamId);
        ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", model.ClassId);

        return View(model);
    }

    // =============== DELETE =================
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var data = await _context.ExamFeeCollections
            .Include(x => x.Student)
            .FirstOrDefaultAsync(x => x.FeeCollectionId == id);

        if (data == null) return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.ExamFeeCollections.FindAsync(id);
        if (data != null)
        {
            _context.ExamFeeCollections.Remove(data);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
