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

    public IActionResult CreateFeeType()
    {
        // Make sure to populate ViewBag.Classes properly
        ViewBag.Classes = _context.Classes
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = c.ClassName
            })
            .ToList();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateFeeType(FeeType model)
    {
        try
        {
            // DEBUG: Check what data we're getting
            Console.WriteLine($"Name: {model.Name}, ClassId: {model.ClassId}, Amount: {model.Amount}");

            // Direct save - simplest possible
            _context.FeeTypes.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Fee type saved!";
            return RedirectToAction("FeeTypes");
        }
        catch (Exception ex)
        {
            // Show exact error
            TempData["Error"] = $"Error: {ex.Message}";

            // Repopulate dropdown
            ViewBag.Classes = _context.Classes
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName
                })
                .ToList();

            return View(model);
        }
    }

    // ==================== EDIT FEE TYPE ====================
    public async Task<IActionResult> EditFeeType(int id)
    {
        var feeType = await _feeService.GetFeeTypeByIdAsync(id);
        if (feeType == null)
        {
            return NotFound();
        }

        // Populate classes dropdown
        ViewBag.Classes = _context.Classes
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = c.ClassName,
                Selected = c.ClassId == feeType.ClassId
            })
            .ToList();

        return View(feeType);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFeeType(int id, FeeType model)
    {
        if (id != model.FeeTypeId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _feeService.UpdateFeeTypeAsync(model);
                TempData["SuccessMessage"] = "Fee type updated successfully!";
                return RedirectToAction(nameof(FeeTypes));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }
        }

        // Repopulate dropdown
        ViewBag.Classes = _context.Classes
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = c.ClassName,
                Selected = c.ClassId == model.ClassId
            })
            .ToList();

        return View(model);
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