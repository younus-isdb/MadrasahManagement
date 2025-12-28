using MadrasahManagement.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class StudentEditVM
{
    [Required]
    public int StudentId { get; set; }

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
    [Required]
    public int DepartmentId { get; set; }

    [Required]
    public int ClassId { get; set; }

    [Required]
    public int SectionId { get; set; }

    // -------------------------
    // Identity & Admission
    // -------------------------
    [Required, MaxLength(20)]
    public string RegNo { get; set; } = default!;

    [MaxLength(50)]
    public string? NationalId { get; set; }

    /// <summary>
    /// Used for UI binding (converted to DateOnly in entity)
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime AdmissionDate { get; set; }

    // -------------------------
    // Personal Info
    // -------------------------
    public Gender? Gender { get; set; }

    [DataType(DataType.Date)]
    public DateTime DOB { get; set; }

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
    // Files (Optional)
    // -------------------------
    public IFormFile? ProfileImage { get; set; }
    public IFormFile? DocumentFile { get; set; }

    // Existing files (display purpose)
    public string? ExistingProfileImageUrl { get; set; }
    public string? ExistingDocumentUrl { get; set; }

    // -------------------------
    // Status
    // -------------------------
    public bool IsActive { get; set; }

    // -------------------------
    // Leaving Info
    // -------------------------
    [DataType(DataType.Date)]
    public DateTime? LeavingDate { get; set; }

    [MaxLength(300)]
    public string? LeavingReason { get; set; }
}
