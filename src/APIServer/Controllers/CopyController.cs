using ApiServer.Attributes;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using RepositoryEF.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServer.Controllers
{
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
                return new JsonResult(copyRepository.GetCopies());
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        // GET api/<CopiesController>/5
        [HttpGet("{id}")]
        [ApiKey("ReadOnly", "Librarian", "Admin")]
        public IActionResult Get(int id)
        {
            try
            {
                logger.LogInformation("Get request received.");
                return new JsonResult(copyRepository.GetCopy(id));
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
                return StatusCode(200, "Object was sucesfully added to the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        // PUT api/<CopiesController>/5
        [HttpPut("{id}")]
        [ApiKey("Librarian", "Admin")]
        public IActionResult Put(int id, Copy copy)
        {
            try
            {
                logger.LogInformation("Put request received.");
                copy.Id = id;
                copyRepository.UpdateCopy(copy);
                return StatusCode(200, "Object was sucesfully updated in the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        // DELETE api/<CopiesController>/5
        [HttpDelete("{id}")]
        [ApiKey("Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                logger.LogInformation("Delete request received.");
                copyRepository.DeleteCopy(id);
                return StatusCode(200, "Object was sucesfully deleted from the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }
    }
}
