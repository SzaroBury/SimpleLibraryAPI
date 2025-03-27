using SimpleLibrary.API.Attributes;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibrary.API.Controllers;

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
    public async Task<IActionResult> GetAll(
        [FromQuery] string search = "",
        [FromQuery] bool? isAvailable = null,
        [FromQuery] string olderThan = "",
        [FromQuery] string newerThan = "",
        [FromQuery] string author = "",
        [FromQuery] string category = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        try
        {
            logger.LogInformation($"GetAll ({Request.QueryString})");

            var result = await bookService.SearchBooksAsync(search, isAvailable, olderThan, newerThan, author, category, page, pageSize);

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
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");

            var result = await bookService.GetBookByIdAsync(id);
            
            return Ok(result);
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
    public async Task<IActionResult> Post(BookPostDTO book)
    {
        try
        {
            logger.LogInformation("Post request received.");
            await bookService.CreateBookAsync(book);
            return StatusCode(200, "Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }

    }

    [HttpPut]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Update(BookPutDTO book)
    {
        try
        {
            logger.LogInformation("Update request received.");
            var result = await bookService.UpdateBookAsync(book);
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
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            await bookService.DeleteBookAsync(id);
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
