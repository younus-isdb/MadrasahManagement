using MadrasahManagement.Models;
using System.ComponentModel.DataAnnotations;

public class StudentCreateVM
{
    // -------------------------
    // Basic
    // -------------------------
    [Required, MaxLength(150)]
    public string StudentName { get; set; } = default!;

    public string? ArabicStudentName { get; set; }
    public string? BanglaStudentName { get; set; }

    // -------------------------
    // Academic
    // -------------------------
    [Required]
    public int DepartmentId { get; set; }

    [Required]
    public int ClassId { get; set; }

    [Required]
    public int SectionId { get; set; }

    // -------------------------
    // Identity
    // -------------------------
    [Required]
    public string RegNo { get; set; } = default!;
    public string? NationalId { get; set; }

    public DateTime AdmissionDate { get; set; } = DateTime.Today;

    // -------------------------
    // Personal
    // -------------------------
    public Gender? Gender { get; set; }
    public DateTime DOB { get; set; } = DateTime.Today.AddYears(-5);
    public string? BloodGroup { get; set; }

    // -------------------------
    // Parents
    // -------------------------
    public string? FatherName { get; set; }
    public string? FatherPhone { get; set; }
    public string? MotherName { get; set; }
    public string? MotherPhone { get; set; }

    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianEmail { get; set; }

    // -------------------------
    // Address
    // -------------------------
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    // -------------------------
    // Emergency
    // -------------------------
    public string? EmergencyContactName { get; set; }
    public string? EmergencyPhone { get; set; }
    public string? MedicalNotes { get; set; }

    // -------------------------
    // Previous Academic
    // -------------------------
    public string? PreviousSchoolName { get; set; }
    public double? PreviousResult { get; set; }

    // -------------------------
    // Files
    // -------------------------
    public IFormFile? ProfileImage { get; set; }
    public IFormFile? DocumentFile { get; set; }

    public bool IsActive { get; set; } = true;
}
