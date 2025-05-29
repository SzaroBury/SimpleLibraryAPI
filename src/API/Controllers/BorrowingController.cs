using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using SimpleLibrary.API.Requests.Borrowings;
using SimpleLibrary.API.Mappers;

namespace SimpleLibrary.API.Controllers;

[Route("api/borrowings")]
[ApiController]
[Authorize]
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
    public async Task<IActionResult> Search(        
        string? search,
        string? olderThan,
        string? newerThan,
        string? copy,
        string? reader,
        int? page = null,
        int? pageSize = null)
    {
        logger.LogInformation("Search request received.");
        var result = await borrowingService.SearchBorrowingsAsync(search, olderThan, newerThan, copy, reader, page ?? 1, pageSize ?? 25);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        logger.LogInformation("Get request received.");
        var result = await borrowingService.GetBorrowingByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(PostBorrowingRequest borrowing)
    {
        logger.LogInformation("Post request received.");
        var command = borrowing.ToCommand();
        var result = await borrowingService.CreateBorrowingAsync(command);
        return Ok(result);
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Patch(PatchBorrowingRequest borrowing)
    { 
        logger.LogInformation("Patch request received.");
        var command = borrowing.ToCommand();
        var result = await borrowingService.UpdateBorrowingAsync(command);
        return Ok("Object was sucesfully updated in the datebase.");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        logger.LogInformation("Delete request received.");
        await borrowingService.DeleteBorrowingAsync(id);
        return Ok("Object was sucesfully deleted from the datebase.");
    }
}
