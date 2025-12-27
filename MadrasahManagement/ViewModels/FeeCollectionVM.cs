using MadrasahManagement.Models;

namespace MadrasahManagement.ViewModels
{
    public class FeeCollectionVM
    {
        public List<Student> Students { get; set; }
        public List<FeeType> FeeTypes { get; set; }
        public int SelectedStudentId { get; set; }
        public int SelectedFeeTypeId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
    public class SalaryPaymentVM
    {
        public int StaffId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
    public class MonthlyReportVM
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal TotalAmount { get; set; }
        public int Count { get; set; }
    }
}
