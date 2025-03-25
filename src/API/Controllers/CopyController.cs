using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibrary.API.Controllers;

[Route("api/copy")]
[ApiController]
public class CopyController : ControllerBase
{
    private readonly IRepository<Copy> copyRepository;
    private readonly ILogger<CopyController> logger;
    public CopyController(IRepository<Copy> copyRepository, ILogger<CopyController> logger)
    {
        this.copyRepository = copyRepository;
        this.logger = logger;
    }

    // GET: api/<CopiesController>
    [HttpGet("/api/copies")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = await copyRepository.GetAllAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking GetAll():");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    // GET api/<CopiesController>/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            if(!Guid.TryParse(id, out var copyGuid))
            {
                throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
            }
            var result = await copyRepository.GetByIdAsync(copyGuid)
                ?? throw new KeyNotFoundException($"A copy record with the specified ID ({id}) could not be found in the system.");
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

    // POST api/<CopyController>
    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Post(Copy copy)
    {
        try
        {
            logger.LogInformation("Post request received.");
            await copyRepository.AddAsync(copy);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<Copy Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    // PUT api/<CopiesController>/5
    [HttpPut("{id}")]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Put(string id, Copy copy)
    {
        try
        {
            logger.LogInformation("Put request received.");
            await copyRepository.UpdateAsync(copy);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Put(id: {id}, <Copy Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    // DELETE api/<CopiesController>/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            if(!Guid.TryParse(id, out var copyGuid))
            {
                throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
            }
            await copyRepository.DeleteAsync(copyGuid);
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
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Delete(id: {id}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }
}
