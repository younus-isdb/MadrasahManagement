using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    public class TeacherApiDtp
    {
       
            [Required] public string Name { get; set; } = default!;
            [Required][EmailAddress] public string Email { get; set; } = default!;
            [Required] public string Contact { get; set; } = default!;
            [Required] public int DepartmentId { get; set; }
            [Required] public DateTime JoiningDate { get; set; }
            public string? Qualification { get; set; }
            public string? Designation { get; set; }
		
		public string? ImageUrl { get; set; }

	}

	// API Model
	public class EditTeacherApiModel
	{
		[Required]
		[StringLength(100)]
		public string Name { get; set; } = default!;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = default!;

		[Required]
		[StringLength(20)]
		public string Contact { get; set; } = default!;

		[Required]
		public int DepartmentId { get; set; }

		[Required]
		public string JoiningDate { get; set; } = default!;

		[StringLength(250)]
		public string? Qualification { get; set; }

		[StringLength(150)]
		public string? Designation { get; set; }

		public IFormFile? ImageFile { get; set; }

		public bool RemoveImage { get; set; }
	}

	public class UploadFileModel
	{
	
		public IFormFile File { get; set; }
	}
}
