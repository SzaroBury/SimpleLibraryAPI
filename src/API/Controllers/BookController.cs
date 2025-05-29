using SimpleLibrary.Application.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SimpleLibrary.API.Requests.Books;
using SimpleLibrary.API.Mappers;

namespace SimpleLibrary.API.Controllers;

[Route("api/books")]
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

    [HttpGet]
    public async Task<IActionResult> Search(
        string search = "",
        bool? isAvailable = null,
        string olderThan = "",
        string newerThan = "",
        string author = "",
        string category = "",
        int? page = null,
        int? pageSize = null)
    {
        logger.LogInformation($"Search ({Request.QueryString})");

        var result = await bookService.SearchBooksAsync(search, isAvailable, olderThan, newerThan, author, category, page ?? 1, pageSize ?? 25);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        logger.LogInformation("Get request received.");
        var result = await bookService.GetBookByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(PostBookRequest book)
    {
        logger.LogInformation("Post request received.");
        var command = book.ToCommand();
        await bookService.CreateBookAsync(command);
        return StatusCode(200, "Object was sucesfully added to the datebase.");
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Patch(PatchBookRequest book)
    {
        logger.LogInformation("Update request received.");
        var command = book.ToCommand();
        var result = await bookService.UpdateBookAsync(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        logger.LogInformation("Delete request received.");
        await bookService.DeleteBookAsync(id);
        return Ok();
    }
}
