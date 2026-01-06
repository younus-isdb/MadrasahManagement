//using FastReport;
//using FastReport.Export.Pdf;
//using MadrasahManagement.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.IO;

//namespace MadrasahManagement.Controllers
//{
//    public class ReportsController : Controller
//    {
//        private readonly MadrasahDbContext _context;
//        private readonly IWebHostEnvironment _env;

//        public ReportsController(MadrasahDbContext context, IWebHostEnvironment env)
//        {
//            _context = context;
//            _env = env;
//        }

//        // Report selection page
//        public IActionResult Index()
//        {
//            ViewBag.Departments = _context.Departments.ToList();
//            ViewBag.Classes = _context.Classes.ToList();
//            return View();
//        }

//        // Generate filtered Exam Results PDF
//        public IActionResult ExamResults(int departmentId = 0, int classId = 0, bool inline = false)
//        {
//            // Fetch results with related entities
//            var results = _context.ExamResults
//                .Include(r => r.Student)
//                .Include(r => r.Class)
//                .Include(r => r.Examination)
//                .Include(r => r.ResultDetails)
//                    .ThenInclude(d => d.Subject)
//                .AsQueryable();

//            if (departmentId > 0)
//                results = results.Where(r => r.Student.DepartmentId == departmentId);

//            if (classId > 0)
//                results = results.Where(r => r.ClassId == classId);

//            var data = results.ToList();

//            if (!data.Any())
//                return Content("No exam results found for selected department/class.");

//            // Initialize FastReport
//            var report = new Report();
//            report.RegisterData(data, "ExamResults", true);

//            // Load .frx template
//            string reportPath = Path.Combine(_env.WebRootPath, "Reports", "ExamResults.frx");
//            if (!System.IO.File.Exists(reportPath))
//                return NotFound("Report template not found.");

//            report.Load(reportPath);

//            // Optional: set parameters in report
//            report.SetParameterValue("DepartmentId", departmentId);
//            report.SetParameterValue("ClassId", classId);

//            // Prepare report
//            report.Prepare();

//            // Export to PDF
//            using var ms = new MemoryStream();
//            report.Export(new PDFExport(), ms);
//            ms.Position = 0;

//            if (inline)
//                return File(ms.ToArray(), "application/pdf"); // browser view
//            else
//                return File(ms.ToArray(), "application/pdf", "ExamResults.pdf"); // download
//        }
//    }
//}
