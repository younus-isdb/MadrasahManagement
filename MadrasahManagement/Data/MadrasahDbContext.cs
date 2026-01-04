using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace MadrasahManagement.Models
{
    public class MadrasahDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public MadrasahDbContext(DbContextOptions<MadrasahDbContext> options) : base(options)
        {
        }

        // =============================
        // 📘 DbSet: 30+ Tables
        // =============================
        public DbSet<Student> Students { get; set; } = default!;
        public DbSet<Teacher> Teachers { get; set; } = default!;
        public DbSet<Class> Classes { get; set; } = default!;
        public DbSet<Section> Sections { get; set; } = default!;
        public DbSet<Subject> Subjects { get; set; } = default!;
        public DbSet<ClassSubject> ClassSubjects { get; set; } = default!;
        public DbSet<Exam> Exams { get; set; } = default!;
        public DbSet<ExamResult> ExamResults { get; set; } = default!;
        public DbSet<ResultDetail> ResultDetails { get; set; } = default!;

        public DbSet<Attendance> Attendances { get; set; } = default!;
        // ===== Examination =====
        public DbSet<Examination> Examinations { get; set; } = default!;
        public DbSet<ExamFee> ExamFees { get; set; } = default!;
        public DbSet<ExamRoutine> ExamRoutines { get; set; } = default!;
        public DbSet<SeatPlan> SeatPlans { get; set; } = default!;

        // ===== Result / Academic =====
        public DbSet<PointCondition> PointConditions { get; set; } = default!;
        public DbSet<PointConditionDetail> PointConditionDetails { get; set; } = default!;
        public DbSet<MeritCondition> MeritConditions { get; set; } = default!;
        public DbSet<SubClassGroup> SubClassGroups { get; set; } = default!;

        // ===== Finance =====
        public DbSet<ExamFeeCollection> ExamFeeCollections { get; set; } = default!;
        public DbSet<ExamIncomeExpense> ExamIncomeExpenses { get; set; } = default!;
        public DbSet<TeacherAttendance> TeacherAttendances { get; set; } = default!;
        public DbSet<FeeType> FeeTypes { get; set; } = default!;
        public DbSet<FeeCollection> FeeCollections { get; set; } = default!;
        public DbSet<Salary> Salaries { get; set; } = default!;
        public DbSet<Staff> Staffs { get; set; } = default!;
        public DbSet<Expense> Expenses { get; set; } = default!;
        public DbSet<Book> Books { get; set; } = default!;
        public DbSet<IssuedBook> IssuedBooks { get; set; } = default!;
        public DbSet<Notice> Notices { get; set; } = default!;
        public DbSet<Hostel> Hostels { get; set; } = default!;
        public DbSet<HostelResident> HostelResidents { get; set; } = default!;
        public DbSet<TransportRoute> TransportRoutes { get; set; } = default!;
        public DbSet<TransportAssignment> TransportAssignments { get; set; } = default!;
        public DbSet<Timetable> Timetables { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;
        public DbSet<LoginLog> LoginLogs { get; set; } = default!;
        public DbSet<ActivityLog> ActivityLogs { get; set; } = default!;
        public DbSet<Event> Events { get; set; } = default!;
        public DbSet<Assignment> Assignments { get; set; } = default!;
        public DbSet<Submission> Submissions { get; set; } = default!;
        public DbSet<Department> Departments { get; set; } = default!;


        // =============================
        // ⚙️ Fluent API Configuration
        // =============================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherConfiguration());
            modelBuilder.ApplyConfiguration(new ClassConfiguration());
            modelBuilder.ApplyConfiguration(new SectionConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectConfiguration());
            modelBuilder.ApplyConfiguration(new ClassSubjectConfiguration());
            modelBuilder.ApplyConfiguration(new ExamFeeConfiguration());
            modelBuilder.ApplyConfiguration(new ExamResultConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherAttendanceConfiguration());
            modelBuilder.ApplyConfiguration(new FeeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FeeCollectionConfiguration());
            modelBuilder.ApplyConfiguration(new SalaryConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new IssuedBookConfiguration());
            modelBuilder.ApplyConfiguration(new NoticeConfiguration());
            modelBuilder.ApplyConfiguration(new HostelConfiguration());
            modelBuilder.ApplyConfiguration(new HostelResidentConfiguration());
            modelBuilder.ApplyConfiguration(new TransportRouteConfiguration());
            modelBuilder.ApplyConfiguration(new TransportAssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new TimetableConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());

            base.OnModelCreating(modelBuilder);

            // ExamResult -> Department
            modelBuilder.Entity<ExamResult>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade

            // ExamResult -> Class
            modelBuilder.Entity<ExamResult>()
                .HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade

            // ExamResult -> Examination
            modelBuilder.Entity<ExamResult>()
                .HasOne(e => e.Examination)
                .WithMany()
                .HasForeignKey(e => e.ExamId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade

            // ExamResult -> Student
            modelBuilder.Entity<ExamResult>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade

            // ExamResult -> ResultDetails
            modelBuilder.Entity<ResultDetail>()
                .HasOne(rd => rd.Result)
                .WithMany(r => r.ResultDetails)
                .HasForeignKey(rd => rd.ResultId)
                .OnDelete(DeleteBehavior.Cascade); // Safe cascade
            modelBuilder.Entity<Submission>()
        .HasOne(s => s.Assignment)
        .WithMany()
        .HasForeignKey(s => s.AssignmentId)
        .OnDelete(DeleteBehavior.Cascade);

            // Submissions -> Student (no cascade)
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // pr
            modelBuilder.Entity<SeatPlan>()
        .HasOne(s => s.Student)
        .WithMany()
        .HasForeignKey(s => s.StudentId)
        .OnDelete(DeleteBehavior.Restrict); // or NoAction

            modelBuilder.Entity<SeatPlan>()
                .HasOne(s => s.Class)
                .WithMany()
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SeatPlan>()
                .HasOne(s => s.Department)
                .WithMany()
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SeatPlan>()
                .HasOne(s => s.Subject)
                .WithMany()
                .HasForeignKey(s => s.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // ==========================
        // Configurations Classes
        // ==========================

        public class StudentConfiguration : IEntityTypeConfiguration<Student>
        {
            public void Configure(EntityTypeBuilder<Student> builder)
            {
                builder.HasKey(s => s.StudentId);
                builder.HasIndex(s => s.RegNo).IsUnique();
                builder.HasOne(s => s.AppUser).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(s => s.Class).WithMany().HasForeignKey(s => s.ClassId).OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(s => s.Section).WithMany().HasForeignKey(s => s.SectionId).OnDelete(DeleteBehavior.Restrict);
                builder.Property(s => s.StudentName).IsRequired().HasMaxLength(150);
                builder.Property(s => s.RegNo).IsRequired().HasMaxLength(20).IsUnicode(false);
                builder.Property(s => s.AdmissionDate).HasDefaultValueSql("GETDATE()");
                builder.Property(s => s.TranslatedNames)
                       .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v))
                       .HasMaxLength(1000);
                builder.Property(s => s.CreatedAt).HasDefaultValueSql("GETDATE()");
            }
        }

        public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
        {
            public void Configure(EntityTypeBuilder<Teacher> builder)
            {
                builder.ToTable("Teachers");
                builder.HasIndex(x => x.Email).IsUnique();
                builder.HasKey(t => t.TeacherId);
                builder.HasOne(t => t.AppUser).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(t => t.Department).WithMany(d => d.Teachers).HasForeignKey(t => t.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ClassConfiguration : IEntityTypeConfiguration<Class>
        {
            public void Configure(EntityTypeBuilder<Class> builder)
            {
                builder.ToTable("Classes");
                builder.HasKey(c => c.ClassId);
                builder.HasOne(c => c.Department).WithMany(d => d.Classes).HasForeignKey(c => c.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class SectionConfiguration : IEntityTypeConfiguration<Section>
        {
            public void Configure(EntityTypeBuilder<Section> builder)
            {
                builder.ToTable("Sections");
                builder.HasKey(s => s.SectionId);
                builder.HasOne(s => s.Class).WithMany(c => c.Sections).HasForeignKey(s => s.ClassId).OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
        {
            public void Configure(EntityTypeBuilder<Subject> builder)
            {
                builder.ToTable("Subjects");
                builder.HasKey(s => s.SubjectId);
                builder.HasIndex(s => s.SubjectCode).IsUnique();
                builder.HasOne(s => s.Class).WithMany(c => c.Subjects).HasForeignKey(s => s.ClassId).OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(s => s.Department).WithMany().HasForeignKey(s => s.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ClassSubjectConfiguration : IEntityTypeConfiguration<ClassSubject>
        {
            public void Configure(EntityTypeBuilder<ClassSubject> builder)
            {
                builder.ToTable("ClassSubjects");
                builder.HasKey(cs => new { cs.ClassId, cs.SubjectId });
                builder.HasOne(cs => cs.Class).WithMany(c => c.ClassSubjects).HasForeignKey(cs => cs.ClassId).OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(cs => cs.Subject).WithMany(s => s.ClassSubjects).HasForeignKey(cs => cs.SubjectId).OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(cs => cs.Teacher).WithMany(t => t.ClassSubjects).HasForeignKey(cs => cs.TeacherId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ExamFeeConfiguration : IEntityTypeConfiguration<ExamFee>
        {
            public void Configure(EntityTypeBuilder<ExamFee> builder)
            {
                builder.ToTable("ExamFees");
                builder.Property(e => e.ExamAmount).HasPrecision(18, 2);
                builder.HasOne(e => e.Department).WithMany().HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(e => e.Class).WithMany().HasForeignKey(e => e.ClassId).OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(e => e.Examination).WithMany().HasForeignKey(e => e.ExamId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ExamResultConfiguration : IEntityTypeConfiguration<ExamResult>
        {
            public void Configure(EntityTypeBuilder<ExamResult> builder)
            {
                builder.ToTable("ExamResults");
                builder.HasKey(er => er.ResultId);
                builder.HasOne(er => er.Examination).WithMany().HasForeignKey(er => er.ExamId).OnDelete(DeleteBehavior.NoAction);
                builder.HasOne(er => er.Student).WithMany().HasForeignKey(er => er.StudentId).OnDelete(DeleteBehavior.NoAction);
            }
        }

        public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
        {
            public void Configure(EntityTypeBuilder<Attendance> builder)
            {
                builder.ToTable("Attendances");
                builder.HasOne(a => a.Student).WithMany(s => s.Attendances).HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(a => a.Teacher).WithMany(t => t.MarkedAttendances).HasForeignKey(a => a.TeacherId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class TeacherAttendanceConfiguration : IEntityTypeConfiguration<TeacherAttendance>
        {
            public void Configure(EntityTypeBuilder<TeacherAttendance> builder)
            {
                builder.ToTable("TeacherAttendances");
                builder.HasOne(ta => ta.Teacher).WithMany(t => t.TeacherAttendances).HasForeignKey(ta => ta.TeacherId).OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class FeeTypeConfiguration : IEntityTypeConfiguration<FeeType>
        {
            public void Configure(EntityTypeBuilder<FeeType> builder)
            {
                builder.ToTable("FeeTypes");
                builder.HasOne(ft => ft.Class).WithMany(c => c.FeeTypes).HasForeignKey(ft => ft.ClassId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class FeeCollectionConfiguration : IEntityTypeConfiguration<FeeCollection>
        {
            public void Configure(EntityTypeBuilder<FeeCollection> builder)
            {
                builder.ToTable("FeeCollections");
                builder.HasOne(fc => fc.Student).WithMany(s => s.FeeCollections).HasForeignKey(fc => fc.StudentId).OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(fc => fc.FeeType).WithMany().HasForeignKey(fc => fc.FeeTypeId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class SalaryConfiguration : IEntityTypeConfiguration<Salary>
        {
            public void Configure(EntityTypeBuilder<Salary> builder)
            {
                builder.ToTable("Salaries");
                builder.HasOne(s => s.Teacher).WithMany(t => t.Salaries).HasForeignKey(s => s.TeacherId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
        {
            public void Configure(EntityTypeBuilder<Expense> builder) { builder.ToTable("Expenses"); }
        }

        public class BookConfiguration : IEntityTypeConfiguration<Book>
        {
            public void Configure(EntityTypeBuilder<Book> builder)
            {
                builder.ToTable("Books");
                builder.HasIndex(b => new { b.Title, b.Category }).IsUnique().HasFilter("[Category] is not null");
            }
        }

        public class IssuedBookConfiguration : IEntityTypeConfiguration<IssuedBook>
        {
            public void Configure(EntityTypeBuilder<IssuedBook> builder)
            {
                builder.ToTable("IssuedBooks");
                builder.HasOne(ib => ib.Book).WithMany(b => b.IssuedBooks).HasForeignKey(ib => ib.BookId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class NoticeConfiguration : IEntityTypeConfiguration<Notice>
        {
            public void Configure(EntityTypeBuilder<Notice> builder)
            {
                builder.ToTable("Notices");
                builder.HasOne(n => n.AppRole).WithMany().HasForeignKey(n => n.VisibleToRoleId).OnDelete(DeleteBehavior.SetNull);
            }
        }

        public class HostelConfiguration : IEntityTypeConfiguration<Hostel>
        {
            public void Configure(EntityTypeBuilder<Hostel> builder) { builder.ToTable("Hostels"); }
        }

        public class HostelResidentConfiguration : IEntityTypeConfiguration<HostelResident>
        {
            public void Configure(EntityTypeBuilder<HostelResident> builder)
            {
                builder.ToTable("HostelResidents");
                builder.HasOne(hr => hr.Student).WithMany(s => s.HostelResidents).HasForeignKey(hr => hr.StudentId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class TransportRouteConfiguration : IEntityTypeConfiguration<TransportRoute>
        {
            public void Configure(EntityTypeBuilder<TransportRoute> builder) { builder.ToTable("TransportRoutes"); }
        }

        public class TransportAssignmentConfiguration : IEntityTypeConfiguration<TransportAssignment>
        {
            public void Configure(EntityTypeBuilder<TransportAssignment> builder)
            {
                builder.ToTable("TransportAssignments");
                builder.HasOne(ta => ta.Student).WithMany(s => s.TransportAssignments).HasForeignKey(ta => ta.StudentId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class TimetableConfiguration : IEntityTypeConfiguration<Timetable>
        {
            public void Configure(EntityTypeBuilder<Timetable> builder)
            {
                builder.HasOne(t => t.Class).WithMany(c => c.Timetables).HasForeignKey(t => t.ClassId).OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(t => t.Section).WithMany(s => s.Timetables).HasForeignKey(t => t.SectionId).OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(t => t.Teacher).WithMany(te => te.Timetables).HasForeignKey(t => t.TeacherId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class MessageConfiguration : IEntityTypeConfiguration<Message>
        {
            public void Configure(EntityTypeBuilder<Message> builder) { builder.ToTable("Messages"); }
        }

        public class EventConfiguration : IEntityTypeConfiguration<Event>
        {
            public void Configure(EntityTypeBuilder<Event> builder) { builder.ToTable("Events"); }
        }

        public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
        {
            public void Configure(EntityTypeBuilder<Assignment> builder)
            {
                builder.ToTable("Assignments");
                builder.HasOne(a => a.Class).WithMany(c => c.Assignments).HasForeignKey(a => a.ClassId).OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
        {
            public void Configure(EntityTypeBuilder<Submission> builder)
            {
                builder.ToTable("Submissions");
                builder.HasOne(s => s.Assignment).WithMany(a => a.Submissions).HasForeignKey(s => s.AssignmentId).OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
        {
            public void Configure(EntityTypeBuilder<Department> builder)
            {
                builder.ToTable("Departments");
                builder.HasMany(d => d.Classes).WithOne(c => c.Department).HasForeignKey(c => c.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}