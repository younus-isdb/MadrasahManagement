using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize(Roles = "Admin,Librarian")]
    public class IssuedBookController : ControllerBase
    {
        private readonly MadrasahDbContext _db;


        public IssuedBookController(MadrasahDbContext db)
        {
            _db = db;

        }


        //Get: IssuedBookController
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var issued = await _db.IssuedBooks
                .Include(a => a.Book)
               
                .OrderByDescending(a => a.IssueDate).Where(i => i.ReturnDate == null).ToListAsync();
            return Ok(issued);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get()
        {
            var issued = await _db.IssuedBooks
                .Include(a => a.Book)
               
                .OrderByDescending(a => a.IssueDate).Where(i => i.ReturnDate == null).ToListAsync();
            return Ok(issued);
        }


        [HttpPost("issue")]
        public async Task<ActionResult> IssueBooks([FromBody] IssueRequest request)
        {
            if (string.IsNullOrEmpty(request.Username))
                return BadRequest("Username is required.");

            if (request.BookTitles == null || !request.BookTitles.Any())
                return BadRequest("Select at least one book.");

            var user = await _db.Users
                .Where(u => u.UserName == request.Username || u.NormalizedUserName == request.Username.ToUpper())
                .FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("User not found.");

            try
            {
                foreach (var bookTitle in request.BookTitles)
                {
                    var book = await _db.Books
                        .FirstOrDefaultAsync(b => b.Title == bookTitle);

                    if (book == null || book.AvailableCopies <= 0)
                        return BadRequest($"Book '{bookTitle}' is not available.");

                    var issuedBook = new IssuedBook
                    {
                        BookId = book.BookId,
                        IssuedTo = user.Id,
                        UserFullName = user.UserName,
                        UserType = request.UserType,
                        Class = request.Class,
                        Section = request.Section,
                        RollNumber = request.RollNumber,
                        IssueDate = DateOnly.FromDateTime(DateTime.Now),
                        Fine = 0
                    };

                    _db.IssuedBooks.Add(issuedBook);
                    book.AvailableCopies--;
                }

                await _db.SaveChangesAsync();
                return Ok("Books issued successfully");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error issuing books");
            }
        }

     

    }
}
