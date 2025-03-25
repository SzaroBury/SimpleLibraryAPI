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
            return new JsonResult(borrowingRepository.GetAllBorrowings());
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // GET api/<BorrowingsController>/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult Get(int id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            return new JsonResult(borrowingRepository.GetBorrowing(id));
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
            return StatusCode(200, "Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // PUT api/<BorrowingsController>/5
    [HttpPut("{id}")]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Put(int id, Borrowing borrowing)
    { 
        try
        {
            logger.LogInformation("Put request received.");
            borrowing.Id = id;
            borrowingRepository.UpdateBorrowing(borrowing);
            return StatusCode(200, "Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // DELETE api/<BorrowingsController>/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public IActionResult Delete(int id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            borrowingRepository.DeleteBorrowing(id);
            return StatusCode(200, "Object was sucesfully deleted from the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }
}
