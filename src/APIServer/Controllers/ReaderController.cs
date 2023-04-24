using ApiServer.Attributes;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
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
                return new JsonResult(readerRepository.GetReaders());
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        // GET api/Reader/5
        [HttpGet("{id}")]
        [ApiKey("ReadOnly", "Librarian", "Admin")]
        public IActionResult Get(int id)
        {
            try
            {
                logger.LogInformation("Get request received.");
                return new JsonResult(readerRepository.GetReader(id));
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
                return StatusCode(200, "Object was sucesfully added to the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        // PUT api/Reader/5
        [HttpPut("{id}")]
        [ApiKey("Librarian", "Admin")]
        public IActionResult Put(int id, Reader reader)
        {            
            try
            {
                logger.LogInformation("Put request received.");
                reader.Id = id;
                readerRepository.UpdateReader(reader);
                return StatusCode(200, "Object was sucesfully updated in the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        // DELETE api/Reader/5
        [HttpDelete("{id}")]
        [ApiKey("Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                logger.LogInformation("Delete request received.");
                readerRepository.DeleteReader(id);
                return StatusCode(200, "Object was sucesfully deleted from the datebase.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }
    }
}
