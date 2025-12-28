using MadrasahManagement.Models;

namespace MadrasahManagement.ApiDto
{
    public class SalaryApiDto
    {
       
            public int? TeacherId { get; set; }
            public int? StaffId { get; set; }
            public decimal BasicSalary { get; set; }
            public decimal Allowances { get; set; }
            public decimal Deductions { get; set; }
            public Month MonthName { get; set; }
            public int Year { get; set; }
            public PaymentMethodType PaymentMethod { get; set; }
       
    }
    public class UpdateSalaryApiDto
    {
        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public Month MonthName { get; set; }
        public int Year { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
    }
}
