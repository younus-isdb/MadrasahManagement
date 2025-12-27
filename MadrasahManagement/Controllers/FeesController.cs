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

    // ==================== FEE COLLECTION ====================
    public IActionResult CollectFee()
    {
        var vm = new FeeCollectionVM
        {
            Students = _context.Students.ToList(),
            FeeTypes = _context.FeeTypes.ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CollectFee(FeeCollectionVM model)
    {
        try
        {
            // Create FeeCollection directly
            var feeCollection = new FeeCollection
            {
                StudentId = model.SelectedStudentId,
                FeeTypeId = model.SelectedFeeTypeId,
                AmountPaid = model.Amount,
                PaymentMethod = model.PaymentMethod,
                DatePaid = DateTime.Now,
                Status = PaymentStatus.Paid,
                ReceiptNumber = $"REC-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
            };

            // Save to database
            _context.FeeCollections.Add(feeCollection);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Fee collected successfully!";
            return RedirectToAction("Receipt", new { id = feeCollection.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";

            // Repopulate dropdowns
            model.Students = _context.Students.ToList();
            model.FeeTypes = _context.FeeTypes.ToList();

            return View(model);
        }
    }

    public async Task<IActionResult> Receipt(int id)
    {
        var collection = await _feeService.GetFeeCollectionByIdAsync(id);
        if (collection == null)
        {
            TempData["ErrorMessage"] = "Receipt not found!";
            return RedirectToAction(nameof(CollectFee));
        }

        return View(collection);
    }

    // ==================== REPORTS ====================
    public async Task<IActionResult> CollectionReport(DateTime? startDate, DateTime? endDate)
    {
        var fromDate = startDate ?? DateTime.Today.AddDays(-30);
        var toDate = endDate ?? DateTime.Today;

        ViewBag.StartDate = fromDate.ToString("yyyy-MM-dd");
        ViewBag.EndDate = toDate.ToString("yyyy-MM-dd");

        var collections = await _feeService.GetFeeCollectionsByDateRangeAsync(fromDate, toDate);
        return View(collections);
    }

    public async Task<IActionResult> OutstandingFees()
    {
        var outstandingFees = await _feeService.GetOutstandingFeesAsync();
        return View(outstandingFees);
    }

    public async Task<IActionResult> TodaysCollection()
    {
        var collections = await _feeService.GetTodaysCollectionsAsync();
        ViewBag.Total = collections.Sum(c => c.AmountPaid);
        return View(collections);
    }

    // ==================== UTILITY METHODS ====================
    private List<SelectListItem> GetPaymentMethods()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value = "Cash", Text = "Cash" },
            new SelectListItem { Value = "bKash", Text = "bKash" },
            new SelectListItem { Value = "Bank", Text = "Bank Transfer" },
            new SelectListItem { Value = "Card", Text = "Credit/Debit Card" }
        };
    }

    // AJAX endpoint for getting fee amount
    [HttpGet]
    public async Task<JsonResult> GetFeeAmount(int feeTypeId)
    {
        var feeType = await _context.FeeTypes.FindAsync(feeTypeId);
        return Json(new { amount = feeType?.Amount ?? 0 });
    }

    // AJAX endpoint for getting student's pending amount
    [HttpGet]
    public async Task<JsonResult> GetStudentPendingAmount(int studentId, int feeTypeId)
    {
        var totalDue = await _feeService.GetTotalFeeDueByStudentAsync(studentId);
        var totalPaid = await _feeService.GetTotalFeePaidByStudentAsync(studentId);
        var pending = totalDue - totalPaid;

        return Json(new { pending = pending });
    }

}