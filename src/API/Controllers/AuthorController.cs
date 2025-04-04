using SimpleLibrary.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.DTO;

namespace SimpleLibrary.API.Controllers;

[Route("api/authors")]
[ApiController]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService authorService;
    private readonly ILogger<AuthorController> logger;
    public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger)
    {
        this.authorService = authorService;
        this.logger = logger;
    }

    [HttpGet("search")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null, 
        [FromQuery] string? olderThan = null, 
        [FromQuery] string? youngerThan = null, 
        [FromQuery] int? page = null, 
        [FromQuery] int? pageSize = null
    )
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = await authorService.SearchAuthorsAsync(search, olderThan, youngerThan, page ?? 1, pageSize ?? 25);
            return Ok(result);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"ArgumentException catched during invoking GetAll(search: {search}, olderThan: {olderThan}, youngerThan: {youngerThan}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking GetAll(search: {search}, olderThan: {olderThan}, youngerThan: {youngerThan}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking GetAll(search: {search}, olderThan: {olderThan}, youngerThan: {youngerThan}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }

    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            var result = await authorService.GetAuthorByIdAsync(id);
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
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Post(AuthorPostDTO author)
    {
        try
        {
            logger.LogInformation("Post request received.");
            await authorService.CreateAuthorAsync(author);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"ArgumentException catched during invoking Post(<AuthorPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Post(<AuthorPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<AuthorPostDTO Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }

    [HttpPut]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Put(AuthorPutDTO author)
    {
        try
        {
            logger.LogInformation("Put request received.");
            await authorService.UpdateAuthorAsync(author);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Put(<AuthorPutDTO Object, Id: {author.Id}>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Put(<AuthorPutDTO Object, Id: {author.Id}>):");
            logger.LogInformation($"    {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Put(<AuthorPutDTO Object - id: {author.Id}>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }

    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            await authorService.DeleteAuthorAsync(id);
            return Ok("Object was sucesfully deleted from the datebase.");
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
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Delete(id: {id}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }
}
