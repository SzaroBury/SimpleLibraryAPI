using ApiServer.Controllers;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiServer.Tests
{
    public class TestAuthorController
    {
        [Fact]
        public void GetAll_OnSuccess_List()
        {
            //Arrange 
            Mock<IAuthorRepository> mockAuthorRepository = new Mock<IAuthorRepository>();
            Mock<ILogger<AuthorController>> mockAuthorLogger = new Mock<ILogger<AuthorController>>();

            mockAuthorRepository.Setup(service => service.GetAuthors()).Returns(new List<Author>()
            {
                new() { Id = 0, FirstName = "Imiê", LastName = "Nazwisko" }
            });

            AuthorController authorController = new AuthorController(mockAuthorRepository.Object, mockAuthorLogger.Object);

            //Act
            var response = authorController.GetAll() as JsonResult;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<JsonResult>(response);
            Assert.IsType<List<Author>>(response!.Value);
            Assert.NotEmpty(response.Value as IEnumerable<Author>);
            Assert.Contains(response.Value as IEnumerable<Author>, a => a.Id == 0 && a.FirstName == "Imiê" && a.LastName == "Nazwisko");
        }        

        [Fact]
        public void GetAll_OnFailure_Returns500()
        {
            //Arrange 
            Mock<IAuthorRepository> mockAuthorRepository = new Mock<IAuthorRepository>();
            Mock<ILogger<AuthorController>> mockAuthorLogger = new Mock<ILogger<AuthorController>>();

            mockAuthorRepository.Setup(service => service.GetAuthors()).Throws<InvalidOperationException>();

            AuthorController authorController = new AuthorController(mockAuthorRepository.Object, mockAuthorLogger.Object);

            //Act
            var response = authorController.GetAll() as ObjectResult;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, response!.StatusCode);
            Assert.NotNull(response.Value);
        }
    }
}