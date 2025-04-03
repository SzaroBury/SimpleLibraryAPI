using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibrary.API.Controllers;

[Route("api/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IRepository<Category> categoryRepository;
    private readonly ILogger<CategoryController> logger;
    public CategoryController(IRepository<Category> categoryRepository, ILogger<CategoryController> logger)
    {
        this.categoryRepository = categoryRepository;
        this.logger = logger;
    }

    // GET: api/<CategoryController>
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    [HttpGet("/api/categories")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = await categoryRepository.GetAllAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking GetAll():");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    // GET api/<CategoryController>/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> Get(string id)
    {   
        try
        {
            logger.LogInformation("Get request received.");
            if(!Guid.TryParse(id, out var categoryGuid))
            {
                throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
            }
            var result = await categoryRepository.GetByIdAsync(categoryGuid)
                ?? throw new KeyNotFoundException($"A category record with the specified ID ({id}) could not be found in the system.");
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

    // POST api/<CategoryController>
    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Post(Category category)
    {
        try
        {
            logger.LogInformation("Post request received.");
            await categoryRepository.AddAsync(category);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<Category Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    // PUT api/<CategoryController>/5
    [HttpPut("{id}")]
    [ApiKey( "Librarian", "Admin")]
    public async Task<IActionResult> Put(string id, Category category)
    {
        try
        {
            logger.LogInformation("Put request received.");
            categoryRepository.Update(category);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Put(id: {id}, <Category Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    // DELETE api/<CategoryController>/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            if(!Guid.TryParse(id, out var categoryGuid))
            {
                throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
            }
            await categoryRepository.DeleteAsync(categoryGuid);
            return Ok("Object was sucesfully deleted from the datebase.");
        }
        catch(FormatException e)
        {
            logger.LogInformation($"{DateTime.Now}: FormatException catched during invoking Delete(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Delete():");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }
}
