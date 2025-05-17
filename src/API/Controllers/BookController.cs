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
        [FromQuery] string search = "",
        [FromQuery] bool? isAvailable = null,
        [FromQuery] string olderThan = "",
        [FromQuery] string newerThan = "",
        [FromQuery] string author = "",
        [FromQuery] string category = "",
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        try
        {
            logger.LogInformation($"Search ({Request.QueryString})");

            var result = await bookService.SearchBooksAsync(search, isAvailable, olderThan, newerThan, author, category, page ?? 1, pageSize ?? 25);

            return Ok(result);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"ArgumentException catched during invoking Search(search: {search}, olderThan: {olderThan}, newerThan: {newerThan}, author: {author}, category: {category}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Search(search: {search}, olderThan: {olderThan}, newerThan: {newerThan}, author: {author}, category: {category}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Search(search: {search}, olderThan: {olderThan}, newerThan: {newerThan}, author: {author}, category: {category}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            var result = await bookService.GetBookByIdAsync(id);
            return Ok(result);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Get(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Get(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Get(id: {id}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(PostBookRequest book)
    {
        try
        {
            logger.LogInformation("Post request received.");
            var command = BookMapper.ToCommand(book);
            await bookService.CreateBookAsync(command);
            return StatusCode(200, "Object was sucesfully added to the datebase.");
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"ArgumentException catched during invoking Post(<BookPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Post(<BookPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Post(<BookPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"InvalidOperationException catched during invoking Post(<BookPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<BookPostDTO Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Patch(PatchBookRequest book)
    {
        try
        {
            logger.LogInformation("Update request received.");
            var command = BookMapper.ToCommand(book);
            var result = await bookService.UpdateBookAsync(command);
            return Ok(result);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Patch(<BookPatchDTO Object, Id: {book.Id}>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Patch(<BookPatchDTO Object, Id: {book.Id}>):");
            logger.LogInformation($"    {e.Message}");
            return NotFound(e.Message);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"ArgumentException catched during invoking Patch(<BookPatchDTO Object, Id: {book.Id}>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }        
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"InvalidOperationException catched during invoking Patch(<BookPatchDTO Object, Id: {book.Id}>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Patch(<BookPatchDTO Object, Id: {book.Id}>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            await bookService.DeleteBookAsync(id);
            return Ok();
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Delete(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }        
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Delete(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Delete(id: {id}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }
}
