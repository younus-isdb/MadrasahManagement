using MadrasahManagement.Models;
using MadrasahManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{
    public class BookController : Controller
    {
        private readonly MadrasahDbContext _db;
        private readonly IUploadService _uploadService;

        public BookController(MadrasahDbContext db, IUploadService uploadService)
        {
            _db = db;
            _uploadService = uploadService;
        }


        // GET: BookController
        public async Task<ActionResult> Index()
        {
            var books = await _db.Books.ToListAsync();
            return View(books);
        }

        // GET: BookController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var books = await _db.Books.Include(a => a.IssuedBooks).FirstOrDefaultAsync(b => b.BookId == id);

            if (books == null)
            {
                return NotFound();
            }
            return View(books);
        }

        // GET: BookController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Book book)
        {
            if (!ModelState.IsValid)
                return View(book);

            try
            {
                book.AvailableCopies = book.TotalCopies;
                if (book.ImageFile != null && book.ImageFile.Length > 0)
                {
                    book.ImageUrl = await _uploadService.FileSave(book.ImageFile);
                }

                _db.Books.Add(book);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "A book with the same title already exists in this category.");
                return View(book);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the book. Please try again.");
                return View(book);
            }
        }

        // GET: BookController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var books = await _db.Books.FindAsync(id);
            if (books == null)
            {
                return NotFound();
            }

            return View(books);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Book book, int newCopiesToAdd = 0)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            try
            {
                var existingBook = await _db.Books.FindAsync(id);
                if (existingBook == null)
                {
                    return NotFound();
                }

                // Calculate new total after adding copies
                var newTotalCopies = existingBook.TotalCopies + newCopiesToAdd;

                // Check if reducing total copies below currently issued books
                var currentlyIssued = await _db.IssuedBooks.CountAsync(a => a.BookId == id && a.ReturnDate == null);

                if (newTotalCopies < currentlyIssued)
                {
                    ModelState.AddModelError("",
                        $"Cannot reduce total copies. There are {currentlyIssued} copies currently issued.");

                    // Return to view with current book data
                    return View(existingBook);
                }

                // Calculate available copies difference
                var diff = newCopiesToAdd;

                // Update book properties
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                existingBook.Category = book.Category;
                existingBook.TotalCopies = newTotalCopies;
                existingBook.AvailableCopies = Math.Max(0, existingBook.AvailableCopies + diff);

                // Handle image update if new file is provided
                if (book.ImageFile != null && book.ImageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingBook.ImageUrl))
                    {
                        await _uploadService.FileDelete(existingBook.ImageUrl);
                    }
                    // Save new image
                    existingBook.ImageUrl = await _uploadService.FileSave(book.ImageFile);
                }

                //// Validate the model state for required fields
                //if (string.IsNullOrEmpty(existingBook.Title) ||
                //    string.IsNullOrEmpty(existingBook.Author) ||
                //    string.IsNullOrEmpty(existingBook.ISBN))
                //{
                //    ModelState.AddModelError("", "Title, Author, and ISBN are required fields.");
                //    return View(existingBook);
                //}

                _db.Books.Update(existingBook);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "A book with the same title already exists .");
                return View(book);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the book. Please try again.");
                return View(book);
            }
        }

        private bool Exists(int id)
        {
            return _db.Books.Any(b => b.BookId == id);
        }



        // GET: BookController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var book = await _db.Books.Include(b => b.IssuedBooks).FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: BookController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var book = await _db.Books.
                    Include(b => b.IssuedBooks).FirstOrDefaultAsync(b => b.BookId == id);
                if (book == null)
                {
                    return NotFound();
                }

                bool hasActIssued = book.IssuedBooks.Any(a => a.ReturnDate == null);
                if (hasActIssued)
                {
                    ModelState.AddModelError("", "Cannot delete book. There are active issued copies that haven't been returned yet.");
                    return View("Delete", book);
                }

                _db.Books.Remove(book);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Unable to delete book. It may have related records in the system.");
                return View("Delete", await _db.Books.FindAsync(id));
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the book.");
                return View(await _db.Books.FindAsync(id));
            }
        }


    }
}