using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadrasahManagement.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    public class Examination
    {
        [Key]
        public int ExamId { get; set; }

        [Required, MaxLength(150)]
        public string ExamName { get; set; } = string.Empty;

        // Initialize to prevent null reference errors
        [JsonIgnore]
        public virtual ICollection<ExamFee> ExamFees { get; set; } = new List<ExamFee>();
        //[JsonIgnore]
        //public ICollection<ExamResult> ExamResults { get; set; } = new HashSet<ExamResult>();
        [JsonIgnore]
        public virtual ICollection<PointCondition> PointConditions { get; set; } = new List<PointCondition>();
        [JsonIgnore]
        public virtual ICollection<ExamRoutine> ExamRoutine { get; set; } = new List<ExamRoutine>();

    }

    public class ExamFee
    {
        [Key]
        public int ExamFeeId { get; set; }
        public string EducationYear { get; set; } = string.Empty;

        public int ClassId { get; set; }
        [ForeignKey(nameof(ClassId))]
        public Class Class { get; set; } = null!;

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        public int ExamId { get; set; }
        [ForeignKey(nameof(ExamId))]
        public Examination Examination { get; set; } = null!;

        public decimal ExamAmount { get; set; }

        public ICollection<ExamFeeCollection> FeeCollections { get; set; } = new List<ExamFeeCollection>();
    }

    public class ExamFeeCollection
    {
        [Key]
        public int FeeCollectionId { get; set; }

        public int ExamFeeId { get; set; }
        public ExamFee ExamFee { get; set; } = null!;
        [Required]
        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        public decimal ExamFeeAmount { get; set; }
        public int TotalSubject { get; set; }
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

        [Required, StringLength(10)] // Added length constraint
        public string EducationYear { get; set; } = string.Empty;

        public int ClassId { get; set; }
        [ForeignKey(nameof(ClassId))]
        public virtual Class Class { get; set; } = null!; // Use virtual for lazy loading support if needed

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; } = null!;

        public int ExamId { get; set; }
        [ForeignKey(nameof(ExamId))]
        public virtual Examination Examination { get; set; } = null!;

        public int SubjectId { get; set; }
        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; } = null!;

        public int PassMarks { get; set; }
        public int HighestMark { get; set; }

        // Navigation
        public virtual ICollection<PointConditionDetail> Details { get; set; } = new List<PointConditionDetail>();
    }

    public class PointConditionDetail
    {
        [Key]
        public int PointConditionDetailId { get; set; }

        public int PointConditionId { get; set; }
        [ForeignKey(nameof(PointConditionId))]
        public virtual PointCondition PointCondition { get; set; } = null!;

        public int FromMark { get; set; }
        public int ToMark { get; set; }

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
        public string EducationYear{ get; set; }


        [Required]
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; }


        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        [Required]
        public int ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Examination? Examination { get; set; }

        public int SubjectId { get; set; }
        [ForeignKey(nameof(SubjectId))]
        public Subject? Subject { get; set; }
        public int RoomNumber { get; set; }

        [Required]
        public DateTime ExamDate { get; set; }=DateTime.Now;
        public string ExamDay {  get; set; }= string.Empty;
        public string ExamStartTime { get; set; }= string.Empty;
        public string ExamEndTime { get; set; }= string.Empty;

    }
   
    public class ExamIncomeExpense
    {
        [Key]
        public int IncomeExpenseId { get; set; }

        public int ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Examination? Examination { get; set; }
        public string  TypesOfExpense { get; set; }
        public decimal Amount { get; set; }

         
    }
    public class SeatPlan
    {
        [Key]
        public int SeatPlanId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExamDate { get; set; }
        public int RoomNumber { get; set; }
        public int NumberOfRows {  get; set; }
        public int StudentsPerBench {  get; set; }

        // Student reference
        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;

        public int ClassId { get; set; }
        [ForeignKey(nameof(ClassId))]
        public virtual Class Class { get; set; } = null!;

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; } = null!;

        public int? SubjectId { get; set; }
        [ForeignKey(nameof(SubjectId))]
        public virtual Subject? Subject { get; set; }

        [NotMapped]
        public string StudentRoll { get; set; } = ""; // Seat number / Roll display (view only)
    }
}

