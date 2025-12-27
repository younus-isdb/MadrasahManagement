using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using MadrasahManagement.Services;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class FeeCollectionController : ControllerBase
{
    private readonly MadrasahDbContext _context;
    private readonly IFeeService _feeService;

    public FeeCollectionController(MadrasahDbContext context, IFeeService feeService)
    {
        _context = context;
        _feeService = feeService;
    } 


    [HttpGet("collectfee")]
    public IActionResult GetCollectFeeData()
    {
        var vm = new FeeCollectionVM
        {
            Students = _context.Students.ToList(),
            FeeTypes = _context.FeeTypes.ToList()
        };
        return Ok(vm);
    }

    [HttpPost("collectfee")]
    public async Task<IActionResult> CollectFee(FeeCollectionVM model)
    {
        try
        {
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

            _context.FeeCollections.Add(feeCollection);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReceipt), new { id = feeCollection.Id }, feeCollection);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("receipt/{id}")]
    public async Task<IActionResult> GetReceipt(int id)
    {
        var collection = await _feeService.GetFeeCollectionByIdAsync(id);
        if (collection == null)
        {
            return NotFound();
        }
        return Ok(collection);
    }

    // ==================== REPORTS ====================
    [HttpGet("collectionreport")]
    public async Task<IActionResult> GetCollectionReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var fromDate = startDate ?? DateTime.Today.AddDays(-30);
        var toDate = endDate ?? DateTime.Today;

        var collections = await _feeService.GetFeeCollectionsByDateRangeAsync(fromDate, toDate);
        return Ok(new { StartDate = fromDate, EndDate = toDate, Collections = collections });
    }

    //[HttpGet("outstandingfees")]
    //public async Task<IActionResult> GetOutstandingFees()
    //{
    //    var outstandingFees = await _feeService.GetOutstandingFeesAsync();
    //    return Ok(outstandingFees);
    //}

    [HttpGet("todayscollection")]
    public async Task<IActionResult> GetTodaysCollection()
    {
        var collections = await _feeService.GetTodaysCollectionsAsync();
        var total = collections.Sum(c => c.AmountPaid);
        return Ok(new { Collections = collections, Total = total });
    }

    //// ==================== UTILITY ENDPOINTS ====================
    //[HttpGet("feeamount/{feeTypeId}")]
    //public async Task<IActionResult> GetFeeAmount(int feeTypeId)
    //{
    //    var feeType = await _context.FeeTypes.FindAsync(feeTypeId);
    //    return Ok(new { amount = feeType?.Amount ?? 0 });
    //}

    //[HttpGet("studentpendingamount")]
    //public async Task<IActionResult> GetStudentPendingAmount([FromQuery] int studentId, [FromQuery] int feeTypeId)
    //{
    //    var totalDue = await _feeService.GetTotalFeeDueByStudentAsync(studentId);
    //    var totalPaid = await _feeService.GetTotalFeePaidByStudentAsync(studentId);
    //    var pending = totalDue - totalPaid;

    //    return Ok(new { pending = pending });
    //}

    [HttpGet("paymentmethods")]
    public IActionResult GetPaymentMethods()
    {
        var methods = new List<object>
        {
            new { Value = "Cash", Text = "Cash" },
            new { Value = "bKash", Text = "bKash" },
            new { Value = "Bank", Text = "Bank Transfer" },
            new { Value = "Card", Text = "Credit/Debit Card" }
        };
        return Ok(methods);
    }
}