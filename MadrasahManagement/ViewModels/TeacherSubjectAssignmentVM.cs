using MadrasahManagement.Models;
using System.ComponentModel;

namespace MadrasahManagement.ViewModels
{
    public class TeacherSubjectAssignmentVM
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = default!;
        public string? TeacherDepartment { get; set; }

        //// Filter options
        //[DisplayName("Filter by Department")]
        //public int? SelectedDepartmentId { get; set; }

        //[DisplayName("Filter by Class")]
        //public int? SelectedClassId { get; set; }

        // Selected subjects
        [DisplayName("Select Subjects")]
        public List<int> SelectedSubjectIds { get; set; } = new List<int>();

        //// Lists for dropdowns
        //public List<Department> Departments { get; set; } = new List<Department>();
        //public List<Class> Classes { get; set; } = new List<Class>();

        // Subjects list
        public List<SubjectCheckbox> AvailableSubjects { get; set; } = new List<SubjectCheckbox>();
    }

    public class SubjectCheckbox
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = default!;
        public string SubjectCode { get; set; } = default!;
        public string ClassName { get; set; } = default!;
        public string DepartmentName { get; set; } = default!;
        public bool IsSelected { get; set; }
    }
}
