using SimpleLibrary.API.Attributes;
using SimpleLibrary.Application.Repositories;
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
            return new JsonResult(categoryRepository.GetAllCategories());
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // GET api/<CategoryController>/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult Get(int id)
    {   
        try
        {
            return new JsonResult(categoryRepository.GetCategory(id));
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
            categoryRepository.CreateCategory(category);
            return StatusCode(200, "Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // PUT api/<CategoryController>/5
    [HttpPut("{id}")]
    [ApiKey( "Librarian", "Admin")]
    public IActionResult Put(int id, Category category)
    {
        try
        {
            category.Id = id;
            categoryRepository.UpdateCategory(category);
            return StatusCode(200, "Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // DELETE api/<CategoryController>/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public IActionResult Delete(int id)
    {
        try
        {
            categoryRepository.DeleteCategory(id);
            return StatusCode(200, "Object was sucesfully deleted from the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }
}
