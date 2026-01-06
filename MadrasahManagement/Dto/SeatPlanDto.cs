namespace MadrasahManagement.Dto
{
    public class SeatPlanReadDto
    {
        public int SeatPlanId { get; set; }
        public DateTime ExamDate { get; set; }
        public int RoomNumber { get; set; }
        public int NumberOfRows { get; set; }
        public int StudentsPerBench { get; set; }

        public int StudentId { get; set; }
        public string RegNo { get; set; } = "";
        public string StudentName { get; set; } = "";

        public int ClassId { get; set; }
        public string ClassName { get; set; } = "";

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = "";

        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
    }
    public class SeatPlanCreateDto
    {
        public DateTime ExamDate { get; set; }
        public int RoomNumber { get; set; }
        public int NumberOfRows { get; set; }
        public int StudentsPerBench { get; set; }

        // Student-wise selections
        public List<int> StudentIds { get; set; } = new();
        public List<int> ClassIds { get; set; } = new();
        public List<int> DepartmentIds { get; set; } = new();
        public List<int> SubjectIds { get; set; } = new();
    }

}
