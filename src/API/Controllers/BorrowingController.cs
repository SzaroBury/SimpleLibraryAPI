using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.DTO;

namespace SimpleLibrary.API.Controllers;

[Route("api/borrowings")]
[ApiController]
public class BorrowingController : ControllerBase
{
    private readonly IBorrowingService borrowingService;
    private readonly ILogger<BorrowingController> logger;

    public BorrowingController(IBorrowingService borrowingService, ILogger<BorrowingController> logger)
    {
        this.borrowingService = borrowingService;
        this.logger = logger;
    }

    [HttpGet]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> Search(        
        [FromQuery] string? search,
        [FromQuery] string? olderThan,
        [FromQuery] string? newerThan,
        [FromQuery] string? copy,
        [FromQuery] string? reader,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        try
        {
            logger.LogInformation("Search request received.");
            var result = await borrowingService.SearchBorrowingsAsync(search, olderThan, newerThan, copy, reader, page ?? 1, pageSize ?? 25);
            return Ok(result);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"ArgumentException catched during invoking Search(search: {search}, olderThan: {olderThan}, newerThan: {newerThan}, copy: {copy}, reader: {reader}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Search(search: {search}, olderThan: {olderThan}, newerThan: {newerThan}, copy: {copy}, reader: {reader}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Search(search: {search}, olderThan: {olderThan}, newerThan: {newerThan}, copy: {copy}, reader: {reader}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"InvalidOperationException catched during invoking Search(search: {search}, olderThan: {olderThan}, newerThan: {newerThan}, copy: {copy}, reader: {reader}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Search(search: {search}, olderThan: {olderThan}, newerThan: {newerThan}, copy: {copy}, reader: {reader}, page: {page ?? 1}, pageSize: {pageSize ?? 25}):");
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
            var result = await borrowingService.GetBorrowingByIdAsync(id);
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
    public async Task<IActionResult> Post(BorrowingPostDTO borrowing)
    {
        try
        {
            logger.LogInformation("Post request received.");
            var result = await borrowingService.CreateBorrowingAsync(borrowing);
            return Ok(result);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Post(<BorrowingPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Post(<BorrowingPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"InvalidOperationException catched during invoking Post(<BorrowingPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<BorrowingPostDTO Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error: {e.Message}");
        }
    }

    [HttpPatch]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Patch(BorrowingPatchDTO borrowing)
    { 
        try
        {
            logger.LogInformation("Patch request received.");
            var result = await borrowingService.UpdateBorrowingAsync(borrowing);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"ArgumentException catched during invoking Patch(<BorrowingPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Patch(<BorrowingPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Patch(<BorrowingPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"InvalidOperationException catched during invoking Patch(<BorrowingPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Patch(<BorrowingPatchDTO Object>):");
            logger.LogError($"    {e.Message}");
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
            await borrowingService.DeleteBorrowingAsync(id);
            return Ok("Object was sucesfully deleted from the datebase.");
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Delete(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return NotFound(e.Message);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Delete(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Delete(id: {id}:");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Error: {e.Message}");
        }
    }
}
