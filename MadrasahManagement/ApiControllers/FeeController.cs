using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using MadrasahManagement.Services;
using MadrasahManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class FeeController : ControllerBase
{
    private readonly MadrasahDbContext _context;
    private readonly IFeeService _feeService;

    public FeeController(MadrasahDbContext context, IFeeService feeService)
    {
        _context = context;
        _feeService = feeService;
    }

    [HttpGet("feetypes")]
    public async Task<IActionResult> GetallFeeTypes()
    {
        var feeTypes = await _feeService.GetAllFeeTypesAsync();
        return Ok(feeTypes);
    }

    [HttpGet("feetypes/{id}")]
    public async Task<IActionResult> GetFeeType(int id)
    {
        var feeType = await _feeService.GetFeeTypeByIdAsync(id);
        if (feeType == null)
        {
            return NotFound();
        }
        return Ok(feeType);
    }

    [HttpPost("feetypes")]
    public async Task<IActionResult> SaveFeeType(FeeType model)
    {
        try
        {
            _context.FeeTypes.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFeeType), new { id = model.FeeTypeId }, model);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPut("feetypes/{id}")]
    public async Task<IActionResult> UpdateFeeType(int id, FeeType model)
    {
        if (id != model.FeeTypeId)
        {
            return BadRequest();
        }

        try
        {
            await _feeService.UpdateFeeTypeAsync(model);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpDelete("feetypes/{id}")]
    public async Task<IActionResult> removeFeeType(int id)
    {
        try
        {
            var result = await _feeService.DeleteFeeTypeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

  
}