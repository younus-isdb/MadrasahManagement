using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadrasahManagement.Models
{
    public class Examination
    {
        [Key]
        public int ExamId { get; set; }

        [Required, MaxLength(150)]
        public string ExamName { get; set; } = string.Empty;
        public ICollection<ExamFee>? ExamFees { get; set; }


    }
    public class ExamFee
    {
        [Key]
        public int ExamFeeId { get; set; }

        [Required]
        public string EducationYear { get; set; } = string.Empty;

        // 🔹 Class Table Relation
        [Required]
        public int ClassId { get; set; }
        public Class? Class { get; set; }

        // 🔹 Examination Table Relation
        [Required]
        public int ExamId { get; set; }
        public Examination? Examination { get; set; }

        [Required]
        public decimal ExamFees { get; set; }
    }

    public class SubClassGroup
    {
        [Key]
        public int SubClassGroupId { get; set; }

        [Required]
        public string GroupName { get; set; } = string.Empty;
    }
    public class PointCondition
    {
        [Key]
        public int PointConditionId { get; set; }

        [Required]
        public int ObtainedMarks { get; set; }



        public string? Grade { get; set; }
        private string CalculateGrade(int marks)
        {
            if (marks >= 80) return "A+";
            if (marks >= 70) return "A";
            if (marks >= 60) return "A-";
            if (marks >= 50) return "B";
            if (marks >= 40) return "C";
            return "F";
        }

    }

    public class MeritCondition
    {
        [Key]
        public int MeritConditionId { get; set; }

        public int FromMerit { get; set; }
        public int ToMerit { get; set; }
    }
    public class ExamRoutine
    {
        [Key]
        public int ExamRoutineId { get; set; }

        public int ExamId { get; set; }

        [Required]
        public DateTime ExamDate { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;
    }
    public class ExamFeeCollection
    {
        [Key]
        public int FeeCollectionId { get; set; }
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        public decimal PaidAmount { get; set; }

        public DateTime PaidDate { get; set; }
    }
    public class ExamIncomeExpense
    {
        [Key]
        public int IncomeExpenseId { get; set; }

        public int ExamId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; } = "Income"; // Income / Expense
    }
}

