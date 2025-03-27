using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibrary.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BorrowingController : ControllerBase
{
    private readonly IBorrowingRepository borrowingRepository;
    private readonly ILogger<BorrowingController> logger;
    public BorrowingController(IBorrowingRepository BorrowingRepository, ILogger<BorrowingController> logger)
    {
        this.borrowingRepository = BorrowingRepository;
        this.logger = logger;
    }

    // GET: api/<BorrowingsController>
    [HttpGet("~/api/Borrowings")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult GetAll()
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = borrowingRepository.GetAllBorrowings();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // GET api/<BorrowingsController>/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            var result = borrowingRepository.GetBorrowing(Guid.Parse(id));
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // POST api/<BorrowingsController>
    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Post(Borrowing Borrowing)
    {
        try
        {
            logger.LogInformation("Post request received.");
            borrowingRepository.CreateBorrowing(Borrowing);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // PUT api/<BorrowingsController>/5
    [HttpPut("{id}")]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Put(string id, Borrowing borrowing)
    { 
        try
        {
            logger.LogInformation("Put request received.");
            borrowing.Id = Guid.Parse(id);
            borrowingRepository.UpdateBorrowing(borrowing);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // DELETE api/<BorrowingsController>/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public IActionResult Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            borrowingRepository.DeleteBorrowing(Guid.Parse(id));
            return Ok("Object was sucesfully deleted from the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }
}
