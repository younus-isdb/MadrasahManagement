using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;

public class ClassController : Controller
{
	private readonly MadrasahDbContext _context;

	public ClassController(MadrasahDbContext context)
	{
		_context = context;
	}
	[HttpGet]
	public IActionResult GetAll()
	{
		var data = _context.Classes
			.Select(c => new { c.ClassId, c.ClassName })
			.ToList();

		return Json(data);
	}

	// ===========================
	//  MAIN CREATE VIEW (Normal)
	// ===========================

	// GET: /Class/Create
	public IActionResult Create()
	{
		ViewBag.DepartmentList = _context.Departments.ToList();
		return View(new Class());
	}

	// POST: /Class/Create  (Normal Form Submit)
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(Class model)
	{
		if (ModelState.IsValid)
		{
			_context.Classes.Add(model);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		ViewBag.DepartmentList = _context.Departments.ToList();
		return View(model);
	}


	// ===========================
	//  MODAL CREATE (AJAX)
	// ===========================

	// GET: /Class/CreateModal
	[HttpGet]
	public IActionResult CreateModal()
	{
		ViewBag.DepartmentList = _context.Departments.ToList();
		return PartialView("_CreateClassModal", new Class());
	}

	// POST: /Class/CreateModalPost (AJAX)
	[HttpPost]
	public async Task<IActionResult> CreateModalPost(Class model)
	{
		if (ModelState.IsValid)
		{
			_context.Classes.Add(model);
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
		var data = await _context.Classes
			.Include(c => c.Department)
			.ToListAsync();

		return View(data);
	}


	// ===========================
	//  EDIT
	// ===========================
	public async Task<IActionResult> Edit(int id)
	{
		var data = await _context.Classes.FindAsync(id);
		if (data == null) return NotFound();

		ViewBag.DepartmentList = _context.Departments.ToList();
		return View(data);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(Class model)
	{
		if (ModelState.IsValid)
		{
			_context.Classes.Update(model);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		ViewBag.DepartmentList = _context.Departments.ToList();
		return View(model);
	}


	// ===========================
	//  DETAILS
	// ===========================
	public async Task<IActionResult> Details(int id)
	{
		var data = await _context.Classes
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c => c.ClassId == id);

		if (data == null) return NotFound();

		return View(data);
	}


	// ===========================
	//  DELETE
	// ===========================
	public async Task<IActionResult> Delete(int id)
	{
		var data = await _context.Classes
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c => c.ClassId == id);

		if (data == null) return NotFound();

		return View(data);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var data = await _context.Classes.FindAsync(id);

		if (data != null)
		{
			_context.Classes.Remove(data);
			await _context.SaveChangesAsync();
		}

		return RedirectToAction(nameof(Index));
	}
}
