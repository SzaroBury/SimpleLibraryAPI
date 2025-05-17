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
        [FromQuery] string? search,
        [FromQuery] string? parentCategory,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        try
        {
            logger.LogInformation("Search request received.");
            var result = await categoryService.SearchCategoriesAsync(search, parentCategory, page ?? 1, pageSize ?? 25);
            return Ok(result);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Search(search: {search}, parentCategroy: {parentCategory}, page: {page}, pageSize: {pageSize}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"{DateTime.Now}: KeyNotFoundException catched during invoking Search(search: {search}, parentCategroy: {parentCategory}, page: {page}, pageSize: {pageSize}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"{DateTime.Now}: ArgumentException catched during invoking Search(search: {search}, parentCategroy: {parentCategory}, page: {page}, pageSize: {pageSize}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"{DateTime.Now}: InvalidOperationException catched during invoking Search(search: {search}, parentCategroy: {parentCategory}, page: {page}, pageSize: {pageSize}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Search(search: {search}, parentCategroy: {parentCategory}, page: {page}, pageSize: {pageSize}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {   
        try
        {
            logger.LogInformation("Get request received.");
            var result = await categoryService.GetCategoryByIdAsync(id);
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
    [Authorize]
    public async Task<IActionResult> Post(PostCategoryRequest category)
    {
        try
        {
            logger.LogInformation("Post request received.");
            var command = CategoryMapper.ToCommand(category);
            var result = await categoryService.CreateCategoryAsync(command);
            return Ok(result);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Post(<CategoryPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"{DateTime.Now}: KeyNotFoundException catched during invoking Post(<CategoryPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"{DateTime.Now}: ArgumentException catched during invoking Post(<CategoryPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"{DateTime.Now}: InvalidOperationException catched during invoking Post(<CategoryPostDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<CategoryPostDTO Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Patch(PatchCategoryRequest category)
    {
        try
        {
            logger.LogInformation("Patch request received.");
            var command = CategoryMapper.ToCommand(category);
            var result = await categoryService.UpdateCategoryAsync(command);
            return Ok(result);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Patch(<CategoryPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"{DateTime.Now}: KeyNotFoundException catched during invoking Patch(<CategoryPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(ArgumentException e)
        {
            logger.LogInformation($"{DateTime.Now}: ArgumentException catched during invoking Patch(<CategoryPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"{DateTime.Now}: InvalidOperationException catched during invoking Patch(<CategoryPatchDTO Object>):");
            logger.LogInformation($"    {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Patch(<CategoryPatchDTO Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            await categoryService.DeleteCategoryAsync(id);
            return Ok("Object was sucesfully deleted from the datebase.");
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Delete(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(InvalidOperationException e)
        {
            logger.LogInformation($"{DateTime.Now}: InvalidOperationException catched during invoking Delete(id: {id}):");
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
