using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MadrasahManagement.Models;
using MadrasahManagement.Services;
using System.Threading.Tasks;

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
            try
            {
                if (ModelState.IsValid)
                {
                    book.AvailableCopies = book.TotalCopies;
                    _db.Books.Add(book);
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }

            catch
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again.");
                return View();
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

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();

            }
            if (ModelState.IsValid)
            {
                try
                {
                    var exixtingbook = await _db.Books.FindAsync(id);
                    if (exixtingbook == null)
                    {
                        return NotFound();
                    }

                    var currentlyIssued = await _db.IssuedBooks.CountAsync(a => a.BookId == id && a.ReturnDate == null);

                    if (book.TotalCopies < currentlyIssued)
                    {
                        ModelState.AddModelError("TotalCopies",
                $"Cannot reduce total copies to {book.TotalCopies}. There are {currentlyIssued} copies currently issued.");
                        return View(book);
                    }

                    var diff = book.TotalCopies - exixtingbook.TotalCopies;

                    exixtingbook.Title = book.Title;
                    exixtingbook.Author = book.Author;
                    exixtingbook.ISBN = book.ISBN;
                    exixtingbook.Category = book.Category;
                    exixtingbook.TotalCopies = book.TotalCopies;
                    exixtingbook.AvailableCopies = Math.Max(0, exixtingbook.AvailableCopies + diff);

                    _db.Update(exixtingbook);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Bookexixts(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(book);


        }
        private bool Bookexixts(int id)
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

        private bool BookExists(int id)
        {
            return  _db.Books.Any(a => a.BookId == id);
        }
    }
}
