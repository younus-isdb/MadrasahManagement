using MadrasahManagement.Models;
using System.ComponentModel.DataAnnotations;

public class StudentEditVM
{
    [Required]
    public int StudentId { get; set; }

    // Basic Info
    [Required, MaxLength(150)]
    public string StudentName { get; set; } = default!;

    [MaxLength(150)]
    public string? ArabicStudentName { get; set; }

    [MaxLength(150)]
    public string? BanglaStudentName { get; set; }

    // Admission Date
    [DataType(DataType.Date)]
    public DateTime AdmissionDate { get; set; }

    // Personal Info
    public Gender? Gender { get; set; }

    [DataType(DataType.Date)]
    public DateTime DOB { get; set; } // Changed to nullable

    [MaxLength(5)]
    public string? BloodGroup { get; set; }

    // Parents/Guardians
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

    // Address
    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(150)]
    public string? City { get; set; }

    // Emergency
    [MaxLength(150)]
    public string? EmergencyContactName { get; set; }

    [MaxLength(20)]
    public string? EmergencyPhone { get; set; }

    [MaxLength(500)]
    public string? MedicalNotes { get; set; }

    // Files
    public IFormFile? ProfileImage { get; set; }
    public IFormFile? DocumentFile { get; set; }

    public string? ExistingProfileImageUrl { get; set; }
    public string? ExistingDocumentUrl { get; set; }

    // Status
    public bool IsActive { get; set; }

    [DataType(DataType.Date)]
    public DateTime? LeavingDate { get; set; }

    [MaxLength(300)]
    public string? LeavingReason { get; set; }

    // For DISPLAY ONLY (cannot edit)
    public string? DepartmentName { get; set; }
    public string? ClassName { get; set; }
    public string? SectionName { get; set; }
    public string? RegNo { get; set; } // Added for display
    public string? NationalId { get; set; } // Added for display
    public string? Country { get; set; } // Added for display
    public string? PreviousSchoolName { get; set; } // Added for display
    public double? PreviousResult { get; set; } // Added for display
}