using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Services;
using System.Drawing.Drawing2D;

namespace SchoolManagementSystem.Controllers
{
    public class IssuedBookController : Controller
    {
        private readonly SchoolDbContext _db;


        public IssuedBookController(SchoolDbContext db)
        {
            _db = db;

        }


        //Get: IssuedBookController
        public async Task<IActionResult> Index()
        {
            var issued = await _db.IssuedBooks
                .Include(a => a.Book)
                .Include(a => a.AppUser)
                .OrderByDescending(a => a.IssueDate).ToListAsync();
            return View(issued);
        }

        // GET: IssuedBookController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var issued = await _db.IssuedBooks.Include(a => a.Book).Include(a => a.AppUser).FirstOrDefaultAsync(a => a.Id == id);
            if (issued == null)
            {
                return NotFound();
            }
            return View(issued);
        }

        // GET: IssuedBookController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Books = await _db.Books.Where(b => b.AvailableCopies > 0).ToListAsync();
            ViewBag.BookCount = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string operation, int bookCount, string UserSearch, string UserType,
            string Class, string Section, int? RollNumber, List<int> BookIds)
        {
            // Set common ViewBag properties
            ViewBag.Books = await _db.Books.Where(b => b.AvailableCopies > 0).ToListAsync();
            ViewBag.BookCount = bookCount;
            ViewBag.UserSearch = UserSearch;
            ViewBag.UserType = UserType;
            ViewBag.Class = Class;
            ViewBag.Section = Section;
            ViewBag.RollNumber = RollNumber;
            ViewBag.SelectedBookIds = BookIds ?? new List<int>();

            if (operation == "searchuser")
            {
                if (!string.IsNullOrEmpty(UserSearch))
                {
                    var user = await _db.Users
                        .Where(u => u.UserName == UserSearch || u.NormalizedUserName == UserSearch.ToUpper())
                        .FirstOrDefaultAsync();
                    ViewBag.UserVerified = user != null;
                }
                return View();
            }

            if (operation == "add")
            {
                ViewBag.BookCount = bookCount + 1;
                return View();
            }

            if (operation.StartsWith("delete-"))
            {
                int.TryParse(operation.Replace("delete-", ""), out int index);
                if (index >= 0 && bookCount > 1)
                {
                    bookCount--;
                    ViewBag.BookCount = bookCount;
                    if (BookIds != null && BookIds.Count > index)
                    {
                        BookIds.RemoveAt(index);
                        ViewBag.SelectedBookIds = BookIds;
                    }
                }
                return View();
            }

            if (operation == "submit")
            {
                if (string.IsNullOrEmpty(UserSearch))
                {
                    ModelState.AddModelError("", "Username is required.");
                    return View();
                }

                if (BookIds == null || !BookIds.Any())
                {
                    ModelState.AddModelError("", "Please select at least one book.");
                    return View();
                }

                var user = await _db.Users
                    .Where(u => u.UserName == UserSearch || u.NormalizedUserName == UserSearch.ToUpper())
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    ModelState.AddModelError("", "User not found. Please verify the username first.");
                    ViewBag.UserVerified = false;
                    return View();
                }

                try
                {
                    foreach (var bookId in BookIds)
                    {
                        var book = await _db.Books.FindAsync(bookId);
                        if (book == null || book.AvailableCopies <= 0)
                        {
                            ModelState.AddModelError("", $"Book '{book?.Title}' is not available.");
                            return View();
                        }

                        var issuedBook = new IssuedBook
                        {
                            BookId = bookId,
                            IssuedTo = user.Id,
                            UserFullName = user.UserName,
                            UserType = UserType,
                            Class = Class,
                            Section = Section,
                            RollNumber = RollNumber,
                            IssueDate = DateTime.Now,
                            ReturnDate = null,
                            Fine = 0
                        };

                        _db.IssuedBooks.Add(issuedBook);
                        book.AvailableCopies--;
                    }

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to issue books. Error: {ex.Message}");
                    return View();
                }
            }

            return View();
        }


        // GET: IssuedBookController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var issuedbook = await _db.IssuedBooks.Include(b => b.Book).Include(b => b.AppUser).FirstOrDefaultAsync(b => b.Id == id);
            if (issuedbook == null)
            {
                return NotFound();
            }

            ViewBag.Books = await _db.Books.ToListAsync();
            ViewBag.Users = await _db.Users.ToListAsync();

            return View(issuedbook);
        }


        // POST: IssuedBookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IssuedBook issuedBook)
        {
            if (id!=issuedBook.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(issuedBook);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!IssuedBookExists(id))
                    {
                        return NotFound();
                    }
                }
            }
            ViewBag.Books = await _db.Books.ToListAsync();
            ViewBag.Users = await _db.Users.ToListAsync();

            return View(issuedBook);
        }


        // GET: IssuedBookController/Return/5
        public async Task<IActionResult> Return(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var issuedBook = await _db.IssuedBooks
                .Include(ib => ib.Book)
                .Include(ib => ib.AppUser)
                .FirstOrDefaultAsync(ib => ib.Id == id);

            if (issuedBook == null)
            {
                return NotFound();
            }

         
            if (issuedBook.ReturnDate == null)
            {
                var dueDate = issuedBook.IssueDate.AddDays(14);
                if (DateTimeOffset.Now > dueDate)
                {
                    var daysLate = (DateTimeOffset.Now - dueDate).Days;
                    issuedBook.Fine = daysLate * 10; // 10 per day
                }
            }

            return View(issuedBook);
        }

        // POST: IssuedBookController/Return/5
        [HttpPost]
        [ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int id)
        {
            try
            {
                var issuedBook = await _db.IssuedBooks
                    .Include(ib => ib.Book)
                    .FirstOrDefaultAsync(ib => ib.Id == id);

                if (issuedBook == null)
                {
                    return NotFound();
                }

                if (issuedBook.ReturnDate != null)
                {
                    ModelState.AddModelError("", "This book has already been returned.");
                    return View(issuedBook);
                }

               
                issuedBook.ReturnDate = DateTimeOffset.Now;

              
                var dueDate = issuedBook.IssueDate.AddDays(14);
                if (issuedBook.ReturnDate > dueDate)
                {
                    var daysLate = (issuedBook.ReturnDate.Value - dueDate).Days;

                    issuedBook.Fine = daysLate * 10; 
                }

               
                if (issuedBook.Book != null)
                {
                    issuedBook.Book.ReturnBook();
                }

                _db.Update(issuedBook);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing the return.");
                return View(await _db.IssuedBooks
                    .Include(ib => ib.Book)
                    .Include(ib => ib.AppUser)
                    .FirstOrDefaultAsync(ib => ib.Id == id));
            }
        }

        // GET: IssuedBookController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var issuedBook = await _db.IssuedBooks
                .Include(ib => ib.Book)
                .Include(ib => ib.AppUser)
                .FirstOrDefaultAsync(ib => ib.Id == id);

            if (issuedBook == null)
            {
                return NotFound();
            }

            return View(issuedBook);
        }

        // POST: IssuedBookController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var issuedBook = await _db.IssuedBooks
                    .Include(ib => ib.Book)
                    .FirstOrDefaultAsync(ib => ib.Id == id);

                if (issuedBook == null)
                {
                    return NotFound();
                }

                
                if (issuedBook.ReturnDate == null && issuedBook.Book != null)
                {
                    issuedBook.Book.AvailableCopies++;
                }

                _db.IssuedBooks.Remove(issuedBook);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the record.");

                return View(await _db.IssuedBooks.Include(ib => ib.Book).Include(ib => ib.AppUser).FirstOrDefaultAsync(ib => ib.Id == id));
            }
        }



        private bool IssuedBookExists(int id)
        {
            return _db.IssuedBooks.Any(e => e.Id == id);
        }
    }
}