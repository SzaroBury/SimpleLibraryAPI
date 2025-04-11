using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Application.Services;
using SimpleLibrary.Domain.Repositories;

namespace SimpleLibrary.Tests.Application.Services;

public class TestAuthorService
{
    private readonly Dictionary<string, Guid> guids = DataInitializer.InitializeGuids();
    private readonly IUnitOfWork unitOfWork;
    private readonly Mock<IRepository<Author>> mockAuthorRepository;
    private readonly AuthorService authorService;

    public TestAuthorService()
    {
        mockAuthorRepository = DataInitializer.InitializeAuthorRepository(guids);
        unitOfWork = DataInitializer.InitializeUnitOfWorkAsync(guids, mockAuthorRepository).GetAwaiter().GetResult().Object;
        authorService = new AuthorService(unitOfWork);
    }
    #region GetAllAuthorsAsync
    [Fact]
    public async Task GetAllAuthorsAsync_ReturnsAllAuthors()
    {
        var response = await authorService.GetAllAuthorsAsync();

        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Author>>(response);
        Assert.Equal(4, response.Count());
    }
    #endregion
    #region GetAuthorByIdAsync
    [Fact]
    public async Task GetAuthorByIdAsync_ExistingId_ReturnsAuthor()
    {
        // Act
        var author = await authorService.GetAuthorByIdAsync(guids["a2"].ToString());

        // Assert
        Assert.NotNull(author);
        // Assert.Equal(0, author.Id);
        Assert.Equal("Adam", author.FirstName);
        Assert.Equal("Mickiewicz", author.LastName);
        Assert.Equal("", author.Description);
        Assert.Equal(new DateTime(1798, 12, 24), author.BornDate);
        Assert.Equal("", author.Tags);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_InvalidFormatOfId_ThrowsFormatException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            authorService.GetAuthorByIdAsync("test")
        );
        Assert.Equal($"Invalid author ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
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
        Assert.Equal($"An author with the specified ID ({authorGuid}) was not found in the system.", exception.Message);
    }
    #endregion
    #region CreateAuthorAsync 
    [Theory]
    [InlineData("William", "Shakespeare", "English playwright", "1564-04-23", new[] { "Dramatist", "Poet" }, "William", "Shakespeare", "English playwright", "1564-04-23", "Dramatist,Poet")]
    [InlineData("William", "Shakespeare", null, "1564-04-23", new[] { "Dramatist" }, "William", "Shakespeare", "", "1564-04-23", "Dramatist")]
    [InlineData("William", "Shakespeare", "English playwright", null, new[] { "Dramatist" }, "William", "Shakespeare", "English playwright", null, "Dramatist")]
    [InlineData("William", "Shakespeare", "English playwright", "1564-04-23", new[] { "Dramatist", "Poet", "Playwright" }, "William", "Shakespeare", "English playwright", "1564-04-23", "Dramatist,Poet,Playwright")]
    [InlineData("William", "Shakespeare", "English playwright", "1564-04-23", null, "William", "Shakespeare", "English playwright", "1564-04-23", "")]
    public async Task CreateAuthorAsync_ValidInput_ReturnsExpectedResult(
        string firstName, string lastName, string? description, string? bornDate, string[]? tags,
        string expectedFirstName, string expectedLastName, string expectedDescription, string? expectedBornDate, string expectedTags)
    {
        // Arrange
        var newAuthor = new AuthorPostDTO(firstName, lastName, description, bornDate, tags);

        // Act
        var result = await authorService.CreateAuthorAsync(newAuthor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedFirstName, result.FirstName);
        Assert.Equal(expectedLastName, result.LastName);
        Assert.Equal(expectedDescription, result.Description);
        
        if (expectedBornDate == null)
        {
            Assert.Null(result.BornDate);
        }
        else
        {
            Assert.Equal(DateTime.Parse(expectedBornDate), result.BornDate);
        }

        Assert.Equal(expectedTags, result.Tags);
    }

    [Theory]
    [InlineData("", "Shakespeare")]
    [InlineData("William", "")]
    [InlineData("", "")]
    public async Task CreateAuthorAsync_EmptyFirstOrLastName_ThrowsArgumentException(string firstName, string lastName)
    {
        var newAuthor = new AuthorPostDTO(firstName, lastName, "English playwright", "1564-04-23", ["Dramatist"]);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => authorService.CreateAuthorAsync(newAuthor));

        Assert.Equal("The author's first name and last name cannot be left empty.", exception.Message);
    }

    [Fact]
    public async Task CreateAuthorAsync_InvalidBornDate_ThrowsFormatException()
    {
        var newAuthor = new AuthorPostDTO("William", "Shakespeare", "English playwright", "invalid-date", ["Dramatist"]);

        var exception = await Assert.ThrowsAsync<FormatException>(() => authorService.CreateAuthorAsync(newAuthor));

        Assert.Equal("Invalid date format. Please use the following format: YYYY-MM-DD", exception.Message);
    }

    [Fact]
    public async Task CreateAuthorAsync_InvalidTags_ThrowsFormatException()
    {
        var newAuthor = new AuthorPostDTO("William", "Shakespeare", "English playwright", "1564-04-23", ["Dramatist,Playwright"]);

        var exception = await Assert.ThrowsAsync<FormatException>(() => authorService.CreateAuthorAsync(newAuthor));

        Assert.Equal("Invalid tags format. Please do not use commas in tags.", exception.Message);
    }
    #endregion
    #region UpdateAuthorAsync 
    [Theory]
    [InlineData("Updated", "Name", "New description", "2000-01-01", new[] { "Tag1", "Tag2" }, "Updated", "Name", "New description", "2000-01-01", "Tag1,Tag2")]
    [InlineData("Updated FirstName", null, null, null, null, "Updated FirstName", "Mickiewicz", "", "1798-12-24", "")]
    [InlineData(null, "Updated LastName", null, null, null, "Adam", "Updated LastName", "", "1798-12-24", "")]
    [InlineData(null, null, "Updated Description", null, null, "Adam", "Mickiewicz", "Updated Description", "1798-12-24", "")]
    [InlineData(null, null, null, "1800-01-01", null, "Adam", "Mickiewicz", "", "1800-01-01", "")]
    [InlineData(null, null, null, null, new[] { "Poet", "Romanticism" }, "Adam", "Mickiewicz", "", "1798-12-24", "Poet,Romanticism")]
    [InlineData("Updated Name", "Updated LastName", null, null, new[] { "Poet", "Romantic" }, "Updated Name", "Updated LastName", "", "1798-12-24", "Poet,Romantic")]
    public async Task UpdateAuthorAsync_ValidInput_UpdatesAuthorCorrectly(
        string? firstName, string? lastName, string? description, string? bornDate, string[]? tags,
        string expectedFirstName, string expectedLastName, string expectedDescription, string expectedBornDate, string expectedTags)
    {
        // Arrange
        var authorId = guids["a2"].ToString();
        var updatedAuthor = new AuthorPutDTO(authorId, firstName, lastName, description, bornDate, tags);

        // Act
        var result = await authorService.UpdateAuthorAsync(updatedAuthor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(guids["a2"], result.Id);
        Assert.Equal(expectedFirstName, result.FirstName);
        Assert.Equal(expectedLastName, result.LastName);
        Assert.Equal(expectedDescription, result.Description);
        
        if (expectedBornDate == null)
        {
            Assert.Null(result.BornDate);
        }
        else
        {
            Assert.Equal(DateTime.Parse(expectedBornDate), result.BornDate);
        }

        Assert.Equal(expectedTags, result.Tags);
    }

    [Fact]
    public async Task UpdateAuthorAsync_InvalidIdFormat_ThrowsFormatException()
    {
        var invalidAuthorId = "invalid-id-format";
        var updatedAuthor = new AuthorPutDTO(invalidAuthorId, FirstName: "Updated Name");

        var exception = await Assert.ThrowsAsync<FormatException>(() => authorService.UpdateAuthorAsync(updatedAuthor));

        Assert.Equal("Invalid author ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }

    [Fact]
    public async Task UpdateAuthorAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        var nonExistingAuthorId = Guid.NewGuid().ToString(); // Losowy, nieistniejący ID
        var updatedAuthor = new AuthorPutDTO(nonExistingAuthorId, FirstName: "Updated Name");

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => authorService.UpdateAuthorAsync(updatedAuthor));

        Assert.Equal($"An author with the specified ID ({nonExistingAuthorId}) was not found in the system.", exception.Message);
    }


    [Fact]
    public async Task UpdateAuthorAsync_EmptyFirstName_ThrowsArgumentException()
    {
        var authorId = guids["a2"].ToString();
        var updatedAuthor = new AuthorPutDTO(authorId, FirstName: " ");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => authorService.UpdateAuthorAsync(updatedAuthor));

        Assert.Equal("The author's first name cannot be left empty.", exception.Message);
    }


    [Fact]
    public async Task UpdateAuthorAsync_EmptyLastName_ThrowsArgumentException()
    {
        var authorId = guids["a2"].ToString();
        var updatedAuthor = new AuthorPutDTO(authorId, LastName: " ");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => authorService.UpdateAuthorAsync(updatedAuthor));

        Assert.Equal("The author's last name cannot be left empty.", exception.Message);
    }


    [Fact]
    public async Task UpdateAuthorAsync_InvalidBornDate_ThrowsFormatException()
    {
        var authorId = guids["a2"].ToString();
        var updatedAuthor = new AuthorPutDTO(authorId, BornDate: "invalid-date");

        var exception = await Assert.ThrowsAsync<FormatException>(() => authorService.UpdateAuthorAsync(updatedAuthor));

        Assert.Equal("Invalid date format. Please use the following format: YYYY-MM-DD", exception.Message);
    }


    [Fact]
    public async Task UpdateAuthorAsync_InvalidTags_ThrowsFormatException()
    {
        var authorId = guids["a2"].ToString();
        var updatedTags = new List<string> { "Poet,Writer" }; // Nieprawidłowe, bo zawiera przecinek
        var updatedAuthor = new AuthorPutDTO(authorId, Tags: updatedTags);

        var exception = await Assert.ThrowsAsync<FormatException>(() => authorService.UpdateAuthorAsync(updatedAuthor));

        Assert.Equal("Invalid tags format. Please do not use commas in tags.", exception.Message);
    }    
    #endregion
    #region DeleteAuthorAsync
    [Fact]
    public async Task DeleteAuthorAsync_ValidAuthorId_DeletesAuthorSuccessfully()
    {
        var authorsGuid = guids["a4"];

        await authorService.DeleteAuthorAsync(authorsGuid.ToString());

        mockAuthorRepository.Verify(repo => repo.DeleteAsync(authorsGuid), Times.Once);
    }
    
    [Fact]
    public async Task DeleteAuthorAsync_NonExistentAuthor_ThrowsKeyNotFoundException()
    {
        var authorsGuid = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            authorService.DeleteAuthorAsync(authorsGuid.ToString())
        );

        Assert.Equal($"An author with the specified ID ({authorsGuid}) was not found in the system.", exception.Message);
    }

    [Fact]
    public async Task DeleteAuthorAsync_AuthorWithBooks_ThrowsInvalidOperationException()
    {
        var authorsGuid = guids["a2"].ToString();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            authorService.DeleteAuthorAsync(authorsGuid)
        );

        Assert.Equal($"The author cannot be deleted as there are still books associated with this author in the system: Dziady część II, Dziady część III.", exception.Message);
    }
    #endregion
    #region SearchAuthorsAsync
    [Theory]
    [InlineData("", new[] { "N/A", "Adam", "Jan", "Some random" })]
    [InlineData("Adam", new[] { "Adam" })] 
    [InlineData("Kowalski", new[] { "Jan" })] 
    [InlineData("fantasy", new[] { "N/A" })] 
    [InlineData("unknown_term", new string[] { })]
    public async Task SearchAuthorsAsync_SearchTerm_ReturnsExpectedResults(string searchTerm, string[] expectedFirstNames)
    {
        // Act
        var response = await authorService.SearchAuthorsAsync(searchTerm: searchTerm);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Author>>(response);
        Assert.Equal(expectedFirstNames.Length, response.Count());
        foreach (var expectedName in expectedFirstNames)
        {
            Assert.Contains(response, a => a.FirstName == expectedName);
        }
    }

    [Theory]
    [InlineData("1950-01-01", new[] { "Jan", "Some random" })]
    [InlineData("2000-01-01", new string[] { })]
    [InlineData("2020-01-01", new string[] { })]
    public async Task SearchAuthorsAsync_FilteringByYoungerThan_ReturnsExpectedResults(string youngerThan, string[] expectedFirstNames)
    {
        // Act
        var response = await authorService.SearchAuthorsAsync(youngerThan: youngerThan);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Author>>(response);
        Assert.Equal(expectedFirstNames.Length, response.Count());
        foreach (var expectedName in expectedFirstNames)
        {
            Assert.Contains(response, a => a.FirstName == expectedName);
        }
    }

    [Theory]
    [InlineData(1, 1, new[] { "N/A" })] // Pierwsza strona, jeden element na stronie – zwraca pierwszego autora
    [InlineData(1, 2, new[] { "N/A", "Adam" })] // Pierwsza strona, dwa elementy – zwraca dwóch autorów
    [InlineData(2, 1, new[] { "Adam" })] // Druga strona, jeden element na stronie – zwraca Johna
    [InlineData(2, 2, new[] { "Jan", "Some random" })] // Druga strona, dwa elementy – zwraca tylko Johna
    [InlineData(4, 1, new string[] { "Some random" })] // Trzecia strona, dwa elementy – pusta lista
    public async Task SearchAuthorsAsync_Pagination_ReturnsExpectedResults(int page, int pageSize, string[] expectedFirstNames)
    {
        // Act
        var response = await authorService.SearchAuthorsAsync(page: page, pageSize: pageSize);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Author>>(response);
        Assert.Equal(expectedFirstNames.Length, response.Count());
        foreach (var expectedName in expectedFirstNames)
        {
            Assert.Contains(response, a => a.FirstName == expectedName);
        }
    }

    [Fact]
    public async Task SearchAuthorsAsync_SearchingTwoLetters_ThrowsArgumentException()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await authorService.SearchAuthorsAsync(searchTerm: "ts"));

        Assert.Equal($"The searching term need to have at least three letters.", exception.Message);
    }

    [Fact]
    public async Task SearchAuthorsAsync_PageLessThanOne_ThrowsArgumentException()
    {
        var page = 0;

        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await authorService.SearchAuthorsAsync(page: page));

        Assert.Equal($"Page ({page}) must be greater than zero.", exception.Message);
    }

    [Fact]
    public async Task SearchAuthorsAsync_PageSizeLessThanOne_ThrowsArgumentException()
    {
        var pageSize = 0;

        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await authorService.SearchAuthorsAsync(pageSize: pageSize));

        Assert.Equal($"Size of a page ({pageSize}) must be greater than zero.", exception.Message);
    }

    [Fact]
    public async Task SearchAuthorsAsync_InvalidOlderThanDate_ThrowsFormatException()
    {
        await Assert.ThrowsAsync<FormatException>(async () =>
            await authorService.SearchAuthorsAsync(olderThan: "invalid-date"));
    }

    [Fact]
    public async Task SearchAuthorsAsync_InvalidYoungerThanDate_ThrowsFormatException()
    {
        await Assert.ThrowsAsync<FormatException>(async () =>
            await authorService.SearchAuthorsAsync(youngerThan: "invalid-date"));
    }

    [Fact]
    public async Task SearchAuthorsAsync_PageOutOfRange_ThrowsInvalidOperationException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await authorService.SearchAuthorsAsync(page: 100, pageSize: 1));
    }
    #endregion
}