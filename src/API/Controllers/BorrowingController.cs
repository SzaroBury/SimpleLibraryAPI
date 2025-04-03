using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibrary.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BorrowingController : ControllerBase
{
    private readonly IRepository<Borrowing> borrowingRepository;
    private readonly ILogger<BorrowingController> logger;

    public BorrowingController(IRepository<Borrowing> borrowingRepository, ILogger<BorrowingController> logger)
    {
        this.borrowingRepository = borrowingRepository;
        this.logger = logger;
    }

    // GET: api/<BorrowingsController>
    [HttpGet("~/api/Borrowings")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = await borrowingRepository.GetAllAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking GetAll(): {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }

    // GET api/<BorrowingsController>/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            if(!Guid.TryParse(id, out var borrowingGuid))
            {
                throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
            }
            var result = await borrowingRepository.GetByIdAsync(borrowingGuid)
                ?? throw new KeyNotFoundException($"A borrowing record with the specified ID ({id}) could not be found in the system.");
            return Ok(result);
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Get(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch(KeyNotFoundException e)
        {
            logger.LogInformation($"KeyNotFoundException catched during invoking Get(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Get(id: {id}):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error.");
        }
    }

    // POST api/<BorrowingsController>
    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Post(Borrowing Borrowing)
    {
        try
        {
            logger.LogInformation("Post request received.");
            await borrowingRepository.AddAsync(Borrowing);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Post(<Borrowing Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Unexpected error: {e.Message}");
        }
    }

    // PUT api/<BorrowingsController>/5
    [HttpPut("{id}")]
    [ApiKey("Librarian", "Admin")]
    public async Task<IActionResult> Put(string id, Borrowing borrowing)
    { 
        try
        {
            logger.LogInformation("Put request received.");
            borrowingRepository.Update(borrowing);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Put(id: {id}, <Borrowing Object>):");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // DELETE api/<BorrowingsController>/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            if(!Guid.TryParse(id, out var borrowingGuid))
            {
                throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
            }
            await borrowingRepository.DeleteAsync(borrowingGuid);
            return Ok("Object was sucesfully deleted from the datebase.");
        }
        catch(FormatException e)
        {
            logger.LogInformation($"FormatException catched during invoking Delete(id: {id}):");
            logger.LogInformation($"    {e.Message}");
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Delete(id: {id}:");
            logger.LogError($"    {e.Message}");
            return StatusCode(500, $"Error: {e.Message}");
        }
    }
}
