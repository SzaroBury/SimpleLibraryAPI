using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibrary.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CopyController : ControllerBase
{
    private readonly ICopyRepository copyRepository;
    private readonly ILogger<CopyController> logger;
    public CopyController(ICopyRepository copyRepository, ILogger<CopyController> logger)
    {
        this.copyRepository = copyRepository;
        this.logger = logger;
    }

    // GET: api/<CopiesController>
    [HttpGet("~/api/Copies")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult GetAll()
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = copyRepository.GetAllCopies();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // GET api/<CopiesController>/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            var result = copyRepository.GetCopy(Guid.Parse(id));
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // POST api/<CopyController>
    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Post(Copy copy)
    {
        try
        {
            logger.LogInformation("Post request received.");
            copyRepository.CreateCopy(copy);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // PUT api/<CopiesController>/5
    [HttpPut("{id}")]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Put(string id, Copy copy)
    {
        try
        {
            logger.LogInformation("Put request received.");
            copy.Id = Guid.Parse(id);
            copyRepository.UpdateCopy(copy);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // DELETE api/<CopiesController>/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public IActionResult Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            copyRepository.DeleteCopy(Guid.Parse(id));
            return Ok("Object was sucesfully deleted from the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }
}
