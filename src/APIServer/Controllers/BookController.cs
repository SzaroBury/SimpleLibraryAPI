using ApiServer.Attributes;
using Core.Services.Abstraction;
using Entities.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> logger;
        private readonly IBookService bookService;

        public BookController(ILogger<BookController> logger, IBookService bookService)
        {
            this.logger = logger;
            this.bookService = bookService;
        }

        [HttpGet("~/api/Books")]
        [ApiKey("ReadOnly", "Librarian", "Admin")]
        public IActionResult GetAll(
            [FromQuery] string search = "",
            [FromQuery] bool? isAvailable = null,
            [FromQuery] string olderThan = "",
            [FromQuery] string newerThan = "",
            [FromQuery] int author = -1,
            [FromQuery] int category = -1,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
        {
            try
            {
                logger.LogInformation("GetAll request received. ({String})", Request.QueryString);

                var result = bookService.SearchBooks(search, isAvailable, olderThan, newerThan, author, category, page, pageSize);

                return Ok(result);
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
                
                return Ok(bookService.GetBookById(id));
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                logger.LogError($"{HttpContext.Connection.RemoteIpAddress} -> BookController.Get(id: {id}) -> {e.Message}");
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpPost]
        [ApiKey("Librarian", "Admin")]
        public IActionResult Post(BookPostDTO book)
        {
            try
            {
                logger.LogInformation("Post request received.");
                bookService.CreateBook(book);
                return StatusCode(200, "Object was sucesfully added to the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }

        }

        [HttpPut]
        [ApiKey("Librarian", "Admin")]
        public IActionResult Update(BookPutDTO book)
        {
            try
            {
                logger.LogInformation("Update request received.");
                var result = bookService.UpdateBook(book);
                return Ok(result);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
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
                bookService.DeleteBook(id);
                return Ok();
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }
    }
}
