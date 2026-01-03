using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    public class ExamExpensesController : Controller
    {
        private readonly MadrasahDbContext _context;

        public ExamExpensesController(MadrasahDbContext context)
        {
            _context = context;
        }

        // ================= LIST =================
        public async Task<IActionResult> Index()
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
            ViewBag.TotalAmount = data.Sum(x => x.Amount);
            return View(data); // Pass to Index.cshtml
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
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

            return View(dto); // Pass to Details.cshtml
        }

        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Exams = _context.Examinations.ToList(); // Dropdown for Exam selection
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamIncomeExpenseCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Exams = _context.Examinations.ToList();
                return View(dto);
            }

            var entity = new ExamIncomeExpense
            {
                ExamId = dto.ExamId,
                TypesOfExpense = dto.TypesOfExpense,
                Amount = dto.Amount
            };

            _context.ExamIncomeExpenses.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var existing = await _context.ExamIncomeExpenses.FindAsync(id);
            if (existing == null) return NotFound();

            ViewBag.Exams = _context.Examinations.ToList();

            var dto = new ExamIncomeExpenseUpdateDto
            {
                IncomeExpenseId = existing.IncomeExpenseId,
                ExamId = existing.ExamId,
                TypesOfExpense = existing.TypesOfExpense,
                Amount = existing.Amount
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamIncomeExpenseUpdateDto dto)
        {
            if (id != dto.IncomeExpenseId) return BadRequest("Id mismatch");

            if (!ModelState.IsValid)
            {
                ViewBag.Exams = _context.Examinations.ToList();
                return View(dto);
            }

            var existing = await _context.ExamIncomeExpenses.FindAsync(id);
            if (existing == null) return NotFound();

            existing.ExamId = dto.ExamId;
            existing.TypesOfExpense = dto.TypesOfExpense;
            existing.Amount = dto.Amount;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
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

            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existing = await _context.ExamIncomeExpenses.FindAsync(id);
            if (existing == null) return NotFound();

            _context.ExamIncomeExpenses.Remove(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
