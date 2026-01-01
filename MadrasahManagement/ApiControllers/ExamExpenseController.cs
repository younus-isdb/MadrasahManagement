using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamExpenseController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExamExpenseController(MadrasahDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.ExamIncomeExpenses

                .Include(e => e.Examination)

                .Select(x => new ExamIncomeExpenseReadDto
                {
                    IncomeExpenseId = x.IncomeExpenseId,
                    ExamId = x.ExamId,
                    ExamName = x.Examination!.ExamName,
                    TypesOfExpense = x.TypesOfExpense,
                    Amount = x.Amount,
                }).ToListAsync();

            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var x = await _context.ExamIncomeExpenses

                .Include(e => e.Examination)

                .FirstOrDefaultAsync(e => e.IncomeExpenseId == id);

            if (x == null) return NotFound();

            var dto = new ExamIncomeExpenseReadDto
            {
                IncomeExpenseId = x.IncomeExpenseId,
                ExamId = x.ExamId,
                ExamName = x.Examination!.ExamName,
                TypesOfExpense = x.TypesOfExpense,
                Amount = x.Amount
            };

            return Ok(dto);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExamIncomeExpenseCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new ExamIncomeExpense
            {
                ExamId = dto.ExamId,
                TypesOfExpense = dto.TypesOfExpense,
                Amount = dto.Amount
            };

            _context.ExamIncomeExpenses.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.IncomeExpenseId }, entity);
        }


    }
}
