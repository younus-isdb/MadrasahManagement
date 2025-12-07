using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SectionController : Controller
{
	private readonly MadrasahDbContext _context;

	public SectionController(MadrasahDbContext context)
	{
		_context = context;
	}
	[HttpGet]
	public IActionResult GetAll()
	{
		var data = _context.Sections
			.Select(s => new { s.ClassId, s.SectionName })
			.ToList();

		return Json(data);
	}

	// ===========================
	//  MAIN CREATE VIEW (Normal)
	// ===========================

	// GET: /Section/Create
	public IActionResult Create()
	{
		ViewBag.ClassList = _context.Classes.ToList();
		return View(new Section());
	}

	// POST: /Section/Create  (Normal Form Submit)
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
	//  MODAL CREATE (AJAX)
	// ===========================

	// GET: /Section/CreateModal
	[HttpGet]
	public IActionResult CreateModal()
	{
		ViewBag.ClassList = _context.Classes.ToList();
		return PartialView("_SectionModal", new Section());
	}

	// POST: /Section/CreateModalPost (AJAX)
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
	//  INDEX
	// ===========================
	public async Task<IActionResult> Index()
	{
		var data = await _context.Sections
			.Include(c => c.Class)
			.ToListAsync();

		return View(data);
	}


	// ===========================
	//  EDIT
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
	//  DETAILS
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
	//  DELETE
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
}
