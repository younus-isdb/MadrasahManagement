using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    // Create DTO
    public class StudentCreateDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string StudentName { get; set; } = string.Empty;

        public string? ArabicStudentName { get; set; }
        public string? BanglaStudentName { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required, MaxLength(20)]
        public string RegNo { get; set; } = string.Empty;

        public string? NationalId { get; set; }
        public DateOnly? AdmissionDate { get; set; }

        public int? Gender { get; set; }
        public DateTime? DOB { get; set; }

        public string? BloodGroup { get; set; }

        public string? FatherName { get; set; }
        public string? FatherPhone { get; set; }
        public string? MotherName { get; set; }
        public string? MotherPhone { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianPhone { get; set; }
        public string? GuardianEmail { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public string? EmergencyContactName { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? MedicalNotes { get; set; }

        public string? PreviousSchoolName { get; set; }
        public double? PreviousResult { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string? DocumentUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTimeOffset? LeavingDate { get; set; }
        public string? LeavingReason { get; set; }
    }

    // Update DTO
    public class StudentUpdateDto : StudentCreateDto
    {
        [Required]
        public int StudentId { get; set; }
    }

    // Read DTO
    public class StudentReadDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? ArabicStudentName { get; set; }
        public string? BanglaStudentName { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;

        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;

        public string RegNo { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public DateOnly? AdmissionDate { get; set; }

        public int? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? BloodGroup { get; set; }

        public bool IsActive { get; set; }
    }
}
