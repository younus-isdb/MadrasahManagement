using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.Dto
{
    public class BookApiDto
    {
        [Required, MaxLength(250)]
        public string Title { get; set; } = default!;

        [MaxLength(150)]
        public string? Author { get; set; }

        [MaxLength(20)]
        public string? ISBN { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        
        public int TotalCopies { get; set; }

        public string? ImageUrl { get; set; }

      
      
    }
    public class IssueRequest
    {
        [EmailAddress]
        public string UserEmail { get; set; }
        public string UserType { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public int? RollNumber { get; set; }
        public List<string> BookTitles { get; set; }
    }
    public class UpdateIssueRequest
    {
        public string UserFullName { get; set; }
        public string UserType { get; set; }
        public List<string> BookTitles { get; set; }

        public string Class { get; set; }
        public string Section { get; set; }
        public int? RollNumber { get; set; }
    }
}
