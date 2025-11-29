using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Models
{
    // ================================
    //      CUSTOM APPLICATION USER
    // ================================
    // AppUser = IdentityUser কে কাস্টমাইজ করা সংস্করণ
    // এতে আমরা প্রজেক্ট অনুযায়ী অতিরিক্ত ফিচার যোগ করতে পারি।
    //
    // ASP.NET Core Identity এর default IdentityUser এর primary key হলো string (GUID)।
    // তাই অন্য মডেলে FK হিসেবে ব্যবহার করাও সহজ।
    // ================================

    public class AppUser : IdentityUser
    {
        // ---------- BASIC PROFILE ----------

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
    //      CUSTOM APPLICATION ROLE
    // ================================
    // AppRole = IdentityRole কাস্টমাইজড
    // ভবিষ্যতে Role Description বা Permissions যোগ করতে পারবেন।
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
}
