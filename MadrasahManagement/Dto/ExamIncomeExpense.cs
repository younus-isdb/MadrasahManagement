using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    public class ExamIncomeExpenseCreateDto
    {
        
        [Required]
        public int ExamId { get; set; }
        public string TypesOfExpense { get; set; }
        public decimal Amount { get; set; } 

    }
    public class ExamIncomeExpenseUpdateDto : ExamIncomeExpenseCreateDto
    {
        [Required]
        public int IncomeExpenseId { get; set; } 
    }
    public class ExamIncomeExpenseReadDto
    {
        public int IncomeExpenseId { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string TypesOfExpense { get; set; }
        public decimal Amount { get; set; }

    }
}
