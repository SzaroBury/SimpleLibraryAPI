using ApiServer.Attributes;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorRepository authorRepository;
        private readonly ILogger<AuthorController> logger;
        public AuthorController(IAuthorRepository authorRepository, ILogger<AuthorController> logger)
        {
            this.authorRepository = authorRepository;
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
                return new JsonResult(authorRepository.GetAuthors());
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        // GET api/author/5
        [HttpGet("{id}")]
        [ApiKey("ReadOnly", "Librarian", "Admin")]
        public IActionResult Get(int id)
        {
            try
            {
                logger.LogInformation("Get request received.");
                return new JsonResult(authorRepository.GetAuthor(id));
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
                return StatusCode(200, "Object was sucesfully added to the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        // PUT api/authors
        [HttpPut("{id}")]
        [ApiKey("Librarian", "Admin")]
        public IActionResult Put(int id, Author author)
        {
            try
            {
                logger.LogInformation("Put request received.");
                author.Id = id;
                authorRepository.UpdateAuthor(author);
                return StatusCode(200, "Object was sucesfully updated in the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        // DELETE api/authors/5
        [HttpDelete("{id}")]
        [ApiKey("Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                logger.LogInformation("Delete request received.");
                authorRepository.DeleteAuthor(id);
                return StatusCode(200, "Object was sucesfully deleted from the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }
    }
}
