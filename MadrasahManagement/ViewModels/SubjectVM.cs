using MadrasahManagement.Dto;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    public class SubjectVM
    {
           public int SubjectId { get; set; }

            [Display(Name = "Subject Name")]
            public string SubjectName { get; set; } = default!;

            [Display(Name = "Subject Code")]
            public string SubjectCode { get; set; } = default!;

            [Display(Name = "Class")]
            public string ClassName { get; set; } = default!;

            [Display(Name = "Department")]
            public string DepartmentName { get; set; } = default!;

            [Display(Name = "Type")]
            public string SubjectType => IsOptional ? "Optional" : "Mandatory";

            [Display(Name = "Assigned Teachers")]
            public int TeacherCount { get; set; }

            //[Display(Name = "Total Students")]
            //public int StudentCount { get; set; }

            public bool IsOptional { get; set; }
            public int ClassId { get; set; }
            public int DepartmentId { get; set; }
        }

        // DTOs/Subject/SubjectDetailsViewModel.cs
        public class SubjectDetailsViewModel
        {
            public int SubjectId { get; set; }
            public string SubjectName { get; set; } = default!;
            public string SubjectCode { get; set; } = default!;
            public string ClassName { get; set; } = default!;
            public string DepartmentName { get; set; } = default!;
            public bool IsOptional { get; set; }
            public string SubjectType => IsOptional ? "Optional" : "Mandatory";

            public List<AssignedTeacherDto> AssignedTeachers { get; set; } = new();
            public List<ClassStudentCountDto> StudentCountByClass { get; set; } = new();
            public int TotalAssignedTeachers { get; set; }
            public int TotalStudents { get; set; }
        }
    
}
