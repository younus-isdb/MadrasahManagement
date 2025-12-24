using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadrasahManagement.Models
{
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Examination
    {
        [Key]
        public int ExamId { get; set; }

        [Required, MaxLength(150)]
        public string ExamName { get; set; } = string.Empty;

        // Initialize to prevent null reference errors
        public virtual ICollection<ExamFee> ExamFees { get; set; } = new List<ExamFee>();
        public virtual ICollection<PointCondition> PointConditions { get; set; } = new List<PointCondition>();
        public virtual ICollection<ExamFeeCollection> ExamFeeCollections { get; set; } = new List<ExamFeeCollection>();

    }

    public class ExamFee
    {
        [Key]
        public int ExamFeeId { get; set; }

        [Required, MaxLength(10)] // e.g., "2024-2025"
        public string EducationYear { get; set; } = string.Empty;

        [Required]
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; }

        [Required]
        public int ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Examination? Examination { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal ExamAmount { get; set; }
        public virtual ICollection<ExamFeeCollection> ExamFeeCollections { get; set; } = new List<ExamFeeCollection>();
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
        public string EducationYear { get; set; } = string.Empty;

        [Required]
        public int ClassId { get; set; }
        [ForeignKey(nameof(ClassId))]
        public Class? Class { get; set; }

        [Required]
        public int ExamId { get; set; }
        [ForeignKey(nameof(ExamId))]
        public Examination? Examination { get; set; }

        //[Required]
        //public int SubjectId { get; set; }
        //[ForeignKey(nameof(SubjectId))]
        //public Subject? Subject { get; set; }

        [Required]
        public int PassMarks { get; set; }

        [Required]
        public int HighestMark { get; set; }

        // Navigation
        public ICollection<PointConditionDetail> Details { get; set; } = new List<PointConditionDetail>();
    }
    public class PointConditionDetail
    {
        [Key]
        public int PointConditionDetailId { get; set; }

        [Required]
        public int PointConditionId { get; set; }
        [ForeignKey(nameof(PointConditionId))]
        public PointCondition? PointCondition { get; set; }

        [Required]
        public int FromMark { get; set; }   // >=
        [Required]
        public int ToMark { get; set; }     // <=

        [Required]
        public string Division { get; set; } = string.Empty;

        public bool IsSilverColor { get; set; }
    }



    //public string? Grade { get; set; }
    //private string CalculateGrade(int marks)
    //{
    //    if (marks >= 80) return "A+";
    //    if (marks >= 70) return "A";
    //    if (marks >= 60) return "A-";
    //    if (marks >= 50) return "B";
    //    if (marks >= 40) return "C";
    //    return "F";
    //}



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
        [Required]
        public string EducationYear { get; set; } = string.Empty;
        [Required]
        public int ExamId { get; set; }
        [ForeignKey(nameof(ExamId))]
        public Examination? Examination { get; set; }

        public int ClassId { get; set; }
        [ForeignKey(nameof(ClassId))]
        public Class? Class { get; set; }
        public string TotalSubject { get; set; } = string.Empty;

       public int ExamFee { get; set; }
        //[Required]
        //public int StudentId { get; set; }
        //[ForeignKey(nameof(StudentId))]
        //public Student? Student { get; set; }
        


        //[Required]
        //public int SubjectId { get; set; }
        //[ForeignKey(nameof(SubjectId))]
        //public Subject? Subject { get; set; }


        
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

