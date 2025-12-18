namespace MadrasahManagement.Services
{
    public class StudentIdGenerator
    {

             // classId will be padded to 2 digits if you want fixed width
            public static string Generate(int classId, int countForClass)
            {
                string classPart = classId.ToString("D2"); // e.g., 07
                string year = DateTime.Now.ToString("yy"); // last two digits, e.g., 25
                string serial = countForClass.ToString("D4"); // 0001

                return $"{classPart}{year}{serial}";
            }
   

    }
}
