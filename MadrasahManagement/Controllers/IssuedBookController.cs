using MadrasahManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MadrasahManagement.Controllers
{
    public class IssuedBookController : Controller
    {
        private readonly MadrasahDbContext _db;


        public IssuedBookController(MadrasahDbContext db)
        {
            _db = db;

        }


        //Get: IssuedBookController
        public async Task<IActionResult> Index()
        {
            var issued = await _db.IssuedBooks
                .Include(a => a.Book)
                .Include(a => a.AppUser)
                .OrderByDescending(a => a.IssueDate).Where(i => i.ReturnDate == null).ToListAsync();
            return View(issued);
        }

        // GET: IssuedBookController/Details/5
        public async Task<IActionResult> Details(string userId)
        {
            //if (id <= 0)
            //{
            //    return NotFound();
            //}

            //var issued = await _db.IssuedBooks.Include(a => a.Book).Include(a => a.AppUser).FirstOrDefaultAsync(a => a.Id == id);
            //if (issued == null)
            //{
            //    return NotFound();
            //}
            //return View(issued);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return NotFound();
            }

            // Get ONLY ACTIVE (not returned) issued books for this user
            var issuedBooks = await _db.IssuedBooks
                .Include(b => b.Book)
                .Include(b => b.AppUser)
                .Where(b => b.IssuedTo == userGuid.ToString() && b.ReturnDate == null) // Only active issues
                .ToListAsync();



            if (!issuedBooks.Any())
            {
                return NotFound();
            }

            var firstBook = issuedBooks.First();

            // Get book IDs currently ACTIVE for this user
            var bookIds = issuedBooks.Select(b => b.BookId).ToList();

            //// Setup ViewBag
            //ViewBag.UserSearch = firstBook.UserFullName;
            //ViewBag.UserType = firstBook.UserType;
            //ViewBag.Class = firstBook.Class;
            //ViewBag.Section = firstBook.Section;
            //ViewBag.RollNumber = firstBook.RollNumber;
            ViewBag.SelectedBookIds = bookIds;
            ViewBag.BookCount = issuedBooks.Count;
            ViewBag.UserIdString = userId;

            // Load books that are available OR currently issued to this user
            ViewBag.Books = await _db.Books
                .Where(b => b.AvailableCopies > 0 || bookIds.Contains(b.BookId))
                .ToListAsync();

            return View(issuedBooks);




        }

        // GET: IssuedBookController/Create
        public IActionResult Create()
        {
            ViewBag.BookCount = 0;
            ViewBag.SelectedBookIds = new List<int>();
            ViewBag.Books = _db.Books.ToList();
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
                bookCount++;
            }
            else if (operation != null && operation.StartsWith("delete"))
            {
                var index = int.Parse(operation.Split('-')[1]);
                BookIds.RemoveAt(index);
                bookCount--;
            }

            ViewBag.BookCount = bookCount;
            ViewBag.SelectedBookIds = BookIds ?? new List<int>();
            ViewBag.Books = _db.Books.ToList();

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
                            IssueDate = DateOnly.FromDateTime(DateTime.Now),

                            // ReturnDate = null,
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



        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return NotFound();
            }

            //only for active users
            var issuedBooks = await _db.IssuedBooks
                .Include(b => b.Book)
                .Include(b => b.AppUser)
                .Where(b => b.IssuedTo == userGuid.ToString() && b.ReturnDate == null) // Only active issues
                .ToListAsync();

            if (!issuedBooks.Any())
            {
                return NotFound();
            }

            var firstBook = issuedBooks.First();

            //  book ids for current user
            var bookIds = issuedBooks.Select(b => b.BookId).ToList();

            // Setup ViewBag
            ViewBag.UserSearch = firstBook.UserFullName;
            ViewBag.UserType = firstBook.UserType;
            ViewBag.Class = firstBook.Class;
            ViewBag.Section = firstBook.Section;
            ViewBag.RollNumber = firstBook.RollNumber;
            ViewBag.SelectedBookIds = bookIds;
            ViewBag.BookCount = issuedBooks.Count;
            ViewBag.UserIdString = userId;

            // Load books that are available or currently issued to this user
            ViewBag.Books = await _db.Books
                .Where(b => b.AvailableCopies > 0 || bookIds.Contains(b.BookId))
                .ToListAsync();

            return View(firstBook);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string userId, string operation, int bookCount,
     string UserSearch, string UserType, string Class, string Section, int? RollNumber, List<int> BookIds)
        {
            // Convert userId to Guid
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return NotFound();
            }

            // Get active issued books
            var activeIssuedBooks = await _db.IssuedBooks
                .Include(b => b.Book)
                .Where(b => b.IssuedTo == userGuid.ToString() && b.ReturnDate == null)
                .ToListAsync();

            if (!activeIssuedBooks.Any())
            {
                return NotFound();
            }

            var firstBook = activeIssuedBooks.First();

            // Handle null BookIds
            var currentBookIds = BookIds ?? activeIssuedBooks.Select(b => b.BookId).ToList();

            // Setup ViewBag
            ViewBag.UserSearch = UserSearch ?? firstBook.UserFullName;
            ViewBag.UserType = UserType ?? firstBook.UserType;
            ViewBag.Class = Class ?? firstBook.Class;
            ViewBag.Section = Section ?? firstBook.Section;
            ViewBag.RollNumber = RollNumber ?? firstBook.RollNumber;
            ViewBag.SelectedBookIds = currentBookIds;
            ViewBag.BookCount = bookCount > 0 ? bookCount : activeIssuedBooks.Count;
            ViewBag.UserIdString = userId;

            // Load books
            var activeBookIds = activeIssuedBooks.Select(b => b.BookId).ToList();
            ViewBag.Books = await _db.Books
                .Where(b => b.AvailableCopies > 0 || activeBookIds.Contains(b.BookId))
                .ToListAsync();

            if (operation == "add")
            {
                currentBookIds.Add(0);
                ViewBag.SelectedBookIds = currentBookIds;
                ViewBag.BookCount = bookCount + 1;
                return View(firstBook);
            }

            if (operation.StartsWith("delete-"))
            {
                int.TryParse(operation.Replace("delete-", ""), out int index);

                if (index >= 0 && currentBookIds.Count > index)
                {
                    currentBookIds.RemoveAt(index);
                    ViewBag.SelectedBookIds = currentBookIds;
                    ViewBag.BookCount = bookCount - 1;
                }

                return View(firstBook);
            }

            if (operation == "searchuser")
            {
                if (!string.IsNullOrEmpty(UserSearch))
                {
                    var user = await _db.Users
                        .Where(u => u.UserName == UserSearch || u.NormalizedUserName == UserSearch.ToUpper())
                        .FirstOrDefaultAsync();
                    ViewBag.UserVerified = user != null;
                }
                return View(firstBook);
            }

            if (operation == "submit")
            {
                ModelState.Clear();

                if (string.IsNullOrEmpty(UserSearch))
                    ModelState.AddModelError("UserSearch", "Username is required.");

                if (string.IsNullOrEmpty(UserType))
                    ModelState.AddModelError("UserType", "User type is required.");

                // Student validation
                if (UserType == "Student")
                {
                    if (string.IsNullOrEmpty(Class))
                        ModelState.AddModelError("Class", "Class is required for students.");

                    if (string.IsNullOrEmpty(Section))
                        ModelState.AddModelError("Section", "Section is required for students.");
                }
                else
                {
                    Class = "";
                    Section = "";
                    RollNumber = null;
                    ViewBag.Class = "";
                    ViewBag.Section = "";
                    ViewBag.RollNumber = null;
                }

                var validBookIds = currentBookIds.Where(id => id > 0).Distinct().ToList();

                if (!validBookIds.Any())
                {
                    ModelState.AddModelError("", "Please select at least one book.");
                }

                if (!ModelState.IsValid)
                {
                    return View(firstBook);
                }

                try
                {

                    var user = await _db.Users
                        .Where(u => u.UserName == UserSearch || u.NormalizedUserName == UserSearch.ToUpper())
                        .FirstOrDefaultAsync();

                    if (user == null)
                    {
                        ModelState.AddModelError("", "User not found.");
                        ViewBag.UserVerified = false;
                        return View(firstBook);
                    }

                    var allExistingBooks = await _db.IssuedBooks
                        .Where(b => b.IssuedTo == userGuid.ToString() && b.ReturnDate == null)
                        .ToListAsync();

                    var bookIdsToKeep = allExistingBooks
                        .Where(b => validBookIds.Contains(b.BookId))
                        .Select(b => b.BookId)
                        .ToList();

                    var bookIdsToAdd = validBookIds
                        .Where(id => !allExistingBooks.Any(b => b.BookId == id))
                        .ToList();

                    var bookIdsToRemove = allExistingBooks
                        .Where(b => !validBookIds.Contains(b.BookId))
                        .Select(b => b.BookId)
                        .ToList();

                    foreach (var bookId in bookIdsToRemove)
                    {
                        var book = await _db.Books.FindAsync(bookId);
                        if (book != null)
                        {
                            book.AvailableCopies++;
                            _db.Books.Update(book);
                        }

                        var issuedBookToRemove = allExistingBooks.FirstOrDefault(b => b.BookId == bookId);
                        if (issuedBookToRemove != null)
                        {
                            _db.IssuedBooks.Remove(issuedBookToRemove);
                        }
                    }

                    foreach (var bookId in bookIdsToAdd)
                    {
                        var book = await _db.Books.FindAsync(bookId);
                        if (book == null || book.AvailableCopies <= 0)
                        {
                            ModelState.AddModelError("", $"Book is not available.");
                            return View(firstBook);
                        }

                        var newIssuedBook = new IssuedBook
                        {
                            BookId = bookId,
                            IssuedTo = user.Id,
                            UserFullName = user.UserName,
                            UserType = UserType,
                            Class = UserType == "Student" ? Class : "",
                            Section = UserType == "Student" ? Section : "",
                            RollNumber = UserType == "Student" ? RollNumber : null,
                            IssueDate = DateOnly.FromDateTime(DateTime.Now),
                            Fine = 0,
                            ReturnDate = null
                        };

                        _db.IssuedBooks.Add(newIssuedBook);

                        book.AvailableCopies--;
                        _db.Books.Update(book);
                    }

                    foreach (var bookId in bookIdsToKeep)
                    {
                        var existingBook = allExistingBooks.FirstOrDefault(b => b.BookId == bookId);
                        if (existingBook != null)
                        {
                            existingBook.UserFullName = user.UserName;
                            existingBook.UserType = UserType;
                            existingBook.Class = UserType == "Student" ? Class : "";
                            existingBook.Section = UserType == "Student" ? Section : "";
                            existingBook.RollNumber = UserType == "Student" ? RollNumber : null;
                            _db.IssuedBooks.Update(existingBook);
                        }
                    }

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                    return View(firstBook);
                }
            }

            return View(firstBook);
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
                if (DateOnly.FromDateTime(DateTime.Now) > dueDate)
                {
                    var daysLate = (DateOnly.FromDateTime(DateTime.Now).DayNumber - dueDate.DayNumber);
                    issuedBook.Fine = daysLate * 10; // 10 per day   ,,later i will do every 5 days after 
                }

            }

            return View(issuedBook);
        }

        // POST: IssuedBookController/Return/5
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int id)
        {
            try
            {
                var issuedBook = await _db.IssuedBooks
                    .Include(ib => ib.Book)
                                .Include(ib => ib.AppUser)
                    .FirstOrDefaultAsync(ib => ib.Id == id);

                if (issuedBook == null)
                {
                    return NotFound();
                }

                if (issuedBook.ReturnDate != null)
                {
                    ModelState.AddModelError("", "This book has already been returned.");
                    return View("Return", issuedBook);
                }

                var dueDate = issuedBook.IssueDate.AddDays(14);

                //if (DateTimeOffset.Now > dueDate)
                //{
                //    var daysLate = (DateTimeOffset.Now - dueDate).Days;

                //    issuedBook.Fine = daysLate * 10; 
                //}
                //  issuedBook.ReturnDate = DateTimeOffset.Now;

                if (DateOnly.FromDateTime(DateTime.Now) > dueDate)
                {
                    var daysLate = (DateOnly.FromDateTime(DateTime.Now).DayNumber - dueDate.DayNumber);

                    issuedBook.Fine = daysLate * 10;
                }
                issuedBook.ReturnDate = DateOnly.FromDateTime(DateTime.Now);


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

                var issue = await _db.IssuedBooks
                    .Include(ib => ib.Book)
                    .Include(ib => ib.AppUser)
                    .FirstOrDefaultAsync(ib => ib.Id == id);

                return View("Return", issue);
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

        //public async Task<IActionResult> ReturnedBooks()
        //{

        //    var returnedBooks = await _db.IssuedBooks
        //        .Include(i => i.Book)
        //        .Include(i => i.AppUser)
        //        .Where(i => i.ReturnDate != null)
        //        .OrderByDescending(i => i.ReturnDate)
        //        .ToListAsync();

        //    return View(returnedBooks);
        //}


        //private bool IssuedBookExists(int id)
        //{
        //    return _db.IssuedBooks.Any(e => e.Id == id);
        //}
    }
}