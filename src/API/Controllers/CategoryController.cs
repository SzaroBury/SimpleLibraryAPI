using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using SimpleLibrary.API.Requests.Categories;
using SimpleLibrary.API.Mappers;

namespace SimpleLibrary.API.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService categoryService;
    private readonly ILogger<CategoryController> logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        this.categoryService = categoryService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        string? search,
        string? parentCategory,
        int? page = null,
        int? pageSize = null)
    {
        logger.LogInformation("Search request received.");
        var result = await categoryService.SearchCategoriesAsync(search, parentCategory, page ?? 1, pageSize ?? 25);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        logger.LogInformation("Get request received.");
        var result = await categoryService.GetCategoryByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(PostCategoryRequest category)
    {
        logger.LogInformation("Post request received.");
        var command = category.ToCommand();
        var result = await categoryService.CreateCategoryAsync(command);
        return Ok(result);
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Patch(PatchCategoryRequest category)
    {
        logger.LogInformation("Patch request received.");
        var command = category.ToCommand();
        var result = await categoryService.UpdateCategoryAsync(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        logger.LogInformation("Delete request received.");
        await categoryService.DeleteCategoryAsync(id);
        return Ok("Object was sucesfully deleted from the datebase.");
    }
}
