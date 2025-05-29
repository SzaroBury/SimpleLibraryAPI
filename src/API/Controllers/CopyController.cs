using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using SimpleLibrary.API.Requests.Copies;
using SimpleLibrary.API.Mappers;

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
    public async Task<IActionResult> Search(
        string? search,
        string? book,
        bool? isAvailable,
        int? page,
        int? pageSize)
    {
        logger.LogInformation("Search request received.");
        var result = await copyService.SearchCopiesAsync(search, book, isAvailable, page ?? 1, pageSize ?? 25);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        logger.LogInformation("Get request received.");
        var result = await copyService.GetCopyByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(PostCopyRequest copy)
    {
        logger.LogInformation("Post request received.");
        var command = copy.ToCommand();
        var result = await copyService.CreateCopyAsync(command);
        return Ok(copy);
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Patch(PatchCopyRequest copy)
    {
        logger.LogInformation("Put request received.");
        var command = copy.ToCommand();
        var result = await copyService.UpdateCopyAsync(command);
        return Ok(copy);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        logger.LogInformation("Delete request received.");
        await copyService.DeleteCopyAsync(id);
        return Ok("Object was sucesfully deleted from the datebase.");
    }
}
