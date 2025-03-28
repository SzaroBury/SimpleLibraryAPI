using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services;

namespace SimpleLibrary.Tests.Application.Services;

public class TestAuthorService
{
    private readonly Dictionary<string, Guid> guids = DataInitializer.InitializeGuids();
    private readonly Mock<IRepository<Author>> mockAuthorRepository = new();
    private readonly AuthorService authorService;

    public TestAuthorService()
    {
        mockAuthorRepository = DataInitializer.InitializeAuthorRepository(guids);        
        authorService = new AuthorService(mockAuthorRepository.Object);
    }

    [Fact]
    public async Task GetAllAuthorsAsync_ReturnsAllAuthors()
    {
        var response = await authorService.GetAllAuthorsAsync();

        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Author>>(response);
        Assert.Equal(2, response.Count());
    }

    [Fact]
    public async Task GetAuthorByIdAsync_ExistingId_ReturnsAuthor()
    {
        // Act
        var author = await authorService.GetAuthorByIdAsync(Guid.NewGuid());

        // Assert
        Assert.NotNull(author);
        // Assert.Equal(0, author.Id);
        Assert.Equal("Firstname", author.FirstName);
        Assert.Equal("Lastname", author.LastName);
        Assert.Equal("Description", author.Description);
        Assert.Equal(new DateTime(2000, 1, 1), author.BornDate);
        Assert.Equal("Tag1, Tag2", author.Tags);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        //Arange
        Guid authorGuid = Guid.NewGuid();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            authorService.GetAuthorByIdAsync(authorGuid)
        );
        Assert.Equal($"Author with the given id ({authorGuid}) is not found in database.", exception.Message);
    }

    [Fact]
    public async Task CreateAuthorAsync_AddsAuthor_ReturnsCreatedAuthor()
    {
        var newAuthor = new AuthorPostDTO
        (
            "Jane", 
            "Doe", 
            "Jane Doe was cool", 
            "1951-12-31",
            ["sci-fi", "brutal"]
        );

        var createdAuthor = await authorService.CreateAuthorAsync(newAuthor);

        Assert.NotNull(createdAuthor);
        // Assert.Equal(2, createdAuthor.Id);
        Assert.Equal("Jane", createdAuthor.FirstName);
        Assert.Equal("Doe", createdAuthor.LastName);
        Assert.Equal("Jane Doe was cool", createdAuthor.Description);
        Assert.Equal(new DateTime(1951, 12, 31), createdAuthor.BornDate);
        Assert.Equal("sci-fi, brutal", createdAuthor.Tags);
    }

    [Fact]
    public async Task CreateAuthorAsync_EmptyFirstName_ThrowsArgumentException()
    {
        var newAuthor = new AuthorPostDTO
        (
            "", 
            "Doe", 
            "Jane Doe was cool", 
            "1951-12-31",
            ["sci-fi", "brutal"]
        );

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            authorService.CreateAuthorAsync(newAuthor)
        );

        Assert.Equal("The author's first name and last name cannot be left empty.", exception.Message);
    }

    [Fact]
    public async Task CreateAuthorAsync_EmptyLastName_ThrowsArgumentException()
    {
        var newAuthor = new AuthorPostDTO
        (
            "Jane", 
            "", 
            "Jane Doe was cool", 
            "1951-12-31",
            ["sci-fi", "brutal"]
        );

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            authorService.CreateAuthorAsync(newAuthor)
        );

        Assert.Equal("The author's first name and last name cannot be left empty.", exception.Message);
    }

    [Fact]
    public async Task CreateAuthorAsync_InvalidBornDateFormat_ThrowsFormatException()
    {
        var newAuthor = new AuthorPostDTO
        (
            "Jane", 
            "Doe", 
            "Jane Doe was cool", 
            "testing",
            ["sci-fi", "brutal"]
        );

        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            authorService.CreateAuthorAsync(newAuthor)
        );

        Assert.Equal("Invalid date format. Please use the following format: YYYY-MM-DD", exception.Message);
    }

    [Fact]
    public async Task CreateAuthorAsync_CommaInTag_ThrowsFormatException()
    {
        var newAuthor = new AuthorPostDTO
        (
            "Jane", 
            "Doe", 
            "Jane Doe was cool", 
            "1951-12-31",
            ["sci-fi, novels", "brutal"]
        );

        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            authorService.CreateAuthorAsync(newAuthor)
        );

        Assert.Equal("Invalid tags format. Please do not use commas in tags.", exception.Message);
    }

    [Fact]
    public async Task UpdateAuthorAsync_ChangingNamesForExistingId_UpdatesNotNullMembersOfAuthor()
    {
        var updatedAuthor = new Author { FirstName = "Updated", LastName = "Name" };

        var result = await authorService.UpdateAuthorAsync(updatedAuthor);

        Assert.NotNull(result);
        // Assert.Equal(2, result.Id);
        Assert.Equal("Updated", result.FirstName);
        Assert.Equal("Name", result.LastName);
        Assert.Equal("Description", result.Description);
        Assert.Equal(new DateTime(2000, 01, 01), result.BornDate);
        Assert.Equal("Tag1, Tag2", result.Tags);
    }

    [Fact]
    public async Task UpdateAuthorAsync_ChangingBirthDateForExistingId_UpdatesNotNullMembersOfAuthor()
    {
        var updatedAuthor = new Author { BornDate = new DateTime(1949, 12, 31) };

        var result = await authorService.UpdateAuthorAsync(updatedAuthor);

        Assert.NotNull(result);
        // Assert.Equal(0, result.Id);
        Assert.Equal("Firstname", result.FirstName);
        Assert.Equal("Lastname", result.LastName);
        Assert.Equal("Description", result.Description);
        Assert.Equal(new DateTime(1949, 12, 31), result.BornDate);
        Assert.Equal("Tag1, Tag2", result.Tags);
    }

    [Fact]
    public async Task UpdateAuthorAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        var updatedAuthor = new Author { FirstName = "Ghost", LastName = "Writer" };

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            authorService.UpdateAuthorAsync(updatedAuthor)
        );

        Assert.Equal("Author with ID 99 not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteAuthorAsync_ExistingId_DeletesAuthor()
    {
        var authorsGuid = Guid.NewGuid();

        await authorService.DeleteAuthorAsync(authorsGuid.ToString());

        mockAuthorRepository.Verify(repo => repo.DeleteAsync(authorsGuid), Times.Once);
    }

    [Fact]
    public async Task DeleteAuthorAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        var authorsGuid = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            authorService.DeleteAuthorAsync(authorsGuid.ToString())
        );

        Assert.Equal($"Author with ID {authorsGuid} not found.", exception.Message);
    }

    [Fact]
    public async Task SearchAuthorsAsync_NoFilters_ReturnsAllAuthors()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync();

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.NotEmpty(response);
        Assert.Contains(response, a => a.FirstName == "Firstname" && a.LastName == "Lastname");
        Assert.Contains(response, a => a.FirstName == "John" && a.LastName == "Doe");
    }

    [Fact]
    public async Task SearchAuthorsAsync_FilteringWithJohn_ReturnsOnlyJohn()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync("John");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.NotEmpty(response);
        Assert.Single(response);
        Assert.Contains(response, a => a.FirstName == "John" && a.LastName == "Doe");
    }

    [Fact]
    public async Task SearchAuthorsAsync_FilteringWithTags_ReturnsOnlyJohn()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync("fantasy");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.NotEmpty(response);
        Assert.Single(response);
        Assert.Contains(response, a => a.FirstName == "John" && a.LastName == "Doe");
    }

    [Fact]
    public async Task SearchAuthorsAsync_FilteringWithUnknownTerm_ReturnsEmptyList()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync("term");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task SearchAuthorsAsync_FilteringByOlderThan_ReturnsOnlyJohn()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(olderThan: "1990-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Single(response);
        Assert.Contains(response, a => a.FirstName == "John" && a.LastName == "Doe");
    }

    [Fact]
    public async Task SearchAuthorsAsync_FilteringByOlderThanWithEqualDate_ReturnsEmptyList()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(olderThan: "1950-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task SearchAuthorsAsync_FilteringByOlderThanWithNoOneMeetingTheCondition_ReturnsEmptyList()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(olderThan: "1920-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task SearchAuthorsAsync_FilteringByYoungerThan_ReturnsOnlyFirstname()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(youngerThan: "1990-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Single(response);
        Assert.Contains(response, a => a.FirstName == "Firstname" && a.LastName == "Lastname");
    }

    [Fact]
    public async Task SearchAuthorsAsync_FilteringByYoungerThanWithEqualDate_ReturnsEmptyList()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(youngerThan: "2000-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task SearchAuthorsAsync_FilteringByYoungerThanWithNoOneMeetingTheCondition_ReturnsEmptyList()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(youngerThan: "2020-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task SearchAuthorsAsync_FirstPageOnePerPage_ReturnsOnlyFirstname()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(page: 1, pageSize: 1);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Single(response);
        Assert.Contains(response, a => a.FirstName == "Firstname" && a.LastName == "Lastname");
    }

    [Fact]
    public async Task SearchAuthorsAsync_FirstPageTwoPerPage_ReturnsBothAuthors()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(page: 1, pageSize: 2);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.NotEmpty(response);
        Assert.Equal(2, response.ToList().Count);
        Assert.Contains(response, a => a.FirstName == "Firstname" && a.LastName == "Lastname");
        Assert.Contains(response, a => a.FirstName == "John" && a.LastName == "Doe");
    }

    [Fact]
    public async Task SearchAuthorsAsync_SecondPageOnePerPage_ReturnsOnlyJohn()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(page: 2, pageSize: 1);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Single(response);
        Assert.Contains(response, a => a.FirstName == "John" && a.LastName == "Doe");
    }

    [Fact]
    public async Task SearchAuthorsAsync_SecondPageTwoPerPage_ReturnsOnlyJohn()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(page: 2, pageSize: 2);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Single(response);
        Assert.Contains(response, a => a.FirstName == "John" && a.LastName == "Doe");
    }

    [Fact]
    public async Task SearchAuthorsAsync_ThirdPageTwoPerPage_ReturnsEmptyList()
    {
        //Act
        var response = await authorService.SearchAuthorsAsync(page: 3, pageSize: 2);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Author>>(response);
        Assert.Empty(response);
    }
}