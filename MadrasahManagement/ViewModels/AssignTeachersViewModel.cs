namespace MadrasahManagement.ViewModels
{
    public class AssignTeachersViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = default!;
        public string DepartmentName { get; set; } = default!;
        public List<SubjectAssignmentVM> Subjects { get; set; } = new List<SubjectAssignmentVM>();
    }

    public class SubjectAssignmentVM
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = default!;
        public string SubjectCode { get; set; } = default!;
        public bool IsOptional { get; set; }
        public int? CurrentTeacherId { get; set; }
        public string? CurrentTeacherName { get; set; }
        public List<TeacherOptionVM> AvailableTeachers { get; set; } = new List<TeacherOptionVM>();
    }

    public class TeacherOptionVM
    {
        public int TeacherId { get; set; }
        public string Name { get; set; } = default!;
        public string? Designation { get; set; }
        public string? Qualification { get; set; }
        public string DepartmentName { get; set; } = default!;
        public int WorkloadCount { get; set; }
    }
}
