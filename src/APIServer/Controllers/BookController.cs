using ApiServer.Attributes;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> logger;
        private readonly IBookRepository bookRepository;
        private readonly ICopyRepository copyRepository;
        private readonly IBorrowingRepository borrowingRepository;
        public BookController(ILogger<BookController> logger, IBookRepository bookRepository, ICopyRepository copyRepository, IBorrowingRepository borrowingRepository)
        {
            this.logger = logger;
            this.bookRepository = bookRepository;
            this.copyRepository = copyRepository;
            this.borrowingRepository = borrowingRepository;
        }

        [HttpGet("~/api/Books")]
        [ApiKey("ReadOnly", "Librarian", "Admin")]
        public IActionResult GetAll(
            [FromQuery] string search = "",
            [FromQuery] int isAvailable = -1,
            [FromQuery] string olderThan = "",
            [FromQuery] string newerThan = "",
            [FromQuery] int author = -1,
            [FromQuery] int category = -1)
        {
            try
            {
                logger.LogInformation("GetAll request received. ({String})", Request.QueryString);
                List<Book> books = bookRepository.GetBooks();
                List<Copy> copies = copyRepository.GetCopies();
                List<Borrowing> borrowings = borrowingRepository.GetBorrowings();
                List<Book> result = new();

                if (!string.IsNullOrEmpty(search))
                {
                    result.AddRange(books.Where(b =>
                       b.Title.ToLower().Contains(search.ToLower())
                       || b.Description.ToLower().Contains(search.ToLower())
                       || b.Tags.ToLower().Contains(search.ToLower())
                       || b.Language.ToString()!.ToLower().Contains(search.ToLower())
                       || b.Author!.FirstName.ToString().ToLower().Contains(search.ToLower())
                       || b.Author.LastName.ToString().ToLower().Contains(search.ToLower())
                       || b.Author.Description.ToString().ToLower().Contains(search.ToLower())
                       || b.Author.Tags.ToString().ToLower().Contains(search.ToLower())
                       || b.Category!.Name.ToString().ToLower().Contains(search.ToLower())
                       || b.Category.Description.ToString().ToLower().Contains(search.ToLower())
                       || b.Category.Tags.ToString().ToLower().Contains(search.ToLower())
                    ).ToList());
                    books = result;
                    result = new();
                }

                if (isAvailable != -1 && books.Count > 0)
                {
                    if (isAvailable == 0)
                    {
                        result.AddRange( //add books, that don't have copies available to borrow
                            books.Where(b => !copies.Exists(c => b.Id == c.BookId && !borrowings.Exists(br => c.Id == br.CopyId && br.ActualReturnDate == null))
                            ).ToList()
                        );
                    }
                    else if (isAvailable == 1)
                    {
                        result.AddRange( //add books, that have available copies to borrow
                            books.Where(b => copies.Exists(c => c.BookId == b.Id && !borrowings.Exists(br => br.CopyId == c.Id && br.ActualReturnDate == null))
                            ).ToList()
                        );
                    }
                    books = result;
                    result = new();
                }

                if (!string.IsNullOrEmpty(olderThan)
                    && DateTime.TryParse(olderThan, out DateTime olderThanDate)
                    && books.Count > 0)
                {
                    result.AddRange(books.Where(b => b.ReleaseDate <= olderThanDate).ToList());
                    books = result;
                    result = new();
                }

                if (!string.IsNullOrEmpty(newerThan)
                    && DateTime.TryParse(newerThan, out DateTime newerThanDate)
                    && books.Count > 0)
                {
                    result.AddRange(books.Where(b => b.ReleaseDate >= newerThanDate).ToList());
                    books = result;
                    result = new();
                }

                if (author != -1 && books.Count > 0)
                {
                    result.AddRange(books.Where(b => b.AuthorId == author).ToList());
                    books = result;
                    result = new();
                }

                if (category != -1 && books.Count > 0)
                {
                    result.AddRange(books.Where(b => b.CategoryId == category).ToList());
                    books = result;
                }

                return new JsonResult(books);
            }
            catch (Exception e)
            {
                logger.LogError($"{HttpContext.Connection.RemoteIpAddress} -> BookController.GetAll() -> {e.Message}");
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpGet("{id}")]
        [ApiKey("ReadOnly", "Librarian", "Admin")]
        public IActionResult Get(int id)
        {
            try
            {
                logger.LogInformation("Get request received.");
                Book book = bookRepository.GetBook(id);
                JsonResult result = new JsonResult(book) { StatusCode = 200 };
                return result;
            }
            catch (Exception e)
            {
                logger.LogError($"{HttpContext.Connection.RemoteIpAddress} -> BookController.Get(id: {id}) -> {e.Message}");
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpPost]
        [ApiKey("Librarian", "Admin")]
        public IActionResult Post(Book book)
        {
            try
            {
                logger.LogInformation("Post request received.");
                bookRepository.CreateBook(book);
                return StatusCode(200, "Object was sucesfully added to the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }

        }

        [HttpPut("{id}")]
        [ApiKey("Librarian", "Admin")]
        public IActionResult Put(int id, Book book)
        {
            try
            {
                logger.LogInformation("Put request received.");
                book.Id = id;
                bookRepository.UpdateBook(book);
                return StatusCode(200, "Object was sucesfully updated in the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ApiKey("Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                logger.LogInformation("Delete request received.");
                bookRepository.DeleteBook(id);
                return StatusCode(200, "Object was sucesfully deleted from the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }
    }
}
