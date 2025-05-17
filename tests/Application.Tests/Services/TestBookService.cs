using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Application.Services;
using SimpleLibrary.Application.Commands.Books;

namespace SimpleLibrary.Tests.Application.Services;

public class TestBookService
{
    private readonly Dictionary<string, Guid> guids = DataInitializer.InitializeGuids();
    private readonly IUnitOfWork unitOfWork;
    private readonly Mock<IRepository<Book>> mockBookRepository;
    private readonly BookService bookService;

    public TestBookService()
    {   
        Mock<IRepository<Author>> mockAuthorRepository = DataInitializer.InitializeAuthorRepository(guids);
        Mock<IRepository<Category>> mockCategoryRepository = DataInitializer.InitializeCategories(guids);
        mockBookRepository = DataInitializer.InitializeBookRepositoryAsync(guids, mockAuthorRepository, mockCategoryRepository).GetAwaiter().GetResult();
        unitOfWork = DataInitializer.InitializeUnitOfWorkAsync(guids, mockBookRepository: mockBookRepository).GetAwaiter().GetResult().Object;

        bookService = new(unitOfWork);
    }

    #region SearchBooks
    [Theory]
    [InlineData(null, 6, new[] { "b1", "b2", "b3", "b4", "b5", "b6" })]
    [InlineData("Some", 4, new[] { "b1", "b2", "b3", "b6" })]
    [InlineData("Mickiewicz", 2, new[] { "b4", "b5" })]
    [InlineData("Novel", 3, new[] { "b1", "b2", "b6" })]
    [InlineData("Tomato", 0, new string[] { })]
    public async Task SearchBooks_BySearchTerm_ReturnsExpectedResults(string? searchTerm, int expectedCount, string[] expectedIds)
    {
        // Act
        var response = await bookService.SearchBooksAsync(searchTerm: searchTerm);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Book>>(response);
        Assert.Equal(expectedCount, response.ToList().Count);
        foreach (var expectedId in expectedIds)
        {
            Assert.Contains(response, b => b.Id == guids[expectedId]);
        }
    }

    [Theory]
    [InlineData(true, new[] { "b1", "b2", "b3", "b6" })]
    [InlineData(false, new[] { "b4", "b5" })]
    public async Task SearchBooks_ByAvailability_ReturnsExpectedResults(bool isAvailable, string[] expectedIds)
    {
        // Act
        var response = await bookService.SearchBooksAsync(isAvailable: isAvailable);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Book>>(response);
        Assert.Equal(expectedIds.Count(), response.Count());
        foreach (var expectedId in expectedIds)
        {
            Assert.Contains(response, b => b.Id == guids[expectedId]);
        }
    }
    [Fact]
    public async Task SearchBooks_OlderThan_ReturnsCertainBooks()
    {
        //Act
        var response = await bookService.SearchBooksAsync(olderThan: "1900-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(4, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b1"]);
        Assert.Contains(response, b => b.Id == guids["b2"]);
        Assert.Contains(response, b => b.Id == guids["b4"]);
        Assert.Contains(response, b => b.Id == guids["b5"]);
    }

    [Fact]
    public async Task SearchBooks_NewerThan_ReturnsCertainBooks()
    {
        //Act
        var response = await bookService.SearchBooksAsync(newerThan: "1900-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b1"]);
        Assert.Contains(response, b => b.Id == guids["b3"]);
        Assert.Contains(response, b => b.Id == guids["b6"]);
    }

    [Fact]
    public async Task SearchBooks_ByExistingAuthor_ReturnsCertainBooks()
    {
        //Act
        var response = await bookService.SearchBooksAsync(authorId: guids["a2"].ToString());

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(2, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b4"]);
        Assert.Contains(response, b => b.Id == guids["b5"]);
    }

    [Fact]
    public async Task SearchBooks_ByExistingCategory_ReturnsCertainBooks()
    {
        //Act
        var response = await bookService.SearchBooksAsync(categoryId: guids["c1"].ToString());

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b1"]);
        Assert.Contains(response, b => b.Id == guids["b2"]);
        Assert.Contains(response, b => b.Id == guids["b6"]);
    }

    [Theory]
    [InlineData(2, 2, 2, new[] { "b3", "b4" })]
    [InlineData(1, 3, 3, new[] { "b1", "b2", "b3" })]
    [InlineData(2, 3, 3, new[] { "b4", "b5", "b6" })]
    [InlineData(1, 5, 5, new[] { "b1", "b2", "b3", "b4", "b5" })]
    [InlineData(2, 5, 1, new[] { "b6" })]
    public async Task SearchBooks_Pagination_ReturnsExpectedResults(int page, int pageSize, int expectedCount, string[] expectedIds)
    {
        // Act
        var response = await bookService.SearchBooksAsync(page: page, pageSize: pageSize);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<List<Book>>(response);
        Assert.Equal(expectedCount, response.ToList().Count);
        foreach (var expectedId in expectedIds)
        {
            Assert.Contains(response, b => b.Id == guids[expectedId]);
        }
    }

    [Fact]
    public async Task SearchBooks_InvalidFormatOfOlderThan_ThrowsFormatException()
    {
        //Act && Assert
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(olderThan: "123"));
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(olderThan: "test"));
    }

    [Fact]
    public async Task SearchBooks_InvalidFormatOfNewerThan_ThrowsFormatException()
    {
        //Act && Assert
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(newerThan: "123"));
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(newerThan: "test"));
    }

    [Fact]
    public async Task SearchBooks_InvalidFormatOfAuthorId_ThrowsFormatException()
    {
        //Act && Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            bookService.SearchBooksAsync(authorId: "test")
        );
        Assert.Equal("Invalid author ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }

    [Fact]
    public async Task SearchBooks_NonExistingAuthor_ThrowsKeyNotFoundException()
    {
        //Arrange
        string nonExistingAuthorId = guids["c1"].ToString(); 

        //Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            bookService.SearchBooksAsync(authorId: nonExistingAuthorId)
        );
        Assert.Equal($"An author with the specified ID ({nonExistingAuthorId}) was not found in the system.", exception.Message);
    }

    [Fact]
    public async Task SearchBooks_InvalidFormatOfCategoryId_ThrowsFormatException()
    {
        //Act && Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            bookService.SearchBooksAsync(categoryId: "test")
        );
        Assert.Equal("Invalid category ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }

    [Fact]
    public async Task SearchBooks_ByNonExistingCategory_ThrowsKeyNotFoundException()
    {
        //Arrange
        string nonExistingCategoryId = guids["a1"].ToString(); 

        //Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            bookService.SearchBooksAsync(categoryId: nonExistingCategoryId)
        );
        Assert.Equal($"A category with the specified id ({nonExistingCategoryId}) was not found in the system.", exception.Message);
    }

    [Fact]
    public async Task SearchBooks_NonExistingPage_ThrowsInvalidOperationException()
    {
        //Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => bookService.SearchBooksAsync(page: 4, pageSize: 5));
    }

    [Fact]
    public async Task SearchBooks_NegativePageNumber_ThrowsArgumentOutOfRangeException()
    {
        //Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => bookService.SearchBooksAsync(page: -4));

    }

    [Fact]
    public async Task SearchBooks_NegativePageSize_ThrowsArgumentOutOfRangeException()
    {
        //Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => bookService.SearchBooksAsync(pageSize: -5));
    }
    #endregion

    #region GetBookById
    [Fact]
    public async Task GetBookById_ExistingBook_ReturnsBook()
    {
        //Arrange
        string bookId = guids["b1"].ToString();

        //Act
        var result = await bookService.GetBookByIdAsync(bookId);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(bookId, result.Id.ToString());
    }

    [Fact]
    public async Task GetBookById_InvalidFormatOfId_ThrowsFormatException()
    {
        //Act & Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            bookService.GetBookByIdAsync("test")
        );
        Assert.Equal("Invalid book ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }

    [Fact]
    public async Task GetBookById_NonExisitingBook_ThrowsKeyNotFoundException()
    {
        //Arrange
        string bookId = Guid.Empty.ToString();

        //Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            bookService.GetBookByIdAsync(bookId)
        );
        Assert.Equal($"A book with the specified id ({bookId}) was not found in the system.", exception.Message);
    }
    #endregion

    #region CreateBook
    [Fact]
    public async Task CreateBook_CorrectInput_CreatesBook()
    {
        var panTadeusz = new PostBookCommand(
            "Pan Tadeusz",
            "Full title: Sir Thaddeus, or the Last Foray in Lithuania: A Nobility's Tale of the Years 1811–1812, in Twelve Books of Verse.",
            "1834-06-28",
            "Polish",
            ["poem", "epic", "national", "compulsory reading", "unesco"],
            guids["a2"].ToString(),
            guids["c2"].ToString()
        );

        //Act
        var result = await bookService.CreateBookAsync(panTadeusz);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<Book>(result);
        Assert.IsType<Guid>(result.Id);
        mockBookRepository.Verify(repo => repo.AddAsync(result));
    }

    [Fact]
    public async Task CreateBook_EmptyTitle_ThrowsArgumentException()
    {
        var someUnknownBook = new PostBookCommand(
            "",
            "Desc of some unknown book",
            "1997-07-05",
            "Polish",
            ["novel"],
            guids["a2"].ToString(),
            guids["c2"].ToString()
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            bookService.CreateBookAsync(someUnknownBook)
        );
        Assert.Equal("Title cannot be empty.", exception.Message);
    }

    [Fact]
    public async Task CreateBook_InvalidDateFormat_ThrowsFormatException()
    {
        var someBook = new PostBookCommand(
            "Some book",
            "Desc of some book",
            "Hello World",
            "Polish",
            ["novel"],
            guids["a2"].ToString(),
            guids["c2"].ToString()
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            bookService.CreateBookAsync(someBook)
        );
        Assert.Equal("Invalid date format. Please use the following format: YYYY-MM-DD", exception.Message);
    }

    [Fact]
    public async Task CreateBook_InvalidLanguageFormat_ThrowsFormatException()
    {
            var someBook = new PostBookCommand(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Hello world",
            ["novel"],
            guids["a2"].ToString(),
            guids["c2"].ToString()
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            bookService.CreateBookAsync(someBook)
        );
        Assert.Equal("Invalid language format. Pick one of the following values: English, Polish, German, French, Spanish, Other.", exception.Message);
    }
    
    [Fact]
    public async Task CreateBook_InvalidFormatOfCategoryId_ThrowsFormatException()
    {
            var someBook = new PostBookCommand(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Polish",
            ["novel"],
            guids["a2"].ToString(),
            "test"
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            bookService.CreateBookAsync(someBook)
        );
        Assert.Equal("Invalid category ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }
    
    [Fact]
    public async Task CreateBook_NonExistingCategory_ThrowsKeyNotFoundException()
    {
        var nonExistingCategoryId = Guid.Empty.ToString();
        var someBook = new PostBookCommand(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Polish",
            ["novel"],
            guids["a2"].ToString(),
            nonExistingCategoryId
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            bookService.CreateBookAsync(someBook)
        );
        Assert.Equal($"A category with the specified ID ({nonExistingCategoryId}) was not found in the system.", exception.Message);
    }

    [Fact]
    public async Task CreateBook_InvalidFormatOfAuthorId_ThrowsFormatException()
    {
            var someBook = new PostBookCommand(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Polish",
            ["novel"],
            "test",
            guids["c2"].ToString()
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            bookService.CreateBookAsync(someBook)
        );
        Assert.Equal("Invalid author ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }
    [Fact]
    public async Task CreateBook_NonExistingAuthor_ThrowsKeyNotFoundException()
    {
        var someBook = new PostBookCommand(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Polish",
            ["novel"],
            guids["c1"].ToString(),
            guids["c2"].ToString()
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            bookService.CreateBookAsync(someBook)
        );
        Assert.Equal($"An author with the specified ID ({guids["c1"]}) was not found in the system.", exception.Message);
    }

    [Fact]
    public async Task CreateBook_InvalidFormatOfTags_ThrowsFormatException()
    {
        var someBook = new PostBookCommand(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Polish",
            ["novel,"],
            guids["a1"].ToString(),
            guids["c2"].ToString()
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            bookService.CreateBookAsync(someBook)
        );
        Assert.Equal("Invalid format of tags. Please do not use commas.", exception.Message);
    }

    [Fact]
    public async Task CreateBook_SimiliarToExisting_ThrowsInvalidOperationException()
    {
        var dziadyIII = new PostBookCommand ("Dziady część III", "", "1832-1-1", "Polish", ["tag", "another"], guids["a2"].ToString(), guids["c2"].ToString());

        //Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            bookService.CreateBookAsync(dziadyIII)
        );
        Assert.Equal("There is already a similar book in the system.", exception.Message);
    }
    #endregion

    #region UpdateBook
    [Fact]
    public async Task UpdateBook_CorrectInput_UpdatesBook()
    {
        var bookId = guids["b1"].ToString();
        var authorId = guids["a2"].ToString();
        var categoryId = guids["c2"].ToString();

        var someOlderBook = new PatchBookCommand(
            bookId,
            Title: "Some older book",
            Description: "Test description",
            ReleaseDate: "1991-02-02",
            Language: "Polish",
            Tags: ["test", "book"],
            authorId, 
            categoryId
        );

        //Act
        var result = await bookService.UpdateBookAsync(someOlderBook);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<Book>(result);
        Assert.Equal(bookId, result.Id.ToString());
        Assert.Equal("Some older book", result.Title);
        Assert.Equal("Test description", result.Description);
        Assert.Equal(new DateTime(1991, 02, 02), result.ReleaseDate);
        Assert.Equal(Language.Polish, result.Language);
        Assert.Equal("test,book", result.Tags);
        Assert.Equal(authorId, result.AuthorId.ToString());
        Assert.Equal(categoryId, result.CategoryId.ToString());
    }
    
    [Fact]
    public async Task UpdateBook_InvalidFormatOfBookId_ThrowsFormatException()
    {
        var someUnknownBook = new PatchBookCommand(
            Id: "test",
            Title: "Some book"
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => bookService.UpdateBookAsync(someUnknownBook));
        Assert.Equal("Invalid book ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }

    [Fact]
    public async Task UpdateBook_NonExistingBook_ThrowsKeyNotFoundException()
    {
        var nonExistingBookId = Guid.Empty.ToString();
        var someUnknownBook = new PatchBookCommand(
            Id: nonExistingBookId,
            Title: "Some book"
        );

        //Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => bookService.UpdateBookAsync(someUnknownBook));
        Assert.Equal($"A book with the specified id ({nonExistingBookId}) was not found in the system.", exception.Message);
    }

    [Fact]
    public async Task UpdateBook_EmptyTitle_ThrowsArgumentException()
    {
        var someUpdatedBook = new PatchBookCommand(
            Id: guids["b1"].ToString(),
            Title: ""
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Contains("Title can not be empty.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_InvalidDateFormat_ThrowsFormatException()
    {
        var someUpdatedBook = new PatchBookCommand(
            Id: guids["b1"].ToString(),
            ReleaseDate: "Hello world"
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Contains("Invalid date format.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_InvalidLanguageFormat_ThrowsFormatException()
    {
        var someUpdatedBook = new PatchBookCommand(
            Id: guids["b1"].ToString(),
            Language: "Simlish"
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Contains("Invalid language format.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_InvalidTagsFormat_ThrowsFormatException()
    {
        var someUpdatedBook = new PatchBookCommand(
            Id: guids["b1"].ToString(),
            Tags: ["novel,", "book"]
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Contains("Invalid tags format. Please do not use commas in tags.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_InvalidAuthorIdFormat_ThrowsFormatException()
    {
        var someUpdatedBook = new PatchBookCommand(
            Id: guids["b1"].ToString(),
            AuthorId: "test"
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Equal("Invalid author ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_NonExistingAuthor_ThrowsKeyNotFoundException()
    {
        var nonExistingAuthorId = Guid.Empty.ToString();
        var someUpdatedBook = new PatchBookCommand(
            Id: guids["b1"].ToString(),
            AuthorId: nonExistingAuthorId
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Equal($"An author with the specified ID ({nonExistingAuthorId}) was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_InvalidCategoryIdFormat_ThrowsFormatException()
    {
        var someUpdatedBook = new PatchBookCommand(
            Id: guids["b1"].ToString(),
            CategoryId: "test"
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Equal("Invalid category ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_NonExistingCategory_ThrowsArgumentException()
    {
        var nonExistingCategoryId = Guid.Empty.ToString();
        var someUpdatedBook = new PatchBookCommand(
            Id: guids["b1"].ToString(),
            CategoryId: nonExistingCategoryId
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Contains("Category with the given id is not present in the system.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_SimilarBookExisting_ThrowsInvalidOperationException()
    {
        var someUpdatedBook = new PatchBookCommand(
            Id: guids["b4"].ToString(),
            Title: "Dziady część III"
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Equal("There is already a similar book in the system.", ex.Message);
    }
    #endregion

    #region DeleteBook
    [Fact]
    public async Task DeleteBook_ExistingBookWithoutActiveBorrowings_DeletesBookAndCopies()
    {
        //Act
        await bookService.DeleteBookAsync(guids["b6"].ToString());

        //Assert
        mockBookRepository.Verify(r => r.DeleteAsync(guids["b6"]));
    }

    [Fact]
    public async Task DeleteBook_InvalidFormatOfBookId_ThrowsFormatException()
    {
        //Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() => bookService.DeleteBookAsync("test"));
        Assert.Equal("Invalid book ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task DeleteBook_NonExistingBook_ThrowsKeyNotFoundException()
    {
        //Arrange
        var bookId = guids["a1"].ToString();

        //Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => bookService.DeleteBookAsync(bookId));
        Assert.Equal($"A book with the specified id ({bookId}) was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task DeleteBook_ExistingBookWithActiveBorrowings_ThrowsInvalidOperationException()
    {
        //Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => bookService.DeleteBookAsync(guids["b2"].ToString()));
        Assert.Equal("The book can not be deleted. There are still active borrowings in the system.", ex.Message);
    }
    #endregion
}