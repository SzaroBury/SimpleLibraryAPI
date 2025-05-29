using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using SimpleLibrary.API.Requests.Authors;
using SimpleLibrary.API.Mappers;

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

    [HttpGet]
    public async Task<IActionResult> Search(
        string? search = null, 
        string? olderThan = null, 
        string? youngerThan = null, 
        int? page = null, 
        int? pageSize = null)
    {
        logger.LogInformation("Search request received.");
        var result = await authorService.SearchAuthorsAsync(search, olderThan, youngerThan, page ?? 1, pageSize ?? 25);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        logger.LogInformation("Get request received.");
        var result = await authorService.GetAuthorByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(PostAuthorRequest author)
    {
        logger.LogInformation("Post request received.");
        var command = author.ToCommand();
        await authorService.CreateAuthorAsync(command);
        return Ok("Object was sucesfully added to the datebase.");
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Patch(PatchAuthorRequest author)
    {
        logger.LogInformation("Patch request received.");
        var command = author.ToCommand();
        await authorService.UpdateAuthorAsync(command);
        return Ok("Object was sucesfully updated in the datebase.");
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        logger.LogInformation("Delete request received.");
        await authorService.DeleteAuthorAsync(id);
        return Ok("Object was sucesfully deleted from the datebase.");
    }
}
