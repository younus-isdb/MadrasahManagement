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
        Present = 1,
        Absent = 2,
        Late = 3,
        Excused = 4
    }

    public enum FeeFrequency
    {
        Monthly,
        Quarterly,
        Yearly,
        OneTime
    }
    public enum PaymentMethodType
    {
        Cash,
        Bkash,
        Nagad,
        Bank,
        Online
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2,
        Refunded = 3
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

    public enum Month
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December

    }
}
