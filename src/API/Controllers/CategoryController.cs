using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibrary.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository categoryRepository;
    private readonly ILogger<CategoryController> logger;
    public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
    {
        this.categoryRepository = categoryRepository;
        this.logger = logger;
    }

    // GET: api/<CategoryController>
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    [HttpGet("~/api/Categories")]
    public IActionResult GetAll()
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = categoryRepository.GetAllCategories();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // GET api/<CategoryController>/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult Get(string id)
    {   
        try
        {
            logger.LogInformation("Get request received.");
            var result = categoryRepository.GetCategory(Guid.Parse(id));
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // POST api/<CategoryController>
    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Post(Category category)
    {
        try
        {
            logger.LogInformation("Post request received.");
            categoryRepository.CreateCategory(category);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // PUT api/<CategoryController>/5
    [HttpPut("{id}")]
    [ApiKey( "Librarian", "Admin")]
    public IActionResult Put(string id, Category category)
    {
        try
        {
            logger.LogInformation("Put request received.");
            category.Id = Guid.Parse(id);
            categoryRepository.UpdateCategory(category);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // DELETE api/<CategoryController>/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public IActionResult Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            categoryRepository.DeleteCategory(Guid.Parse(id));
            return Ok("Object was sucesfully deleted from the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }
}
