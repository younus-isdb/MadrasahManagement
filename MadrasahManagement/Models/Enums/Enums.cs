namespace MadrasahManagement.Models
{
    public enum Audience
    {
        Student,
        Teacher,
        Everyone
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late,
        Excused
    }

    public enum FeeFrequency
    {
        Monthly,
        Quarterly,
        Yearly,
        OneTime
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
    public enum BloodGroup
    {
        A_Positive,
        A_Negative,
        B_Positive,
        B_Negative,
        AB_Positive,
        AB_Negative,
        O_Positive,
        O_Negative
    }
}
