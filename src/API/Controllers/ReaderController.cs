using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using SimpleLibrary.API.Requests.Readers;
using SimpleLibrary.API.Mappers;

namespace SimpleLibrary.API.Controllers;

[Route("api/readers")]
[ApiController]
[Authorize]
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
    public async Task<IActionResult> Search(
        string? search, 
        string? copy, 
        bool? banned,
        int? page,
        int? pageSize)
    {
        logger.LogInformation("Search request received.");
        var result = await readerService.SearchReadersAsync(search, copy, banned, page ?? 1, pageSize ?? 25);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        logger.LogInformation("Get request received.");
        var result = await readerService.GetReaderByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Librarian")]
    public async Task<IActionResult> Post(PostReaderRequest reader)
    {
        logger.LogInformation("Post request received.");
        var command = reader.ToCommand();
        var result = await readerService.CreateReaderAsync(command);
        return Ok(result);
    }

    [HttpPatch]
    [Authorize(Roles = "Librarian")]
    public async Task<IActionResult> Patch(PatchReaderRequest reader)
    {
        logger.LogInformation("Patch request received.");
        var command = reader.ToCommand();
        var result = await readerService.UpdateReaderAsync(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        logger.LogInformation("Delete request received.");
        await readerService.DeleteReaderAsync(id);
        return Ok("Object was sucesfully deleted from the datebase.");
    }
}
