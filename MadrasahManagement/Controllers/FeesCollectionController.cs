using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using MadrasahManagement.Services;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class FeesCollectionController : Controller
{
    private readonly MadrasahDbContext _context;
    private readonly IFeeService _feeService;

    public FeesCollectionController(MadrasahDbContext context, IFeeService feeService)
    {
        _context = context;
        _feeService = feeService;
    }

    
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

    //REPORTS 
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

    // UTILITY METHODS
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