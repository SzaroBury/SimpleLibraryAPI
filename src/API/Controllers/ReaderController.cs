using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibrary.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReaderController : ControllerBase
{
    private readonly IReaderRepository readerRepository;
    private readonly ILogger<ReaderController> logger;
    public ReaderController(IReaderRepository readerRepository, ILogger<ReaderController> logger)
    {
        this.readerRepository = readerRepository;
        this.logger = logger;
    }

    [HttpGet("~/api/Readers")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult GetAll()
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = readerRepository.GetReaders();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // GET api/Reader/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            var result = readerRepository.GetReader(Guid.Parse(id));
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // POST api/Reader
    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Post(Reader reader)
    {
        try
        {
            logger.LogInformation("Post request received.");
            readerRepository.CreateReader(reader);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // PUT api/Reader/5
    [HttpPut("{id}")]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Put(string id, Reader reader)
    {            
        try
        {
            logger.LogInformation("Put request received.");
            reader.Id = Guid.Parse(id);
            readerRepository.UpdateReader(reader);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // DELETE api/Reader/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public IActionResult Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            readerRepository.DeleteReader(Guid.Parse(id));
            return Ok("Object was sucesfully deleted from the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }
}
