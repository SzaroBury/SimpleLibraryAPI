using SimpleLibrary.API.Attributes;
using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorController : ControllerBase
{
    private readonly IAuthorRepository authorRepository;
    private readonly IAuthorService authorService;
    private readonly ILogger<AuthorController> logger;
    public AuthorController(IAuthorRepository authorRepository, IAuthorService authorService, ILogger<AuthorController> logger)
    {
        this.authorRepository = authorRepository;
        this.authorService = authorService;
        this.logger = logger;
    }

    // GET: api/authors
    [HttpGet("~/api/Authors")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult GetAll()
    {
        try
        {
            logger.LogInformation("GetAll request received.");
            var result = authorRepository.GetAllAuthors();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // GET api/author/5
    [HttpGet("{id}")]
    [ApiKey("ReadOnly", "Librarian", "Admin")]
    public IActionResult Get(string id)
    {
        try
        {
            logger.LogInformation("Get request received.");
            var result = authorRepository.GetAuthor(Guid.Parse(id));
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // POST api/authors
    [HttpPost]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Post(Author author)
    {
        try
        {
            logger.LogInformation("Post request received.");
            authorRepository.CreateAuthor(author);
            return Ok("Object was sucesfully added to the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // PUT api/authors
    [HttpPut("{id}")]
    [ApiKey("Librarian", "Admin")]
    public IActionResult Put(string id, Author author)
    {
        try
        {
            logger.LogInformation("Put request received.");
            author.Id = Guid.Parse(id);
            authorRepository.UpdateAuthor(author);
            return Ok("Object was sucesfully updated in the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    // DELETE api/authors/5
    [HttpDelete("{id}")]
    [ApiKey("Admin")]
    public IActionResult Delete(string id)
    {
        try
        {
            logger.LogInformation("Delete request received.");
            authorRepository.DeleteAuthor(Guid.Parse(id));
            return StatusCode(200, "Object was sucesfully deleted from the datebase.");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error: {e.Message}");
        }
    }
}
