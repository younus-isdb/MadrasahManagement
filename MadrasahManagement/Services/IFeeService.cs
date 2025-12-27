using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Services
{
    public interface IFeeService
    {
        Task<IEnumerable<FeeType>> GetAllFeeTypesAsync();
        Task<FeeType> GetFeeTypeByIdAsync(int id);
        Task<FeeType> CreateFeeTypeAsync(FeeType feeType);
        Task<bool> DeleteFeeTypeAsync(int id);
        Task<bool> UpdateFeeTypeAsync(FeeType feeType);
        Task<IEnumerable<FeeType>> GetFeeTypesByClassAsync(int classId);

        // Fee Collection
        Task<FeeCollection> CollectFeeAsync(FeeCollectionDto feeCollectionDto);
        Task<FeeCollection> GetFeeCollectionByIdAsync(int id);
        Task<IEnumerable<FeeCollection>> GetAllFeeCollectionsAsync();
        Task<IEnumerable<FeeCollection>> GetFeeCollectionsByStudentAsync(int studentId);
        Task<IEnumerable<CollectionDetailDto>> GetFeeCollectionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<FeeCollection>> GetTodaysCollectionsAsync();
        Task<decimal> GetTotalCollectionByMonthAsync(int month, int year);

        // Student Fee Operations
        Task<decimal> GetTotalFeeDueByStudentAsync(int studentId);
        Task<decimal> GetTotalFeePaidByStudentAsync(int studentId);
        Task<decimal> GetBalanceDueByStudentAsync(int studentId);

        // Reports
        Task<IEnumerable<StudentFeeStatusDto>> GetClassFeeStatusAsync(int classId);
        Task<IEnumerable<OutstandingFeeDto>> GetOutstandingFeesAsync(int? classId = null);

        // Utilities
        Task<bool> IsFeeAlreadyPaidAsync(int studentId, int feeTypeId, int month, int year);
        Task<string> GenerateReceiptNumberAsync();
    }
    public class FeeService : IFeeService
    {
        private readonly MadrasahDbContext _context;
        private readonly ILogger<FeeService> _logger;

        public FeeService(MadrasahDbContext context, ILogger<FeeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ==================== Fee Type Management ====================
        public async Task<IEnumerable<FeeType>> GetAllFeeTypesAsync()
        {
            return await _context.FeeTypes
                .Include(ft => ft.Class)
                .ToListAsync();
        }

        public async Task<FeeType> GetFeeTypeByIdAsync(int id)
        {
            return await _context.FeeTypes
                .Include(ft => ft.Class)
                .FirstOrDefaultAsync(ft => ft.FeeTypeId == id);
        }

        public async Task<FeeType> CreateFeeTypeAsync(FeeType feeType)
        {
            _context.FeeTypes.Add(feeType);
            await _context.SaveChangesAsync();
            return feeType;
        }

        public async Task<bool> DeleteFeeTypeAsync(int id)
        {
            var feeType = await _context.FeeTypes.FindAsync(id);
            if (feeType == null) return false;

            _context.FeeTypes.Remove(feeType);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateFeeTypeAsync(FeeType feeType)
        {
            var existingFeeType = await _context.FeeTypes.FindAsync(feeType.FeeTypeId);
            if (existingFeeType == null) return false;

            // Update properties
            existingFeeType.Name = feeType.Name;
            existingFeeType.ClassId = feeType.ClassId;
            existingFeeType.Amount = feeType.Amount;
            existingFeeType.Frequency = feeType.Frequency;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<FeeType>> GetFeeTypesByClassAsync(int classId)
        {
            return await _context.FeeTypes
                .Where(ft => ft.ClassId == classId)
                .ToListAsync();
        }

        // ==================== Fee Collection ====================
        public async Task<FeeCollection> CollectFeeAsync(FeeCollectionDto dto)
        {
            var feeCollection = new FeeCollection
            {
                StudentId = dto.StudentId,
                FeeTypeId = dto.FeeTypeId,
                AmountPaid = dto.AmountPaid,
                PaymentMethod = dto.PaymentMethod,
                Remarks = dto.Remarks,
                DatePaid = dto.DatePaid,
                Status = PaymentStatus.Paid,
                ReceiptNumber = await GenerateReceiptNumberAsync()
            };

            _context.FeeCollections.Add(feeCollection);
            await _context.SaveChangesAsync();
            return feeCollection;
        }
        public async Task<FeeCollection> GetFeeCollectionByIdAsync(int id)
        {
            return await _context.FeeCollections
                .Include(fc => fc.Student)
                .Include(fc => fc.FeeType)
                .FirstOrDefaultAsync(fc => fc.Id == id);
        }

        public async Task<IEnumerable<FeeCollection>> GetAllFeeCollectionsAsync()
        {
            return await _context.FeeCollections
                .Include(fc => fc.Student)
                .Include(fc => fc.FeeType)
                .OrderByDescending(fc => fc.DatePaid)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeeCollection>> GetFeeCollectionsByStudentAsync(int studentId)
        {
            return await _context.FeeCollections
                .Include(fc => fc.FeeType)
                .Where(fc => fc.StudentId == studentId)
                .OrderByDescending(fc => fc.DatePaid)
                .ToListAsync();
        }

        public async Task<IEnumerable<CollectionDetailDto>> GetFeeCollectionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var collections = await _context.FeeCollections
                .Include(fc => fc.Student)
                .ThenInclude(s => s.Class)
                .Include(fc => fc.FeeType)
                .Where(fc => fc.DatePaid >= startDate && fc.DatePaid <= endDate)
                .OrderByDescending(fc => fc.DatePaid)
                .ToListAsync();

            return collections.Select(c => new CollectionDetailDto
            {
                Date = c.DatePaid,
                StudentName = c.Student.StudentName,
                ClassName = c.Student.Class.ClassName,
                FeeType = c.FeeType.Name,
                Amount = c.AmountPaid,
                PaymentMethod = c.PaymentMethod,
               ReceiptNumber = c.ReceiptNumber
            }).ToList();
        }


        public async Task<IEnumerable<FeeCollection>> GetTodaysCollectionsAsync()
        {
            var today = DateTime.Today;
            return await _context.FeeCollections
                .Include(fc => fc.Student)
                .Include(fc => fc.FeeType)
                .Where(fc => fc.DatePaid.Date == today)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalCollectionByMonthAsync(int month, int year)
        {
            return await _context.FeeCollections
                .Where(fc => fc.DatePaid.Month == month &&
                            fc.DatePaid.Year == year)
                .SumAsync(fc => fc.AmountPaid);
        }

        // ==================== Student Fee Operations ====================
        public async Task<decimal> GetTotalFeeDueByStudentAsync(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .ThenInclude(c => c.FeeTypes)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            return student?.Class?.FeeTypes?.Sum(ft => ft.Amount) ?? 0;
        }

        public async Task<decimal> GetTotalFeePaidByStudentAsync(int studentId)
        {
            return await _context.FeeCollections
                .Where(fc => fc.StudentId == studentId)
                .SumAsync(fc => fc.AmountPaid);
        }

        public async Task<decimal> GetBalanceDueByStudentAsync(int studentId)
        {
            var totalDue = await GetTotalFeeDueByStudentAsync(studentId);
            var totalPaid = await GetTotalFeePaidByStudentAsync(studentId);
            return totalDue - totalPaid;
        }

        // ==================== Reports ====================
        public async Task<IEnumerable<StudentFeeStatusDto>> GetClassFeeStatusAsync(int classId)
        {
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .ToListAsync();

            var statusList = new List<StudentFeeStatusDto>();

            foreach (var student in students)
            {
                var totalDue = await GetTotalFeeDueByStudentAsync(student.StudentId);
                var totalPaid = await GetTotalFeePaidByStudentAsync(student.StudentId);
                var balance = totalDue - totalPaid;

                statusList.Add(new StudentFeeStatusDto
                {
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    RollNumber = student.RegNo,
                    TotalFee = totalDue,
                    TotalPaid = totalPaid,
                    Balance = balance,
                    Status = balance <= 0 ? "Paid" : "Pending"
                });
            }

            return statusList;
        }

        public async Task<IEnumerable<OutstandingFeeDto>> GetOutstandingFeesAsync(int? classId = null)
        {
            var query = _context.Students.AsQueryable();

            if (classId.HasValue)
            {
                query = query.Where(s => s.ClassId == classId.Value);
            }

            var students = await query.ToListAsync();
            var outstandingFees = new List<OutstandingFeeDto>();

            foreach (var student in students)
            {
                var balance = await GetBalanceDueByStudentAsync(student.StudentId);

                if (balance > 0)
                {
                    outstandingFees.Add(new OutstandingFeeDto
                    {
                        StudentId = student.StudentId,
                        StudentName = student.StudentName,
                        ClassName = student.Class?.ClassName ?? "N/A",
                        ParentContact = student.FatherPhone ?? "N/A",
                        TotalDue = balance
                    });
                }
            }

            return outstandingFees;
        }

        // ==================== Validation & Utilities ====================
        public async Task<bool> IsFeeAlreadyPaidAsync(int studentId, int feeTypeId, int month, int year)
        {
            return await _context.FeeCollections
                .AnyAsync(fc => fc.StudentId == studentId &&
                               fc.FeeTypeId == feeTypeId &&
                               fc.DatePaid.Month == month &&
                               fc.DatePaid.Year == year);
        }

        public async Task<string> GenerateReceiptNumberAsync()
        {
            var today = DateTime.Today;
            var count = await _context.FeeCollections
                .CountAsync(fc => fc.DatePaid.Date == today);

            return $"REC-{today:yyyyMMdd}-{count + 1:0000}";
        }
    }

}
