using SimpleLibrary.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.API.Controllers;

[Route("api/copies")]
[ApiController]
public class CopyController : ControllerBase
{
    private readonly ICopyService copyService;
    private readonly ILogger<CopyController> logger;
    
    public CopyController(ICopyService copyService, ILogger<CopyController> logger)
    {
        this.copyService = copyService;
        this.logger = logger;
    }

    [HttpGet]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> Search(
        [FromQuery] string? search,
        [FromQuery] string? book,
        [FromQuery] bool? isAvailable,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        try
        {
            logger.LogInformation("Search request received.");
            var result = await copyService.SearchCopiesAsync(search, book, isAvailable, page ?? 1, pageSize ?? 25);
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking GetAll():");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            var result = await copyService.GetCopyByIdAsync(id);
            return Ok(result);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Get(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"{DateTime.Now}: KeyNotFoundException catched during invoking Get(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Get(id: {id}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Post(CopyPostDTO copy)
    {
        try
        {
            logger.LogInformation("Post request received.");
            var result = await copyService.CreateCopyAsync(copy);
            return Ok(copy);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Post(<CopyPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"{DateTime.Now}: KeyNotFoundException catched during invoking Post(<CopyPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"{DateTime.Now}: ArguumentException catched during invoking Post(<CopyPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<CopyPostDTO Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpPatch]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Patch(CopyPatchDTO copy)
    {
        try
        {
            logger.LogInformation("Put request received.");
            var result = await copyService.UpdateCopyAsync(copy);
            return Ok(copy);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Patch(<CopyPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"{DateTime.Now}: KeyNotFoundException catched during invoking Post(<CopyPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"{DateTime.Now}: ArguumentException catched during invoking Post(<CopyPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Patch(<CopyPatchDTO Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            await copyService.DeleteCopyAsync(id);
            return Ok("Object was sucesfully deleted from the datebase.");
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Delete(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"{DateTime.Now}: KeyNotFoundException catched during invoking Delete(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Delete(id: {id}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }
}
