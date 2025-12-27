using Azure.Core;
using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using MadrasahManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly MadrasahDbContext _db;
        private readonly IUploadService _uploadService;

        public BookController(MadrasahDbContext db, IUploadService uploadService)
        {
            _db = db;
            _uploadService = uploadService;
        }


       
        [HttpGet]
        public async Task<ActionResult> Getall()
        {
            var books = await _db.Books.ToListAsync();
            return Ok(books);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] BookApiDto bookdto)
        {
            var book = new Book
            {
                Title = bookdto.Title,
                Author = bookdto.Author,
                ISBN = bookdto.ISBN,
                Category = bookdto.Category,
                TotalCopies = bookdto.TotalCopies,
                AvailableCopies = bookdto.TotalCopies,
                ImageUrl = bookdto.ImageUrl
            };

            _db.Books.Add(book);
            await _db.SaveChangesAsync();
            return Ok(book);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Edit(int id, [FromBody] BookApiDto bookdto, int newCopiesToAdd = 0)
        {
            try
            {
                var existingBook = await _db.Books.FindAsync(id);
                if (existingBook == null)
                    return NotFound();

                // Calculate new total after adding copies
                var newTotalCopies = existingBook.TotalCopies + newCopiesToAdd;

                // Check if reducing total copies below currently issued books
                var currentlyIssued = await _db.IssuedBooks
                    .CountAsync(a => a.BookId == id && a.ReturnDate == null);

                if (newTotalCopies < currentlyIssued)
                    return BadRequest($"Cannot reduce total copies. There are {currentlyIssued} copies currently issued.");

                var diff = newCopiesToAdd;

                existingBook.Title = bookdto.Title;
                existingBook.Author = bookdto.Author;
                existingBook.ISBN = bookdto.ISBN;
                existingBook.Category = bookdto.Category;
                existingBook.TotalCopies = newTotalCopies;
                existingBook.AvailableCopies = Math.Max(0, existingBook.AvailableCopies + diff);
                existingBook.ImageUrl = bookdto.ImageUrl;

                await _db.SaveChangesAsync();
                return Ok(existingBook);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Concurrency error occurred");
            }
            catch (DbUpdateException)
            {
                return Conflict("A book with the same title already exists");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error updating book");
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var book = await _db.Books
                    .Include(b => b.IssuedBooks)
                    .FirstOrDefaultAsync(b => b.BookId == id);

                if (book == null)
                    return NotFound();

                bool hasActiveIssues = book.IssuedBooks.Any(ib => ib.ReturnDate == null);
                if (hasActiveIssues)
                {
                    int activeCount = book.IssuedBooks.Count(ib => ib.ReturnDate == null);
                    return BadRequest($"Cannot delete. {activeCount} copy(s) currently issued.");
                }

                if (book.IssuedBooks.Any())
                    _db.IssuedBooks.RemoveRange(book.IssuedBooks);

                if (!string.IsNullOrEmpty(book.ImageUrl))
                    await _uploadService.FileDelete(book.ImageUrl);

                _db.Books.Remove(book);
                await _db.SaveChangesAsync();

                return Ok("Book deleted successfully");
            }
            catch (DbUpdateException)
            {
                return BadRequest("Unable to delete. Related records exist.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error deleting book.");
            }
        }
    }
}
