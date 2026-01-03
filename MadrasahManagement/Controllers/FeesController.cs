using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using MadrasahManagement.Services;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class FeesController : Controller
{
    private readonly MadrasahDbContext _context;
    private readonly IFeeService _feeService;

    public FeesController(MadrasahDbContext context, IFeeService feeService)
    {
        _context = context;
        _feeService = feeService;
    }

    // ==================== FEE TYPE MANAGEMENT ====================
    public async Task<IActionResult> FeeTypes()
    {
        var feeTypes = await _feeService.GetAllFeeTypesAsync();
        return View(feeTypes);
    }

    // GET
    public IActionResult CreateFeeType(int? departmentId)
    {
        var viewModel = new CreateFeeTypeViewModel();

        // Load all departments
        ViewBag.Departments = _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName
            })
            .OrderBy(d => d.Text)
            .ToList();

        // Load classes if department is selected
        if (departmentId.HasValue)
        {
            viewModel.DepartmentId = departmentId.Value;

            ViewBag.Classes = _context.Classes
                .Where(c => c.DepartmentId == departmentId.Value)
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName
                })
                .OrderBy(c => c.Text)
                .ToList();
        }
        else
        {
            ViewBag.Classes = new List<SelectListItem>();
        }

        return View(viewModel);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFeeType(CreateFeeTypeViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            // Convert ViewModel to Entity
            var feeType = new FeeType
            {
                DepartmentId = viewModel.DepartmentId,
                ClassId = viewModel.ClassId,
                Name = viewModel.Name,
                Amount = viewModel.Amount,
                Frequency = viewModel.Frequency
            };

            _context.FeeTypes.Add(feeType);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Fee type created successfully!";
            return RedirectToAction(nameof(FeeTypes));
        }

        // If invalid, reload dropdowns
        await ReloadDropdowns(viewModel.DepartmentId);
        return View(viewModel);
    }

    private async Task ReloadDropdowns(int departmentId)
    {
        ViewBag.Departments = await _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName,
                Selected = d.DepartmentId == departmentId
            })
            .OrderBy(d => d.Text)
            .ToListAsync();

        if (departmentId > 0)
        {
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
        else
        {
            ViewBag.Classes = new List<SelectListItem>();
        }
    }

    // ==================== EDIT FEE TYPE ====================
    // GET: EditFeeType
    public async Task<IActionResult> EditFeeType(int id)
    {
        var feeType = await _feeService.GetFeeTypeByIdAsync(id);
        if (feeType == null)
        {
            return NotFound();
        }

        // Convert to ViewModel
        var viewModel = new EditFeeTypeViewModel
        {
            FeeTypeId = feeType.FeeTypeId,
            DepartmentId = feeType.DepartmentId,
            ClassId = feeType.ClassId,
            Name = feeType.Name,
            Amount = feeType.Amount,
            Frequency = feeType.Frequency
        };

        // Populate departments dropdown
        ViewBag.Departments = await _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName,
                Selected = d.DepartmentId == feeType.DepartmentId
            })
            .OrderBy(d => d.Text)
            .ToListAsync();

        // Populate classes for the selected department
        ViewBag.Classes = await _context.Classes
            .Where(c => c.DepartmentId == feeType.DepartmentId)
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = c.ClassName,
                Selected = c.ClassId == feeType.ClassId
            })
            .OrderBy(c => c.Text)
            .ToListAsync();

        return View(viewModel);
    }

    // POST: EditFeeType
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFeeType(int id, EditFeeTypeViewModel viewModel)
    {
        if (id != viewModel.FeeTypeId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Convert ViewModel back to Entity
                var feeType = await _feeService.GetFeeTypeByIdAsync(id);
                if (feeType == null)
                {
                    return NotFound();
                }

                // Update entity from ViewModel
                feeType.DepartmentId = viewModel.DepartmentId;
                feeType.ClassId = viewModel.ClassId;
                feeType.Name = viewModel.Name;
                feeType.Amount = viewModel.Amount;
                feeType.Frequency = viewModel.Frequency;

                await _feeService.UpdateFeeTypeAsync(feeType);

                TempData["SuccessMessage"] = "Fee type updated successfully!";
                return RedirectToAction(nameof(FeeTypes));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }
        }

        // If invalid, reload dropdowns
        await ReloadDropdownsForEdit(viewModel.DepartmentId, viewModel.ClassId);
        return View(viewModel);
    }

    private async Task ReloadDropdownsForEdit(int departmentId, int selectedClassId)
    {
        // Load all departments
        ViewBag.Departments = await _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName,
                Selected = d.DepartmentId == departmentId
            })
            .OrderBy(d => d.Text)
            .ToListAsync();

        // Load classes for selected department
        if (departmentId > 0)
        {
            ViewBag.Classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId)
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName,
                    Selected = c.ClassId == selectedClassId
                })
                .OrderBy(c => c.Text)
                .ToListAsync();
        }
        else
        {
            ViewBag.Classes = new List<SelectListItem>();
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteFeeType(int id)
    {
        try
        {
            var result = await _feeService.DeleteFeeTypeAsync(id);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Fee type deleted!" : "Fee type not found!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
        }

        return RedirectToAction(nameof(FeeTypes));
    }

   
}