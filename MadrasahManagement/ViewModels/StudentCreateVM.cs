using System.ComponentModel.DataAnnotations;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MadrasahManagement.ViewModels
{
	public class StudentCreateVM
	{
		// Form
	

		
		// Academic
		[Required]
		public int DepartmentId { get; set; }
		public int ClassId { get; set; }
		public int SectionId { get; set; }

		// Identity / Admission

		public string RollNo { get; set; } = "";
		public string AdmissionNumber { get; set; } = "";

		public string StudentName { get; set; } = "";
		public string? FatherName { get; set; }
		public DateTimeOffset AdmissionDate { get; set; } = DateTimeOffset.Now;

		// Names
		[Required]

		public string? BanglaStudentName { get; set; }

		// Parents
	
		public string? Mobile { get; set; }

		// Dropdowns
		public IEnumerable<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
		public IEnumerable<SelectListItem> Classes { get; set; } = new List<SelectListItem>();
		public IEnumerable<SelectListItem> Sections { get; set; } = new List<SelectListItem>();

		// Grid
		public List<Student> Students { get; set; } = new();

		
	}

}
