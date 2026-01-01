using MadrasahManagement.Controllers;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class SectionController : Controller
{
	private readonly MadrasahDbContext _context;
  

    public SectionController(MadrasahDbContext context)
	{
        _context = context ; 
       
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

    //// GET: /Section/Create
    //public IActionResult Create()
    //{
    //	ViewBag.ClassList = _context.Classes.ToList();
    //	return View(new Section());
    //}

    //// POST: /Section/Create  (Normal Form Submit)
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(Section model)
    //{
    //	if (ModelState.IsValid)
    //	{
    //		_context.Sections.Add(model);
    //		await _context.SaveChangesAsync();
    //		return RedirectToAction(nameof(Index));
    //	}

    //	ViewBag.ClassList = _context.Classes.ToList();
    //	return View(model);
    //}

    // GET: Section/Create
    public async Task<IActionResult> Create(int? departmentId)
    {
        // Get all departments
        ViewBag.Departments = await _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName
            })
            .OrderBy(d => d.Text)
            .ToListAsync();

        if (departmentId.HasValue)
        {
            ViewBag.SelectedDepartmentId = departmentId;

            // Get classes for selected department
            ViewBag.Classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId.Value)
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName
                })
                .OrderBy(c => c.Text)
                .ToListAsync();
        }

        return View();
    }

    // POST: Section/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Section section)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var duplicateExists = await _context.Sections
                    .Include(s => s.Class)
                    .AnyAsync(s =>
                        s.SectionName.Trim().ToLower() == section.SectionName.Trim().ToLower() &&
                        s.ClassId == section.ClassId &&
                        s.DepartmentId == section.DepartmentId);

                if (duplicateExists)
                {
                    ModelState.AddModelError("SectionName",
                        $"A section named '{section.SectionName}' already exists in this class.");

                    // Reload dropdowns
                    await ReloadDropdowns(section.DepartmentId);
                    return View(section);
                }

                // Additional check: Make sure the class belongs to the selected department
                var classBelongsToDept = await _context.Classes
                    .AnyAsync(c => c.ClassId == section.ClassId && c.DepartmentId == section.DepartmentId);

                if (!classBelongsToDept)
                {
                    ModelState.AddModelError("ClassId",
                        "The selected class does not belong to the selected department.");

                    await ReloadDropdowns(section.DepartmentId);
                    return View(section);
                }

                // Save the section
                _context.Sections.Add(section);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Section '{section.SectionName}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch 
            {
               
                ModelState.AddModelError("", "An error occurred while creating the section.");

                await ReloadDropdowns(section.DepartmentId);
                return View(section);
            }
        }

        // If model state is invalid, reload dropdowns
        if (section.DepartmentId > 0)
        {
            await ReloadDropdowns(section.DepartmentId);
        }
        else
        {
            await ReloadAllDepartments();
        }

        return View(section);
    }

    // Helper method to reload dropdowns
    private async Task ReloadDropdowns(int departmentId)
    {
        // Load all departments
        ViewBag.Departments = await _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName
            })
            .OrderBy(d => d.Text)
            .ToListAsync();

        // Load classes for the specific department
        ViewBag.Classes = await _context.Classes
            .Where(c => c.DepartmentId == departmentId)
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = c.ClassName
            })
            .OrderBy(c => c.Text)
            .ToListAsync();
    }

    private async Task ReloadAllDepartments()
    {
        ViewBag.Departments = await _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName
            })
            .OrderBy(d => d.Text)
            .ToListAsync();
    }

    // ===========================
    //  MODAL CREATE (AJAX)
    // ===========================

    // GET: /Section/CreateModal

    // GET: /Section/CreateModal
    [HttpGet]
    public IActionResult CreateModal()
    {
        // Get all departments for the first dropdown
        ViewBag.DepartmentList = _context.Departments.ToList();

        // Get all classes (or empty initially)
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
    public async Task<IActionResult> Index(int? departmentId, int? classId)
    {
        // Get all departments for dropdown
        ViewBag.Departments = await _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName
            })
            .OrderBy(d => d.Text)
            .ToListAsync();

        // Get all classes for initial dropdown (when no department is selected)
        ViewBag.AllClasses = await _context.Classes
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = c.ClassName
            })
            .OrderBy(c => c.Text)
            .ToListAsync();

        // Store selected values in ViewBag
        ViewBag.SelectedDepartmentId = departmentId;
        ViewBag.SelectedClassId = classId;

        // Initialize query with includes
        var query = _context.Sections
            .Include(s => s.Class)
                .ThenInclude(c => c.Department)
            .AsQueryable();

        // Apply department filter if selected
        if (departmentId.HasValue)
        {
            // Get classes only for the selected department
            ViewBag.Classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId.Value)
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName
                })
                .OrderBy(c => c.Text)
                .ToListAsync();

            query = query.Where(s => s.Class.DepartmentId == departmentId.Value);
        }

        // Apply class filter if selected
        if (classId.HasValue)
        {
            query = query.Where(s => s.ClassId == classId.Value);
        }

        // Get filtered sections
        var sections = await query
            .OrderBy(s => s.Class.Department.DepartmentName)
            .ThenBy(s => s.Class.ClassName)
            .ThenBy(s => s.SectionName)
            .ToListAsync();

        return View(sections);
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

    // POST: Section/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Section section)
    {
        if (id != section.SectionId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var duplicateExists = await _context.Sections
                    .AnyAsync(s =>
                        s.SectionName.Trim().ToLower() == section.SectionName.Trim().ToLower() &&
                        s.ClassId == section.ClassId &&
                        s.DepartmentId == section.DepartmentId &&
                        s.SectionId != section.SectionId);

                if (duplicateExists)
                {
                    ModelState.AddModelError("SectionName",
                        $"A section named '{section.SectionName}' already exists in this class.");

                    await ReloadDropdownsForEdit(section.SectionId);
                    return View(section);
                }

                // Validate class belongs to department
                var classBelongsToDept = await _context.Classes
                    .AnyAsync(c => c.ClassId == section.ClassId && c.DepartmentId == section.DepartmentId);

                if (!classBelongsToDept)
                {
                    ModelState.AddModelError("ClassId",
                        "The selected class does not belong to the selected department.");

                    await ReloadDropdownsForEdit(section.SectionId);
                    return View(section);
                }

                // Update section
                _context.Update(section);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Section '{section.SectionName}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SectionExists(section.SectionId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch
            {
              
                ModelState.AddModelError("", "An error occurred while updating the section.");

                await ReloadDropdownsForEdit(section.SectionId);
                return View(section);
            }
        }

        await ReloadDropdownsForEdit(section.SectionId);
        return View(section);
    }

    private async Task ReloadDropdownsForEdit(int sectionId)
    {
        var section = await _context.Sections.FindAsync(sectionId);

        if (section != null)
        {
            // Load all departments
            ViewBag.Departments = await _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName,
                    Selected = d.DepartmentId == section.DepartmentId
                })
                .OrderBy(d => d.Text)
                .ToListAsync();

            // Load classes for the specific department
            ViewBag.Classes = await _context.Classes
                .Where(c => c.DepartmentId == section.DepartmentId)
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName,
                    Selected = c.ClassId == section.ClassId
                })
                .OrderBy(c => c.Text)
                .ToListAsync();
        }
    }

    private bool SectionExists(int id)
    {
        return _context.Sections.Any(e => e.SectionId == id);
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
