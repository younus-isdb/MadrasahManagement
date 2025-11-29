using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;

namespace MadrasahManagement.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly MadrasahDbContext _context;

        public DepartmentController(MadrasahDbContext context)
        {
            _context = context;
        }
        // 🔥 Modal Load Action
        [HttpGet]
        public IActionResult CreateModal()
        {
            return PartialView("_DepartmentCreatePartial", new Department());
        }

        // 🔥 POST: Create Department
        [HttpPost]
        public IActionResult Create(Department model)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(model);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return PartialView("_DepartmentCreatePartial", model);
        }

        // 🔥 Dropdown Refresh এর জন্য API
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _context.Departments
                .Select(d => new { d.DepartmentId, d.DepartmentName })
                .ToList();

            return Json(list);
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .Include(d => d.Classes)
                .Include(d => d.Teachers)
                .ToListAsync();

            return View(departments);
        }


        // GET: Department/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .Include(d => d.Classes)
                .Include(d => d.Teachers)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null) return NotFound();

            return View(department);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        //// POST: Department/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Department department)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(department);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(department);
        //}

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            return View(department);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.DepartmentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null) return NotFound();

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(d => d.DepartmentId == id);
        }


    }
}
