// File: MadrasahDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace MadrasahManagement.Models
{
    public class MadrasahDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public MadrasahDbContext(DbContextOptions<MadrasahDbContext> options) : base(options) { }

        // -----------------------
        // DbSets
        // -----------------------
        public DbSet<Student> Students { get; set; } = default!;
        public DbSet<Teacher> Teachers { get; set; } = default!;
        public DbSet<Class> Classes { get; set; } = default!;
        public DbSet<Section> Sections { get; set; } = default!;
        public DbSet<Subject> Subjects { get; set; } = default!;
        public DbSet<ClassSubject> ClassSubjects { get; set; } = default!;
        public DbSet<Exam> Exams { get; set; } = default!;
        public DbSet<ExamResult> ExamResults { get; set; } = default!;
        public DbSet<Attendance> Attendances { get; set; } = default!;
        public DbSet<TeacherAttendance> TeacherAttendances { get; set; } = default!;
        public DbSet<FeeType> FeeTypes { get; set; } = default!;
        public DbSet<FeeCollection> FeeCollections { get; set; } = default!;
        public DbSet<Salary> Salaries { get; set; } = default!;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Identity tables configuration first

            // Apply configurations (keeps OnModelCreating organized)
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherConfiguration());
            modelBuilder.ApplyConfiguration(new ClassConfiguration());
            modelBuilder.ApplyConfiguration(new SectionConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectConfiguration());
            modelBuilder.ApplyConfiguration(new ClassSubjectConfiguration());
            modelBuilder.ApplyConfiguration(new ExamConfiguration());
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
            modelBuilder.ApplyConfiguration(new LoginLogConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityLogConfiguration());
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        }

        // -----------------------
        // Configurations (Inner classes for clarity)
        // You can move these into separate files for cleanliness.
        // -----------------------

        public class StudentConfiguration : IEntityTypeConfiguration<Student>
        {
            public void Configure(EntityTypeBuilder<Student> builder)
            {
                builder.ToTable("Students");
                builder.HasKey(s => s.StudentId);

                // Unique rollno constraint
                builder.HasIndex(s => s.RollNo).IsUnique();

                // Name
                builder.Property(s => s.StudentName).IsRequired().HasMaxLength(150);
                builder.Property(s => s.ArabicStudentName).HasMaxLength(150);
                builder.Property(s => s.BanglaStudentName).HasMaxLength(150);

                // RollNo ASCII constraint (optional)
                builder.Property(s => s.RollNo)
                       .IsRequired()
                       .HasMaxLength(20)
                       .IsUnicode(false);

                // Dates defaults
                builder.Property(s => s.AdmissionDate)
                       .HasDefaultValueSql("GETUTCDATE()");

                builder.Property(s => s.CreatedAt)
                       .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                builder.HasOne(s => s.AppUser)
                       .WithMany()
                       .HasForeignKey(s => s.UserId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(s => s.Class)
                       .WithMany(c => c.Students)
                       .HasForeignKey(s => s.ClassId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(s => s.Section)
                       .WithMany(sec => sec.Students)
                       .HasForeignKey(s => s.SectionId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(s => s.Department)
                       .WithMany(d => d.Students) // department -> classes but for student we keep Restrict
                       .HasForeignKey(s => s.DepartmentId)
                       .OnDelete(DeleteBehavior.Restrict);

                // TranslatedNames conversion -> JSON (System.Text.Json)
                builder.Property<string>("TranslatedNamesJson")
                       .HasMaxLength(2000)
                       .HasColumnName("TranslatedNamesJson"); // underlying column

                builder.Ignore(s => s.TranslatedNames); // we'll provide conversion via property accessors in model OR map via backing field

                // Optional index for name search (non-unique)
                builder.HasIndex(s => s.StudentName).HasDatabaseName("IX_StudentName");
            }
        }

        public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
        {
            public void Configure(EntityTypeBuilder<Teacher> builder)
            {
                builder.ToTable("Teachers");
                builder.HasKey(t => t.TeacherId);

                builder.Property(t => t.Qualification).HasMaxLength(250);
                builder.Property(t => t.Designation).HasMaxLength(150);

                builder.HasOne(t => t.AppUser)
                       .WithMany()
                       .HasForeignKey(t => t.UserId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(t => t.Department)
                       .WithMany(d => d.Teachers)
                       .HasForeignKey(t => t.DepartmentId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ClassConfiguration : IEntityTypeConfiguration<Class>
        {
            public void Configure(EntityTypeBuilder<Class> builder)
            {
                builder.ToTable("Classes");
                builder.HasKey(c => c.ClassId);
                builder.Property(c => c.ClassName).IsRequired().HasMaxLength(100);
                builder.Property(c => c.SessionYear).HasMaxLength(9);

                builder.HasOne(c => c.Department)
                       .WithMany(d => d.Classes)
                       .HasForeignKey(c => c.DepartmentId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class SectionConfiguration : IEntityTypeConfiguration<Section>
        {
            public void Configure(EntityTypeBuilder<Section> builder)
            {
                builder.ToTable("Sections");
                builder.HasKey(s => s.SectionId);
                builder.Property(s => s.SectionName).IsRequired().HasMaxLength(50);

                builder.HasOne(s => s.Class)
                       .WithMany(c => c.Sections)
                       .HasForeignKey(s => s.ClassId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
        {
            public void Configure(EntityTypeBuilder<Subject> builder)
            {
                builder.ToTable("Subjects");
                builder.HasKey(s => s.SubjectId);

                builder.Property(s => s.SubjectName).IsRequired().HasMaxLength(150);
                builder.Property(s => s.SubjectCode).IsRequired().HasMaxLength(50);
                builder.HasIndex(s => s.SubjectCode).IsUnique();

                builder.HasOne(s => s.Class)
                       .WithMany(c => c.Subjects)
                       .HasForeignKey(s => s.ClassId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(s => s.Department)
                       .WithMany()
                       .HasForeignKey(s => s.DepartmentId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ClassSubjectConfiguration : IEntityTypeConfiguration<ClassSubject>
        {
            public void Configure(EntityTypeBuilder<ClassSubject> builder)
            {
                builder.ToTable("ClassSubjects");

                // Composite PK: keep teacher included for uniqueness per teacher-subject-class assignment
                builder.HasKey(cs => new { cs.ClassId, cs.SubjectId, cs.TeacherId });

                builder.HasOne(cs => cs.Class)
                       .WithMany(c => c.ClassSubjects)
                       .HasForeignKey(cs => cs.ClassId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(cs => cs.Subject)
                       .WithMany(s => s.ClassSubjects)
                       .HasForeignKey(cs => cs.SubjectId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(cs => cs.Teacher)
                       .WithMany(t => t.ClassSubjects)
                       .HasForeignKey(cs => cs.TeacherId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ExamConfiguration : IEntityTypeConfiguration<Exam>
        {
            public void Configure(EntityTypeBuilder<Exam> builder)
            {
                builder.ToTable("Exams");
                builder.HasKey(e => e.ExamId);

                builder.Property(e => e.Name).IsRequired().HasMaxLength(150);

                builder.HasOne(e => e.Class)
                       .WithMany(c => c.Exams)
                       .HasForeignKey(e => e.ClassId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ExamResultConfiguration : IEntityTypeConfiguration<ExamResult>
        {
            public void Configure(EntityTypeBuilder<ExamResult> builder)
            {
                builder.ToTable("ExamResults");
                builder.HasKey(er => er.ResultId);

                builder.HasOne(er => er.Exam)
                       .WithMany(e => e.ExamResults)
                       .HasForeignKey(er => er.ExamId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(er => er.Student)
                       .WithMany(s => s.ExamResults)
                       .HasForeignKey(er => er.StudentId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(er => er.Subject)
                       .WithMany()
                       .HasForeignKey(er => er.SubjectId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
        {
            public void Configure(EntityTypeBuilder<Attendance> builder)
            {
                builder.ToTable("Attendances");
                builder.HasKey(a => a.AttendanceId);

                builder.Property(a => a.Status).IsRequired();

                builder.HasOne(a => a.Student)
                       .WithMany(s => s.Attendances)
                       .HasForeignKey(a => a.StudentId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(a => a.Teacher)
                       .WithMany(t => t.MarkedAttendances)
                       .HasForeignKey(a => a.TeacherId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasIndex(a => new { a.Date, a.StudentId }).HasDatabaseName("IX_Attendance_Date_Student");
            }
        }

        public class TeacherAttendanceConfiguration : IEntityTypeConfiguration<TeacherAttendance>
        {
            public void Configure(EntityTypeBuilder<TeacherAttendance> builder)
            {
                builder.ToTable("TeacherAttendances");
                builder.HasKey(ta => ta.Id);

                builder.HasOne(ta => ta.Teacher)
                       .WithMany(t => t.TeacherAttendances)
                       .HasForeignKey(ta => ta.TeacherId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class FeeTypeConfiguration : IEntityTypeConfiguration<FeeType>
        {
            public void Configure(EntityTypeBuilder<FeeType> builder)
            {
                builder.ToTable("FeeTypes");
                builder.HasKey(ft => ft.FeeTypeId);

                builder.Property(ft => ft.Name).IsRequired().HasMaxLength(150);

                builder.HasOne(ft => ft.Class)
                       .WithMany(c => c.FeeTypes)
                       .HasForeignKey(ft => ft.ClassId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class FeeCollectionConfiguration : IEntityTypeConfiguration<FeeCollection>
        {
            public void Configure(EntityTypeBuilder<FeeCollection> builder)
            {
                builder.ToTable("FeeCollections");
                builder.HasKey(fc => fc.Id);

                builder.HasOne(fc => fc.Student)
                       .WithMany(s => s.FeeCollections)
                       .HasForeignKey(fc => fc.StudentId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(fc => fc.FeeType)
                       .WithMany()
                       .HasForeignKey(fc => fc.FeeTypeId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class SalaryConfiguration : IEntityTypeConfiguration<Salary>
        {
            public void Configure(EntityTypeBuilder<Salary> builder)
            {
                builder.ToTable("Salaries");
                builder.HasKey(s => s.SalaryId);

                builder.HasOne(s => s.Teacher)
                       .WithMany(t => t.Salaries)
                       .HasForeignKey(s => s.TeacherId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
        {
            public void Configure(EntityTypeBuilder<Expense> builder)
            {
                builder.ToTable("Expenses");
                builder.HasKey(e => e.ExpenseId);
                builder.Property(e => e.Type).IsRequired().HasMaxLength(100);
            }
        }

        public class BookConfiguration : IEntityTypeConfiguration<Book>
        {
            public void Configure(EntityTypeBuilder<Book> builder)
            {
                builder.ToTable("Books");
                builder.HasKey(b => b.BookId);
                builder.Property(b => b.Title).IsRequired().HasMaxLength(250);
            }
        }

        public class IssuedBookConfiguration : IEntityTypeConfiguration<IssuedBook>
        {
            public void Configure(EntityTypeBuilder<IssuedBook> builder)
            {
                builder.ToTable("IssuedBooks");
                builder.HasKey(ib => ib.Id);

                builder.HasOne(ib => ib.Book)
                       .WithMany(b => b.IssuedBooks)
                       .HasForeignKey(ib => ib.BookId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(ib => ib.AppUser)
                       .WithMany()
                       .HasForeignKey(ib => ib.IssuedTo)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class NoticeConfiguration : IEntityTypeConfiguration<Notice>
        {
            public void Configure(EntityTypeBuilder<Notice> builder)
            {
                builder.ToTable("Notices");
                builder.HasKey(n => n.NoticeId);

                builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
                builder.Property(n => n.Content).IsRequired().HasMaxLength(4000);

                builder.HasOne(n => n.AppRole)
                       .WithMany()
                       .HasForeignKey(n => n.VisibleToRoleId)
                       .OnDelete(DeleteBehavior.SetNull);
            }
        }

        public class HostelConfiguration : IEntityTypeConfiguration<Hostel>
        {
            public void Configure(EntityTypeBuilder<Hostel> builder)
            {
                builder.ToTable("Hostels");
                builder.HasKey(h => h.HostelId);
                builder.Property(h => h.Name).IsRequired().HasMaxLength(150);
            }
        }

        public class HostelResidentConfiguration : IEntityTypeConfiguration<HostelResident>
        {
            public void Configure(EntityTypeBuilder<HostelResident> builder)
            {
                builder.ToTable("HostelResidents");
                builder.HasKey(hr => hr.Id);

                builder.HasOne(hr => hr.Student)
                       .WithMany(s => s.HostelResidents)
                       .HasForeignKey(hr => hr.StudentId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(hr => hr.Hostel)
                       .WithMany(h => h.Residents)
                       .HasForeignKey(hr => hr.HostelId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class TransportRouteConfiguration : IEntityTypeConfiguration<TransportRoute>
        {
            public void Configure(EntityTypeBuilder<TransportRoute> builder)
            {
                builder.ToTable("TransportRoutes");
                builder.HasKey(tr => tr.RouteId);
                builder.Property(tr => tr.RouteName).IsRequired().HasMaxLength(150);
            }
        }

        public class TransportAssignmentConfiguration : IEntityTypeConfiguration<TransportAssignment>
        {
            public void Configure(EntityTypeBuilder<TransportAssignment> builder)
            {
                builder.ToTable("TransportAssignments");
                builder.HasKey(ta => ta.Id);

                builder.HasOne(ta => ta.Student)
                       .WithMany(s => s.TransportAssignments)
                       .HasForeignKey(ta => ta.StudentId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(ta => ta.TransportRoute)
                       .WithMany(tr => tr.Assignments)
                       .HasForeignKey(ta => ta.RouteId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class TimetableConfiguration : IEntityTypeConfiguration<Timetable>
        {
            public void Configure(EntityTypeBuilder<Timetable> builder)
            {
                builder.ToTable("Timetables");
                builder.HasKey(t => t.Id);

                builder.Property(t => t.Day).IsRequired().HasMaxLength(20);
                builder.Property(t => t.Period).IsRequired().HasMaxLength(50);
                builder.Property(t => t.Room).HasMaxLength(50);

                builder.HasOne(t => t.Class)
                       .WithMany(c => c.Timetables)
                       .HasForeignKey(t => t.ClassId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(t => t.Section)
                       .WithMany(s => s.Timetables)
                       .HasForeignKey(t => t.SectionId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(t => t.Subject)
                       .WithMany(s => s.Timetables)
                       .HasForeignKey(t => t.SubjectId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(t => t.Teacher)
                       .WithMany(te => te.Timetables)
                       .HasForeignKey(t => t.TeacherId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class MessageConfiguration : IEntityTypeConfiguration<Message>
        {
            public void Configure(EntityTypeBuilder<Message> builder)
            {
                builder.ToTable("Messages");
                builder.HasKey(m => m.MessageId);

                builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);

                builder.HasOne(m => m.Sender)
                       .WithMany(u => u.SendMessages)
                       .HasForeignKey(m => m.SenderId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(m => m.Receiver)
                       .WithMany(u => u.ReceiveMessages)
                       .HasForeignKey(m => m.ReceiverId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasIndex(m => new { m.SenderId, m.ReceiverId, m.Timestamp }).HasDatabaseName("IX_Message_SenderReceiverTime");
            }
        }

        public class LoginLogConfiguration : IEntityTypeConfiguration<LoginLog>
        {
            public void Configure(EntityTypeBuilder<LoginLog> builder)
            {
                builder.ToTable("LoginLogs");
                builder.HasKey(ll => ll.Id);

                builder.HasOne(ll => ll.AppUser)
                       .WithMany()
                       .HasForeignKey(ll => ll.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
        {
            public void Configure(EntityTypeBuilder<ActivityLog> builder)
            {
                builder.ToTable("ActivityLogs");
                builder.HasKey(al => al.Id);

                builder.HasOne(al => al.AppUser)
                       .WithMany()
                       .HasForeignKey(al => al.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class EventConfiguration : IEntityTypeConfiguration<Event>
        {
            public void Configure(EntityTypeBuilder<Event> builder)
            {
                builder.ToTable("Events");
                builder.HasKey(e => e.EventId);

                builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
                builder.Property(e => e.Description).HasMaxLength(4000);
                builder.Property(e => e.Location).HasMaxLength(250);

                // concurrency token already via [Timestamp] on model
            }
        }

        public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
        {
            public void Configure(EntityTypeBuilder<Assignment> builder)
            {
                builder.ToTable("Assignments");
                builder.HasKey(a => a.AssignmentId);

                builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
                builder.Property(a => a.Description).HasMaxLength(4000);

                builder.HasOne(a => a.Class)
                       .WithMany(c => c.Assignments)
                       .HasForeignKey(a => a.ClassId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(a => a.Subject)
                       .WithMany()
                       .HasForeignKey(a => a.SubjectId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(a => a.Teacher)
                       .WithMany(t => t.Assignments)
                       .HasForeignKey(a => a.TeacherId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
        {
            public void Configure(EntityTypeBuilder<Submission> builder)
            {
                builder.ToTable("Submissions");
                builder.HasKey(s => s.SubmissionId);

                builder.Property(s => s.FileLink).IsRequired().HasMaxLength(1000);

                builder.HasOne(s => s.Assignment)
                       .WithMany(a => a.Submissions)
                       .HasForeignKey(s => s.AssignmentId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(s => s.Student)
                       .WithMany(st => st.Submissions)
                       .HasForeignKey(s => s.StudentId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
        {
            public void Configure(EntityTypeBuilder<Department> builder)
            {
                builder.ToTable("Departments");
                builder.HasKey(d => d.DepartmentId);

                builder.Property(d => d.DepartmentName).IsRequired().HasMaxLength(100);
                builder.Property(d => d.Description).HasMaxLength(200);

                builder.HasMany(d => d.Classes)
                       .WithOne(c => c.Department)
                       .HasForeignKey(c => c.DepartmentId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(d => d.Teachers)
                       .WithOne(t => t.Department)
                       .HasForeignKey(t => t.DepartmentId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
