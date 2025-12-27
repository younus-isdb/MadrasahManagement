using MadrasahManagement.Models;

namespace MadrasahManagement.Dto
{
   
    public class FeeCollectionDto
    {
        public int StudentId { get; set; }
        public int FeeTypeId { get; set; }
        public decimal AmountPaid { get; set; }
        public PaymentMethodType PaymentMethod { get; set; } = PaymentMethodType.Cash;
        public string Remarks { get; set; }
        public DateTime DatePaid { get; set; } = DateTime.Now;
    }

    public class StudentFeeStatusDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public decimal TotalFee { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
    }

    public class OutstandingFeeDto  //foe due
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string ParentContact { get; set; }
        public decimal TotalDue { get; set; }
    }

   
    public class CollectionDetailDto
    {
        public DateTime Date { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string FeeType { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public string ReceiptNumber { get; set; }
    }

    public class PartialPaymentDto
    {
        public int StudentId { get; set; }
        public int FeeTypeId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public string Remarks { get; set; }
    }

    public class FeeSummaryDto
    {
        public string FeeTypeName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Balance { get; set; }
        public PaymentStatus Status { get; set; }
    }

    public class FeeCollectionReportDto
    {
        public DateTime ReportDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalCollection { get; set; }
        public int TotalTransactions { get; set; }
        public List<CollectionDetailDto> CollectionDetails { get; set; }
    }

    public class DailyCollectionSummaryDto
    {
        public DateTime Date { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalBank { get; set; }
        public decimal TotalOnline { get; set; }
        public decimal GrandTotal { get; set; }
        public int TransactionCount { get; set; }
    }

    public class MonthlyCollectionSummaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalCollection { get; set; }
        public Dictionary<string, decimal> ClassWiseCollection { get; set; }
        public Dictionary<string, decimal> PaymentMethodWise { get; set; }
    }

    public class BulkFeeCollectionDto
    {
        public int StudentId { get; set; }
        public int FeeTypeId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
    }
}
