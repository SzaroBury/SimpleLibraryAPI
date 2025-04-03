using SimpleLibrary.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.DTO;

namespace SimpleLibrary.API.Controllers;

[Route("api/readers")]
[ApiController]
public class ReaderController : ControllerBase
{
    private readonly IReaderService readerService;
    private readonly ILogger<ReaderController> logger;
    
    public ReaderController(IReaderService readerService, ILogger<ReaderController> logger)
    {
        this.readerService = readerService;
        this.logger = logger;
    }

    [HttpGet]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> Search(
        [FromQuery] string? search, 
        [FromQuery] string? copy, 
        [FromQuery] bool? banned,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        try
        {
            logger.LogInformation("Search request received.");
            var result = await readerService.SearchReadersAsync(search, copy, banned, page ?? 1, pageSize ?? 25);
            return Ok(result);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"{DateTime.Now}: ArgumentException catched during invoking Search(search: {search}, copy: {copy}, banned: {banned}, page: {page}, pageSize: {pageSize}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Search(search: {search}, copy: {copy}, banned: {banned}, page: {page}, pageSize: {pageSize}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"{DateTime.Now}: KeyNotFoundException catched during invoking Search(search: {search}, copy: {copy}, banned: {banned}, page: {page}, pageSize: {pageSize}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"{DateTime.Now}: InvalidOperationException catched during invoking Search(search: {search}, copy: {copy}, banned: {banned}, page: {page}, pageSize: {pageSize}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Search(search: {search}, copy: {copy}, banned: {banned}, page: {page}, pageSize: {pageSize}):");
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
            var result = await readerService.GetReaderByIdAsync(id);
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
    public async Task<IActionResult> Post(ReaderPostDTO reader)
    {
        try
        {
            logger.LogInformation("Post request received.");
            var result = await readerService.CreateReaderAsync(reader);
            return Ok(result);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"{DateTime.Now}: ArgumentException catched during invoking Post(<ReaderPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Post(<ReaderPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"{DateTime.Now}: InvalidOperationException catched during invoking Post(<ReaderPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<ReaderPostDTO Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpPatch]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Patch(ReaderPatchDTO reader)
    {            
        try
        {
            logger.LogInformation("Patch request received.");
            var result = await readerService.UpdateReaderAsync(reader);
            return Ok(result);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Patch(<ReaderPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"{DateTime.Now}: KeyNotFoundException catched during invoking Patch(<ReaderPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Patch(<ReaderPatchDTO Object>):");
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
            await readerService.DeleteReaderAsync(id);
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
