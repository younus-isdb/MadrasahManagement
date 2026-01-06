namespace MadrasahManagement.Dto
{
    public class ResultProcessCreateDto
    {
        public string EducationYear { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int ClassId { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }

        public List<ResultSubjectInputDto> Subjects { get; set; } = new();
    }
    public class ResultProcessUpdateDto : ResultProcessCreateDto
    {
        public int ResultProcessId { get; set; }
    }

    public class ResultSubjectInputDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty; // ✅ initialized
        public int Marks { get; set; }
    }
}