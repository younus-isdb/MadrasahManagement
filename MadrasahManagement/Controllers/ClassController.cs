using Microsoft.AspNetCore.Authorization;
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
	[HttpPost]
	public async Task<IActionResult> CreateFromModal(Class model)
	{
		if (ModelState.IsValid)
		{
			_context.Classes.Add(model);
			await _context.SaveChangesAsync();

			return Json(new { success = true });
		}

		// validation fail হলে আবার partial return
		return PartialView("_CreateClassModal", model);
	}


	// GET: Class
	public async Task<IActionResult> Index()
	{
		var data = await _context.Classes
			.Include(c => c.Department)
			.ToListAsync();

		return View(data);
	}
	//[Authorize]
	// GET: Create
	public IActionResult Create()
	{
		ViewBag.DepartmentList = _context.Departments.ToList();
		return View();
	}

	// POST: Create
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

		// Required again when validation fails
		ViewBag.DepartmentList = _context.Departments.ToList();
		return View(model);
	}

	// GET: Edit
	public async Task<IActionResult> Edit(int id)
	{
		var data = await _context.Classes.FindAsync(id);
		if (data == null) return NotFound();

		ViewBag.DepartmentList = _context.Departments.ToList();
		return View(data);
	}

	// POST: Edit
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

	// GET: Details
	public async Task<IActionResult> Details(int id)
	{
		var data = await _context.Classes
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c => c.ClassId == id);

		if (data == null) return NotFound();

		return View(data);
	}

	// GET: Delete
	public async Task<IActionResult> Delete(int id)
	{
		var data = await _context.Classes
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c => c.ClassId == id);

		if (data == null) return NotFound();

		return View(data);
	}

	// POST: Delete
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
