using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    public class ExaminationFeeController : Controller
    {
        private readonly MadrasahDbContext _context;

        public ExaminationFeeController(MadrasahDbContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var data = await _context.ExamFees
                            .Include(f => f.Class)
                            .Include(f => f.Department)
                            .Include(f => f.Examination)
                            .ToListAsync();
            return View(data);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamFeeCreateDto examFeeDto)
        {
            if (ModelState.IsValid)
            {
                // Convert DTO to Entity
                var examFee = new ExamFee
                {
                    EducationYear = examFeeDto.EducationYear,
                    ClassId = examFeeDto.ClassId,
                    DepartmentId = examFeeDto.DepartmentId,
                    ExamId = examFeeDto.ExamId,
                    ExamAmount = examFeeDto.ExamAmount
                };

                _context.Add(examFee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(examFeeDto); 
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var examFee = await _context.ExamFees.FindAsync(id);
            if (examFee == null) return NotFound();

            LoadDropdowns(examFee.DepartmentId);
            return View(examFee);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamFee examFee)
        {
            if (id != examFee.ExamFeeId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(examFee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(examFee.DepartmentId);
            return View(examFee);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var examFee = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Department)
                .Include(e => e.Examination)
                .FirstOrDefaultAsync(m => m.ExamFeeId == id);

            if (examFee == null) return NotFound();

            return View(examFee);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examFee = await _context.ExamFees.FindAsync(id);
            if (examFee == null) return NotFound();

            _context.ExamFees.Remove(examFee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: API endpoint to get classes by department
        public async Task<IActionResult> GetClassesByDepartment(int departmentId)
        {
            var classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId)
                .Select(c => new { c.ClassId, c.ClassName })
                .ToListAsync();

            return Json(classes);
        }

        // 🔹 Updated Dropdown Helper
        private void LoadDropdowns(int? selectedDepartmentId = null)
        {
            ViewBag.DepartmentId = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewBag.ExamId = new SelectList(_context.Examinations, "ExamId", "ExamName");

            // Load classes based on selected department (for edit view)
            if (selectedDepartmentId.HasValue)
            {
                var classes = _context.Classes
                    .Where(c => c.DepartmentId == selectedDepartmentId.Value)
                    .ToList();
                ViewBag.ClassId = new SelectList(classes, "ClassId", "ClassName");
            }
            else
            {
                ViewBag.ClassId = new SelectList(new List<Class>(), "ClassId", "ClassName");
            }
        }
    }
}