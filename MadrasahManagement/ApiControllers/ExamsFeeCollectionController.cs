using FastReport;
using FastReport.Export.PdfSimple;
using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamFeesCollectionController : ControllerBase
    {
        private readonly MadrasahDbContext _context;

        public ExamFeesCollectionController(MadrasahDbContext context)
        {
            _context = context;
        }

        // ------------------- CREATE -------------------
        [HttpPost]
        public async Task<ActionResult<ExamFeesReadDto>> Create([FromBody] ExamFeesCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var examFee = new ExamFee
            {
                EducationYear = dto.EducationYear,
                ClassId = dto.ClassId,
                ExamId = dto.ExamId,
                ExamAmount = dto.ExamAmount,
                FeeCollections = dto.FeeCollections.Select(fc => new ExamFeeCollection
                {
                    StudentId = fc.StudentId,
                    ExamFeeAmount = fc.ExamFeeAmount,
                    TotalSubject = fc.TotalSubject
                }).ToList()
            };

            _context.ExamFees.Add(examFee);
            await _context.SaveChangesAsync();

            return await GetById(examFee.ExamFeeId);
        }

        // ------------------- READ ALL -------------------
        [HttpGet]
        public async Task<ActionResult<List<ExamFeesReadDto>>> GetAll()
        {
            var examFees = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.FeeCollections)
                    .ThenInclude(fc => fc.Student)
                .Select(e => new ExamFeesReadDto
                {
                    ExamFeeId = e.ExamFeeId,
                    EducationYear = e.EducationYear,
                    ClassId = e.ClassId,
                    ClassName = e.Class.ClassName,
                    ExamId = e.ExamId,
                    ExamName = e.Examination.ExamName,
                    ExamAmount = e.ExamAmount,
                    FeeCollections = e.FeeCollections.Select(fc => new ExamFeeCollectionReadDto
                    {
                        FeeCollectionId = fc.FeeCollectionId,
                        StudentId = fc.StudentId,
                        StudentName = fc.Student.StudentName,
                        ExamFeeAmount = fc.ExamFeeAmount,
                        TotalSubject = fc.TotalSubject
                    }).ToList()
                })
                .ToListAsync();

            return Ok(examFees);
        }

        // ------------------- READ BY ID -------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamFeesReadDto>> GetById(int id)
        {
            var examFee = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.FeeCollections)
                    .ThenInclude(fc => fc.Student)
                .Where(e => e.ExamFeeId == id)
                .Select(e => new ExamFeesReadDto
                {
                    ExamFeeId = e.ExamFeeId,
                    EducationYear = e.EducationYear,
                    ClassId = e.ClassId,
                    ClassName = e.Class.ClassName,
                    ExamId = e.ExamId,
                    ExamName = e.Examination.ExamName,
                    ExamAmount = e.ExamAmount,
                    FeeCollections = e.FeeCollections.Select(fc => new ExamFeeCollectionReadDto
                    {
                        FeeCollectionId = fc.FeeCollectionId,
                        StudentId = fc.StudentId,
                        StudentName = fc.Student.StudentName,
                        ExamFeeAmount = fc.ExamFeeAmount,
                        TotalSubject = fc.TotalSubject
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (examFee == null) return NotFound();
            return Ok(examFee);
        }

        // ------------------- UPDATE -------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<ExamFeesReadDto>> Update(int id, [FromBody] ExamFeesUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _context.ExamFees
                .Include(e => e.FeeCollections)
                .FirstOrDefaultAsync(e => e.ExamFeeId == id);

            if (existing == null) return NotFound();

            existing.EducationYear = dto.EducationYear;
            existing.ClassId = dto.ClassId;
            existing.ExamId = dto.ExamId;
            existing.ExamAmount = dto.ExamAmount;

            // Update FeeCollections
            foreach (var fcDto in dto.FeeCollections)
            {
                if (fcDto.FeeCollectionId.HasValue)
                {
                    var existingFc = existing.FeeCollections.FirstOrDefault(f => f.FeeCollectionId == fcDto.FeeCollectionId.Value);
                    if (existingFc != null)
                    {
                        existingFc.StudentId = fcDto.StudentId;
                        existingFc.ExamFeeAmount = fcDto.ExamFeeAmount;
                        existingFc.TotalSubject = fcDto.TotalSubject;
                    }
                    else
                    {
                        existing.FeeCollections.Add(new ExamFeeCollection
                        {
                            StudentId = fcDto.StudentId,
                            ExamFeeAmount = fcDto.ExamFeeAmount,
                            TotalSubject = fcDto.TotalSubject
                        });
                    }
                }
                else
                {
                    existing.FeeCollections.Add(new ExamFeeCollection
                    {
                        StudentId = fcDto.StudentId,
                        ExamFeeAmount = fcDto.ExamFeeAmount,
                        TotalSubject = fcDto.TotalSubject
                    });
                }
            }

            await _context.SaveChangesAsync();

            return await GetById(id);
        }

        // ------------------- DELETE -------------------
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _context.ExamFees.FindAsync(id);
            if (existing == null) return NotFound();

            _context.ExamFees.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpGet("report/{id}")]
        public async Task<IActionResult> GenerateReport(int id)
        {
            var examFee = await _context.ExamFees
                .Include(e => e.Class)
                .Include(e => e.Examination)
                .Include(e => e.FeeCollections)
                    .ThenInclude(fc => fc.Student)
                .FirstOrDefaultAsync(e => e.ExamFeeId == id);

            if (examFee == null) return NotFound();

            // Prepare Master Data (Class, Exam, Year, TotalCollected)
            var masterTable = new System.Data.DataTable("ExamInfo");
            masterTable.Columns.Add("ClassName", typeof(string));
            masterTable.Columns.Add("ExamName", typeof(string));
            masterTable.Columns.Add("EducationYear", typeof(string));
            masterTable.Columns.Add("TotalCollected", typeof(decimal));

            var totalCollected = examFee.FeeCollections.Sum(fc => fc.ExamFeeAmount);
            masterTable.Rows.Add(examFee.Class.ClassName, examFee.Examination.ExamName, examFee.EducationYear, totalCollected);

            // Prepare Detail Data (Student info)
            var detailTable = new System.Data.DataTable("FeeCollection");
            detailTable.Columns.Add("StudentName", typeof(string));
            detailTable.Columns.Add("ExamFeeAmount", typeof(decimal));
            detailTable.Columns.Add("TotalSubject", typeof(int));

            foreach (var fc in examFee.FeeCollections)
                detailTable.Rows.Add(fc.Student.StudentName, fc.ExamFeeAmount, fc.TotalSubject);

            try
            {
                using var report = new Report();

                // Load your existing .frx template
                report.Load("wwwroot/reports/ExamFee.frx");

                // Register data
                report.RegisterData(masterTable, "ExamInfo");
                report.RegisterData(detailTable, "FeeCollection");

                report.GetDataSource("ExamInfo").Enabled = true;
                report.GetDataSource("FeeCollection").Enabled = true;

                using var pdfExport = new PDFSimpleExport();
                using var ms = new MemoryStream();
                report.Prepare();
                report.Export(pdfExport, ms);
                ms.Position = 0;

                return File(ms.ToArray(), "application/pdf", $"ExamFee_{examFee.ExamFeeId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Report generation failed: {ex.Message}");
            }
        }

    }
}
