using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services;
using Moq;

namespace SimpleLibrary.Application.Services.Tests;

public class TestAuthorService
{
    Mock<IRepository<Author>> mockAuthorRepository = new();

    public TestAuthorService()
    {
        mockAuthorRepository
        .Setup(repo => repo.GetAllAsync())
        .Returns(Task.FromResult(new List<Author>()
        {
            new() { Id = 0, FirstName = "Firstname", LastName = "Lastname" }
        }));
    }

    [Fact]
    public void SearchAuthors_NoFilters_ReturnsAllAuthors()
    {
        //Arrange 
        AuthorService authorService = new AuthorService(mockAuthorRepository.Object);

        //Act
        var response = authorService.SearchAuthorsAsync();

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.NotEmpty(response);
        Assert.Contains(response, a => a.Id == 0 && a.FirstName == "Firstname" && a.LastName == "Lastname");
    }        
}