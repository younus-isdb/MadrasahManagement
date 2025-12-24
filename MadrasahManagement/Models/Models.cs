using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MadrasahManagement.Models
{

    public class AppUser : IdentityUser
    {
        // 1. ---------- BASIC PROFILE ----------

        // ইউজারের পুরো নাম
        public string? FullName { get; set; }

        // আরবি নাম — মাদরাসা সিস্টেমে প্রয়োজনীয়
        public string? ArabicName { get; set; }

        // প্রোফাইল ছবি (URL / file path)
        public string? ProfilePicture { get; set; }

        // লিঙ্গ — Male/Female
        public string? Gender { get; set; }

        // জন্ম তারিখ
        public DateTime? DateOfBirth { get; set; }


        // ---------- ORGANIZATIONAL INFO ----------

        // কোন বিভাগে ইউজার কাজ করে/পড়াশোনা করে
        public int? DepartmentId { get; set; }

        // কোন মাদরাসার সাথে যুক্ত (যদি মাল্টিপল campus থাকে)
        public int? MadrasahId { get; set; }

        // শিক্ষকের চাকরির পদবি বা স্টাফের designation
        public string? Designation { get; set; }

        // শিক্ষক/স্টাফ/শিক্ষার্থীর unique registration বা ID
        public string? EmployeeCode { get; set; }


        // ---------- SYSTEM FLAGS ----------

        // UserType = Teacher / Student / Parent / Admin
        public string? UserType { get; set; }

        // Boolean flags
        public bool IsTeacher { get; set; }
        public bool IsStudent { get; set; }
        public bool IsParent { get; set; }
        public bool IsAdmin { get; set; }

        // একাউন্ট Active / Deactive
        public bool IsActive { get; set; } = true;


        // ---------- LOGIN TRACKING ----------

        // User creation date
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Last updated
        public DateTime? UpdatedDate { get; set; }

        // শেষ কখন লগইন করেছে
        public DateTime? LastLogin { get; set; }

        // Email/Phone verification flags
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }


        // ---------- SETTINGS ----------

        // Dark Mode / Light Mode
        public string? Theme { get; set; }

        // Language preference — bn/en/ar
        public string? Language { get; set; }

        // Notifications On/Off
        public bool ReceiveNotifications { get; set; } = true;


        // ---------- RELATIONSHIP FOR MESSAGING ----------

        // sender → messages
        public List<Message>? SendMessages { get; set; }

        // receiver → messages
        public List<Message>? ReceiveMessages { get; set; }


        // ---------- AUDIT TRAIL ----------

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }


        // Additional OPTIONAL useful fields
        public string? NationalId { get; set; }
        public string? Address { get; set; }
        public string? PermanentAddress { get; set; }
        public string? BloodGroup { get; set; }
        public DateTime? JoinDate { get; set; }
        public string? EmergencyContact { get; set; }
    }




    // ================================
    //  2.    CUSTOM APPLICATION ROLE
    // ================================

    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }

        public AppRole(string roleName) : base(roleName) { }

        // রোলের বর্ণনা — যেমন “Manages all student-related tasks”
        public string? Description { get; set; }

        // রোল creation / update tracking
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }

    // -------------------------
    // 3. Student
    // -------------------------

    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        // -------------------------
        // Account / Linked Identity User
        // -------------------------
        [Required]
        [ForeignKey(nameof(AppUser))]
        public string UserId { get; set; } = default!;
        [ValidateNever]
        public AppUser AppUser { get; set; } = default!;

        // -------------------------
        // Multilingual Names
        // -------------------------
        [Required, MaxLength(150)]
        public string StudentName { get; set; } = default!;

        [MaxLength(150)]
        public string? ArabicStudentName { get; set; }

        [MaxLength(150)]
        public string? BanglaStudentName { get; set; }


        // -------------------------
        // Academic Info
        // -------------------------
        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        [ValidateNever]
        public Department Department { get; set; } = default!;

        [ForeignKey(nameof(Class))]
        public int ClassId { get; set; }
        [ValidateNever]
        public Class Class { get; set; } = default!;

        [ForeignKey(nameof(Section))]
        public int SectionId { get; set; }
        [ValidateNever]
        public Section Section { get; set; } = default!;


        // -------------------------
        // Identity & Admission
        // -------------------------
        [Required, MaxLength(20)]
        public string RegNo { get; set; } = default!;
        [MaxLength(50)]
        public string? NationalId { get; set; }

        public DateOnly AdmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);


        // -------------------------
        // Personal Info
        // -------------------------
        public Gender? Gender { get; set; }

        public DateTime DOB { get; set; } = DateTime.Today.AddYears(-5);


        [MaxLength(5)]
        public string? BloodGroup { get; set; }


        // -------------------------
        // Parents / Guardians
        // -------------------------
        [MaxLength(150)]
        public string? FatherName { get; set; }

        [MaxLength(20)]
        public string? FatherPhone { get; set; }

        [MaxLength(150)]
        public string? MotherName { get; set; }

        [MaxLength(20)]
        public string? MotherPhone { get; set; }

        [MaxLength(150)]
        public string? GuardianName { get; set; }

        [MaxLength(20)]
        public string? GuardianPhone { get; set; }

        [MaxLength(150)]
        public string? GuardianEmail { get; set; }


        // -------------------------
        // Address / Location
        // -------------------------
        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(150)]
        public string? City { get; set; }

        [MaxLength(150)]
        public string? Country { get; set; }


        // -------------------------
        // Emergency Info
        // -------------------------
        [MaxLength(150)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(20)]
        public string? EmergencyPhone { get; set; }

        [MaxLength(500)]
        public string? MedicalNotes { get; set; }


        // -------------------------
        // Previous Academic
        // -------------------------
        [MaxLength(250)]
        public string? PreviousSchoolName { get; set; }

        public double? PreviousResult { get; set; }


        // -------------------------
        // Media / Documents
        // -------------------------
        [MaxLength(300)]
        public string? ProfileImageUrl { get; set; }

        [MaxLength(300)]
        public string? DocumentUrl { get; set; }


        // -------------------------
        // Status
        // -------------------------
        public bool IsActive { get; set; } = true;

        public DateTimeOffset? LeavingDate { get; set; }

        [MaxLength(300)]
        public string? LeavingReason { get; set; }


        // -------------------------
        // Optional JSON (NotMapped)
        // -------------------------
        [NotMapped]
        public Dictionary<string, string>? TranslatedNames { get; set; }


        // -------------------------
        // Audit
        // -------------------------
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? UpdatedAt { get; set; }


        // -------------------------
        // Navigation Collections
        // -------------------------
        public ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();
        public ICollection<ExamResult> ExamResults { get; set; } = new HashSet<ExamResult>();
        public ICollection<FeeCollection> FeeCollections { get; set; } = new HashSet<FeeCollection>();
        public ICollection<HostelResident> HostelResidents { get; set; } = new HashSet<HostelResident>();
        public ICollection<TransportAssignment> TransportAssignments { get; set; } = new HashSet<TransportAssignment>();
        public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
    }


    // -------------------------
    // 4. Teacher
    // -------------------------
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
        [Required]
        [ForeignKey(nameof(AppUser))]
        public string UserId { get; set; } = default!;

        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }

        public DateTimeOffset JoiningDate { get; set; }

        [MaxLength(250)]
        public string? Qualification { get; set; }

        [MaxLength(150)]
        public string? Designation { get; set; }

        // Navigation
        public AppUser AppUser { get; set; } = default!;
        public Department Department { get; set; } = default!;

        public ICollection<ClassSubject> ClassSubjects { get; set; } = new HashSet<ClassSubject>();
        public ICollection<Attendance> MarkedAttendances { get; set; } = new HashSet<Attendance>();
        public ICollection<TeacherAttendance> TeacherAttendances { get; set; } = new HashSet<TeacherAttendance>();
        public ICollection<Timetable> Timetables { get; set; } = new HashSet<Timetable>();
        public ICollection<Assignment> Assignments { get; set; } = new HashSet<Assignment>();
        public ICollection<Salary> Salaries { get; set; } = new HashSet<Salary>();
    }

    // -------------------------
    // 5. Class
    // -------------------------
    public class Class
    {
        [Key]
        public int ClassId { get; set; }

        [Required, MaxLength(100)]
        public string ClassName { get; set; } = default!;

        [MaxLength(9)]
        public string? SessionYear { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; } = default;

        public ICollection<Section> Sections { get; set; } = new HashSet<Section>();
        public ICollection<Student> Students { get; set; } = new HashSet<Student>();
        public ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new HashSet<ClassSubject>();
        public ICollection<Exam> Exams { get; set; } = new HashSet<Exam>();
        public ICollection<Timetable> Timetables { get; set; } = new HashSet<Timetable>();
        public ICollection<Assignment> Assignments { get; set; } = new HashSet<Assignment>();
        public ICollection<FeeType> FeeTypes { get; set; } = new HashSet<FeeType>();
        public ICollection<ExamFee>? ExamFees { get; set; }
        public virtual ICollection<PointCondition> PointConditions { get; set; } = new List<PointCondition>();

    }

    // -------------------------
    // 6. Section
    // -------------------------
    public class Section
    {
        [Key]
        public int SectionId { get; set; }

        [ForeignKey(nameof(Class))]
        public int ClassId { get; set; }

        [Required, MaxLength(50)]
        public string SectionName { get; set; } = default!;

        public Class? Class { get; set; } = default!;
        public ICollection<Student> Students { get; set; } = new HashSet<Student>();
        public ICollection<Timetable> Timetables { get; set; } = new HashSet<Timetable>();
    }

    // -------------------------
    // 7. Subject
    // -------------------------
    [Index(nameof(SubjectCode), IsUnique = true)]
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(Class))]
        public int ClassId { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }

        [Required, MaxLength(150)]
        public string SubjectName { get; set; } = default!;

        [Required, MaxLength(50)]
        public string SubjectCode { get; set; } = default!;

        public bool IsOptional { get; set; } = false;

        public Class Class { get; set; } = default!;
        public Department Department { get; set; } = default!;

        public ICollection<ClassSubject> ClassSubjects { get; set; } = new HashSet<ClassSubject>();
        public ICollection<Timetable> Timetables { get; set; } = new HashSet<Timetable>();
        //public virtual ICollection<PointCondition> PointConditions { get; set; } = new List<PointCondition>();
    }

    // -------------------------
    // 8. ClassSubject (junction)
    //    -- composite key via Fluent API recommended
    // -------------------------
    public class ClassSubject
    {
        // don't decorate with [Key] here — configure composite key in OnModelCreating
        [ForeignKey(nameof(Class))]
        public int ClassId { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }

        public Class Class { get; set; } = default!;
        public Subject Subject { get; set; } = default!;
        public Teacher Teacher { get; set; } = default!;
    }

    // -------------------------
    // 9. Exam
    // -------------------------
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;

        [ForeignKey(nameof(Class))]
        public int ClassId { get; set; }

        [MaxLength(50)]
        public string? Term { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        public Class Class { get; set; } = default!;
        public ICollection<ExamResult> ExamResults { get; set; } = new HashSet<ExamResult>();
    }

    // -------------------------
    // 10. ExamResult
    // -------------------------
    public class ExamResult
    {
        [Key]
        public int ResultId { get; set; }

        [ForeignKey(nameof(Exam))]
        public int ExamId { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        public double MarksObtained { get; set; }

        [MaxLength(5)]
        public string? Grade { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public Exam Exam { get; set; } = default!;
        public Student Student { get; set; } = default!;
        public Subject Subject { get; set; } = default!;
    }

    // -------------------------
    // 11. Attendance (student)
    // -------------------------
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        public DateTimeOffset Date { get; set; }

        public AttendanceStatus Status { get; set; }

        [ForeignKey(nameof(Teacher))]
        public int? TeacherId { get; set; } // teacher who marked, can be null for auto import

        public Student Student { get; set; } = default!;
        public Teacher? Teacher { get; set; }
    }

    // -------------------------
    // 12. TeacherAttendance
    // -------------------------
    public class TeacherAttendance
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }

        public DateTimeOffset Date { get; set; }
        public AttendanceStatus Status { get; set; }

        public Teacher Teacher { get; set; } = default!;
    }

    // -------------------------
    // 13. FeeType
    // -------------------------
    public class FeeType
    {
        [Key]
        public int FeeTypeId { get; set; }

        [ForeignKey(nameof(Class))]
        public int ClassId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;

        public decimal Amount { get; set; }

        public FeeFrequency Frequency { get; set; }

        public Class Class { get; set; } = default!;
    }

    // -------------------------
    // 14. FeeCollection
    // -------------------------
    public class FeeCollection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey(nameof(FeeType))]
        public int FeeTypeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        [Required]
        public DateTime DatePaid { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }   // Cash / bKash / Bank / etc.

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Paid;

        // Navigation Properties
        public Student Student { get; set; } = default!;
        public FeeType FeeType { get; set; } = default!;
    }

    // -------------------------
    // 15. Salary
    // -------------------------
    public class Salary
    {
        [Key]
        public int SalaryId { get; set; }

        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }

        [Required, MaxLength(20)]
        public string Month { get; set; } = default!; // e.g., "2025-07"

        public decimal Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public Teacher Teacher { get; set; } = default!;
    }

    // -------------------------
    // 16. Expense
    // -------------------------
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required, MaxLength(100)]
        public string Type { get; set; } = default!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateTimeOffset Date { get; set; }
    }

    // -------------------------
    // 17. Book
    // -------------------------
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required, MaxLength(250)]
        public string Title { get; set; } = default!;

        [MaxLength(150)]
        public string? Author { get; set; }

        [MaxLength(20)]
        public string? ISBN { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1")]
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? ImageUrl { get; set; }

        [NotMapped, DisplayName("Image")]
        public IFormFile? ImageFile { get; set; }

        public ICollection<IssuedBook> IssuedBooks { get; set; } = new HashSet<IssuedBook>();
        public bool CanBeIssued()
        {
            return AvailableCopies > 0;
        }

        // Method to issue a book
        public void IssueBook()
        {
            if (AvailableCopies <= 0)
                throw new InvalidOperationException("No copies available for issuing");

            AvailableCopies--;
        }

        // Method to return a book
        public void ReturnBook()
        {
            if (AvailableCopies >= TotalCopies)
                throw new InvalidOperationException("Cannot return more copies than total");

            AvailableCopies++;
        }

    }

    // -------------------------
    // 18. IssuedBook
    // -------------------------
    public class IssuedBook
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Book))]
        [Required(ErrorMessage = "Please select a book")]
        public int BookId { get; set; }

        [ForeignKey(nameof(AppUser))]
        [Required(ErrorMessage = "Please select a user")]
        public string IssuedTo { get; set; }


        public string UserFullName { get; set; } = default!;
        public string UserType { get; set; } = default!;
        public string? Class { get; set; }
        public string? Section { get; set; }
        public int? RollNumber { get; set; }

        public DateOnly IssueDate { get; set; }
        public DateOnly? ReturnDate { get; set; }

        public decimal Fine { get; set; }

        public Book Book { get; set; } = default!;
        public AppUser AppUser { get; set; } = default!;
    }


    // -------------------------
    // 19. Notice
    // -------------------------
    public class Notice
    {
        [Key]
        public int NoticeId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = default!;

        [Required, MaxLength(4000)]
        public string Content { get; set; } = default!;

        [ForeignKey(nameof(AppRole))]
        public string? VisibleToRoleId { get; set; }

        public DateTimeOffset DatePosted { get; set; } = DateTimeOffset.UtcNow;

        public AppRole? AppRole { get; set; } // navigation
    }

    // -------------------------
    // 20. Hostel
    // -------------------------
    public class Hostel
    {
        [Key]
        public int HostelId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;

        public int Capacity { get; set; }

        [MaxLength(500)]
        public string? WardenInfo { get; set; }

        public ICollection<HostelResident> Residents { get; set; } = new HashSet<HostelResident>();
    }

    // -------------------------
    // 21. HostelResident
    // -------------------------
    public class HostelResident
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        [ForeignKey(nameof(Hostel))]
        public int HostelId { get; set; }

        [MaxLength(20)]
        public string? RoomNo { get; set; }

        public DateTimeOffset CheckInDate { get; set; }

        public Student Student { get; set; } = default!;
        public Hostel Hostel { get; set; } = default!;
    }

    // -------------------------
    // 22. TransportRoute
    // -------------------------
    public class TransportRoute
    {
        [Key]
        public int RouteId { get; set; }

        [Required, MaxLength(150)]
        public string RouteName { get; set; } = default!;

        [MaxLength(50)]
        public string? VehicleNo { get; set; }

        [MaxLength(150)]
        public string? DriverName { get; set; }

        [MaxLength(2000)]
        public string? Stops { get; set; } // consider normalized table for stops

        public ICollection<TransportAssignment> Assignments { get; set; } = new HashSet<TransportAssignment>();
    }

    // -------------------------
    // 23. TransportAssignment
    // -------------------------
    public class TransportAssignment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        [ForeignKey(nameof(TransportRoute))]
        public int RouteId { get; set; }

        [MaxLength(200)]
        public string? PickupPoint { get; set; }

        public Student Student { get; set; } = default!;
        public TransportRoute TransportRoute { get; set; } = default!;
    }

    // -------------------------
    // 24. Timetable
    // -------------------------
    public class Timetable
    {
        public int Id { get; set; }

        public int ClassId { get; set; }
        public Class Class { get; set; } = default!;

        public int SectionId { get; set; }
        public Section Section { get; set; } = default!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = default!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = default!;

        public string Day { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string? Room { get; set; }
    }


    // -------------------------
    // 25. Message
    // -------------------------
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [ForeignKey(nameof(Sender))]
        public string SenderId { get; set; } = default!;

        [ForeignKey(nameof(Receiver))]

        public string ReceiverId { get; set; } = default!;

        [Required, MaxLength(2000)]
        public string Content { get; set; } = default!;

        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        [MaxLength(50)]
        public string? Status { get; set; }

        [InverseProperty(nameof(AppUser.SendMessages))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public AppUser Sender { get; set; } = default!;

        [InverseProperty(nameof(AppUser.ReceiveMessages))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public AppUser Receiver { get; set; } = default!;
    }

    // -------------------------
    // 26. LoginLog
    // -------------------------
    public class LoginLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(AppUser))]
        public string UserId { get; set; } = default!;

        public DateTimeOffset LoginTime { get; set; } = DateTimeOffset.UtcNow;

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        public AppUser AppUser { get; set; } = default!;
    }

    // -------------------------
    // 27. ActivityLog
    // -------------------------
    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(AppUser))]
        public string UserId { get; set; } = default!;

        [MaxLength(100)]
        public string ActionType { get; set; } = default!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        public AppUser AppUser { get; set; } = default!;
    }

    // -------------------------
    // 28. Event (upgraded)
    // -------------------------
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = default!;

        [MaxLength(4000)]
        public string? Description { get; set; } // sanitize if HTML

        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public bool IsAllDay { get; set; } = false;

        public string? Location { get; set; }

        public Audience Audience { get; set; } = Audience.Everyone;

        // Audit & concurrency
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }

    // -------------------------
    // 29. Assignment
    // -------------------------
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [ForeignKey(nameof(Class))]
        public int ClassId { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = default!;

        [MaxLength(4000)]
        public string? Description { get; set; }

        public DateTimeOffset DueDate { get; set; }

        public Class Class { get; set; } = default!;
        public Subject Subject { get; set; } = default!;
        public Teacher Teacher { get; set; } = default!;
        public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
    }

    // -------------------------
    // 30. Submission
    // -------------------------
    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }

        [ForeignKey(nameof(Assignment))]
        public int AssignmentId { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        [Required, MaxLength(1000)]
        public string FileLink { get; set; } = default!;

        public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;

        public double? Marks { get; set; }

        [MaxLength(2000)]
        public string? Feedback { get; set; }

        public Assignment Assignment { get; set; } = default!;
        public Student Student { get; set; } = default!;
    }

    // -------------------------
    // 31. Department
    // -------------------------
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required, MaxLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<Class> Classes { get; set; } = new HashSet<Class>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public List<Student>? Students { get; set; }

    }
}