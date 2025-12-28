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
            if (string.IsNullOrEmpty(request.UserEmail))
                return BadRequest("Username is required.");

            if (request.BookTitles == null || !request.BookTitles.Any())
                return BadRequest("Select at least one book.");

            var user = await _db.Users
                .Where(u => u.UserName == request.UserEmail || u.NormalizedUserName == request.UserEmail.ToUpper())
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

        [HttpPut("edit/{userId}")]
        public async Task<ActionResult> UpdateIssuedBooks(string userId, [FromBody] UpdateIssueRequest request)
        {
            if (!Guid.TryParse(userId, out _))
                return BadRequest("Invalid user ID");

            if (request.BookTitles == null || !request.BookTitles.Any())
                return BadRequest("Select at least one book");

            try
            {
                var allExistingBooks = await _db.IssuedBooks
                    .Include(b => b.Book)
                    .Where(b => b.IssuedTo == userId && b.ReturnDate == null)
                    .ToListAsync();

                var validBookTitles = request.BookTitles.Distinct().Where(title => !string.IsNullOrEmpty(title)).ToList();
                var books = await _db.Books.Where(b => validBookTitles.Contains(b.Title)).ToListAsync();

                var bookTitlesToKeep = allExistingBooks
                    .Where(b => validBookTitles.Contains(b.Book.Title))
                    .Select(b => b.Book.Title)
                    .ToList();

                var bookTitlesToAdd = validBookTitles
                    .Where(title => !allExistingBooks.Any(b => b.Book.Title == title))
                    .ToList();

                var bookTitlesToRemove = allExistingBooks
                    .Where(b => !validBookTitles.Contains(b.Book.Title))
                    .Select(b => b.Book.Title)
                    .ToList();

                foreach (var title in bookTitlesToRemove)
                {
                    var book = books.FirstOrDefault(b => b.Title == title);
                    if (book != null)
                    {
                        book.AvailableCopies++;
                    }
                    var issuedBookToRemove = allExistingBooks.FirstOrDefault(b => b.Book.Title == title);
                    if (issuedBookToRemove != null)
                    {
                        _db.IssuedBooks.Remove(issuedBookToRemove);
                    }
                }

                foreach (var title in bookTitlesToAdd)
                {
                    var book = books.FirstOrDefault(b => b.Title == title);
                    if (book == null || book.AvailableCopies <= 0)
                        return BadRequest($"Book '{title}' is not available");

                    var newIssuedBook = new IssuedBook
                    {
                        BookId = book.BookId,
                        IssuedTo = userId,
                        UserFullName = request.UserFullName,
                        UserType = request.UserType,
                        IssueDate = DateOnly.FromDateTime(DateTime.Now),
                        Fine = 0
                    };

                    if (request.UserType == "Student")
                    {
                        newIssuedBook.Class = request.Class;
                        newIssuedBook.Section = request.Section;
                        newIssuedBook.RollNumber = request.RollNumber;
                    }

                    _db.IssuedBooks.Add(newIssuedBook);
                    book.AvailableCopies--;
                }

                foreach (var title in bookTitlesToKeep)
                {
                    var existingBook = allExistingBooks.FirstOrDefault(b => b.Book.Title == title);
                    if (existingBook != null)
                    {
                        existingBook.UserFullName = request.UserFullName;
                        existingBook.UserType = request.UserType;

                        if (request.UserType == "Student")
                        {
                            existingBook.Class = request.Class;
                            existingBook.Section = request.Section;
                            existingBook.RollNumber = request.RollNumber;
                        }
                        else
                        {
                            existingBook.Class = "";
                            existingBook.Section = "";
                            existingBook.RollNumber = null;
                        }
                    }
                }

                await _db.SaveChangesAsync();
                return Ok("Updated");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error");
            }
        }

        [HttpGet("return/{id}")]
        public async Task<ActionResult> GetReturn(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            var issuedBook = await _db.IssuedBooks
                .Include(ib => ib.Book)
               
                .FirstOrDefaultAsync(ib => ib.Id == id);

            if (issuedBook == null)
                return NotFound();

            if (issuedBook.ReturnDate == null)
            {
                var dueDate = issuedBook.IssueDate.AddDays(14);
                if (DateOnly.FromDateTime(DateTime.Now) > dueDate)
                {
                    var daysLate = (DateOnly.FromDateTime(DateTime.Now).DayNumber - dueDate.DayNumber);
                    issuedBook.Fine = daysLate * 10;
                }
            }

            return Ok(issuedBook);
        }

        [HttpPost("return/{id}")]
        public async Task<ActionResult> ReturnBook(int id)
        {
            try
            {
                var issuedBook = await _db.IssuedBooks
                    .Include(ib => ib.Book)
                    .FirstOrDefaultAsync(ib => ib.Id == id);

                if (issuedBook == null)
                    return NotFound();

                if (issuedBook.ReturnDate != null)
                    return BadRequest("Book already returned");

                var dueDate = issuedBook.IssueDate.AddDays(14);
                if (DateOnly.FromDateTime(DateTime.Now) > dueDate)
                {
                    var daysLate = (DateOnly.FromDateTime(DateTime.Now).DayNumber - dueDate.DayNumber);
                    issuedBook.Fine = daysLate * 10;
                }

                issuedBook.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
                if (issuedBook.Book != null)
                {
                    issuedBook.Book.AvailableCopies++;
                }

                await _db.SaveChangesAsync();
                return Ok("Book returned");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error returning book");
            }
        }
    }
}
