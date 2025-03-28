using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibrary.API.Controllers;

[Route("api/reader")]
[ApiController]
public class ReaderController : ControllerBase
{
    private readonly IRepository<Reader> readerRepository;
    private readonly ILogger<ReaderController> logger;
    public ReaderController(IRepository<Reader> readerRepository, ILogger<ReaderController> logger)
    {
        this.readerRepository = readerRepository;
        this.logger = logger;
    }

    [HttpGet("/api/readers")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = await readerRepository.GetAllAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking GetAll():");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    // GET api/Reader/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            if(!Guid.TryParse(id, out var readerGuid))
            {
                throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
            }
            var result = await readerRepository.GetByIdAsync(readerGuid)
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

    // POST api/Reader
    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Post(Reader reader)
    {
        try
        {
            logger.LogInformation("Post request received.");
            await readerRepository.AddAsync(reader);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<Reader Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    // PUT api/Reader/5
    [HttpPut("{id}")]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Put(string id, Reader reader)
    {            
        try
        {
            logger.LogInformation("Put request received.");
            await readerRepository.UpdateAsync(reader);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Put(id: {id}, <Reader Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, "Unexpected error.");
        }
    }

    // DELETE api/Reader/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            if(!Guid.TryParse(id, out var readerGuid))
            {
                throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
            }
            await readerRepository.DeleteAsync(readerGuid);
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
