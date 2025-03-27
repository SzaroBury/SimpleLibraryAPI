using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services.Abstraction;
using Moq;

namespace SimpleLibrary.Application.Services.Tests;

public class TestBookService
{
    private readonly Mock<IBookRepository> mockBookRepository = new();
    private readonly Mock<ICopyRepository> mockCopyRepository = new();
    private readonly Mock<IBorrowingRepository> mockBorrowingRepository = new();
    private readonly Mock<IAuthorService> mockAuthorService = new();
    private readonly Mock<ICategoryRepository> mockCategoryRepository = new();
    private readonly Mock<IReaderRepository> mockReaderRepository = new();
    private readonly Dictionary<string, Guid> guids = [];

    public TestBookService()
    {   
        InitializeGuids();
        InitializeAuthors();
        InitializeCategories();
        InitializeBooks().GetAwaiter().GetResult();
        InitializeCopies();
        InitializeReaders();
        InitializeBorrowings();
    }

    #region SearchBooks
    [Fact]
    public async Task SearchBooks_NoFilters_ReturnsAllBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync();

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(6, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b1"]);
        Assert.Contains(response, b => b.Id == guids["b2"]);
        Assert.Contains(response, b => b.Id == guids["b3"]);
        Assert.Contains(response, b => b.Id == guids["b4"]);
        Assert.Contains(response, b => b.Id == guids["b5"]);
        Assert.Contains(response, b => b.Id == guids["b6"]);
    }

    [Fact]
    public async Task SearchBooks_SearchSome_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(searchTerm: "Some");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(4, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b1"]);
        Assert.Contains(response, b => b.Id == guids["b2"]);
        Assert.Contains(response, b => b.Id == guids["b3"]);
        Assert.Contains(response, b => b.Id == guids["b6"]);
    }

    [Fact]
    public async Task SearchBooks_SearchMickiewicz_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(searchTerm: "Mickiewicz");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(2, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b4"]);
        Assert.Contains(response, b => b.Id == guids["b5"]);
    }

    [Fact]
    public async Task SearchBooks_SearchNovel_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(searchTerm: "Novel");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b1"]);
        Assert.Contains(response, b => b.Id == guids["b2"]);
        Assert.Contains(response, b => b.Id == guids["b6"]);
    }

    [Fact]
    public async Task SearchBooks_SearchTomato_ReturnsEmptyList()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(searchTerm: "Tomato");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task SearchBooks_OnlyAvailable_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(isAvailable: true);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(2, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b2"]);
        Assert.Contains(response, b => b.Id == guids["b3"]);
    }

    [Fact]
    public async Task SearchBooks_OnlyNotAvailable_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(isAvailable: false);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(4, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b1"]);
        Assert.Contains(response, b => b.Id == guids["b4"]);
        Assert.Contains(response, b => b.Id == guids["b5"]);
        Assert.Contains(response, b => b.Id == guids["b6"]);
    }

    [Fact]
    public async Task SearchBooks_OlderThan_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

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
    public async Task SearchBooks_InvalidFormatOfOlderThan_ThrowsFormatException()
    {
        //Arrange 
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act && Assert
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(olderThan: "123"));
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(olderThan: "test"));
    }

    [Fact]
    public async Task SearchBooks_NewerThan_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

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
    public async Task SearchBooks_InvalidFormatOfNewerThan_ThrowsFormatException()
    {
        //Arrange 
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act && Assert
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(newerThan: "123"));
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(newerThan: "test"));
    }

    [Fact]
    public async Task SearchBooks_ByExistingAuthor_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

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
    public async Task SearchBooks_ByNonExistingAuthor_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => bookService.SearchBooksAsync(authorId: Guid.Empty.ToString()));
    }

    [Fact]
    public async Task SearchBooks_ByExistingCategory_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

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

    [Fact]
    public async Task SearchBooks_ByNonExistingCategory_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => bookService.SearchBooksAsync(categoryId: Guid.Empty.ToString()));
    }

    [Fact]
    public async Task SearchBooks_PageTwoOfSizeTwo_ReturnsTwoBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(page: 2, pageSize: 2);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(2, response.ToList().Count);
        Assert.Contains(response, b => b.Id == guids["b3"]);
        Assert.Contains(response, b => b.Id == guids["b4"]);
    }

    [Fact]
    public async Task SearchBooks_PageOneOfSizeThree_ReturnsThreeBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(page: 1, pageSize: 3);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.ToList().Count);
    }

    [Fact]
    public async Task SearchBooks_PageTwoOfSizeThree_ReturnsThreeBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(page: 2, pageSize: 3);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.ToList().Count);
    }

    [Fact]
    public async Task SearchBooks_PageOneOfSizeFive_ReturnsFiveBooks()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(pageSize: 5);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(5, response.ToList().Count);
    }

    [Fact]
    public async Task SearchBooks_PageTwoOfSizeFive_ReturnsOneBook()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = await bookService.SearchBooksAsync(page: 2, pageSize: 5);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Single(response);
    }

    [Fact]
    public async Task SearchBooks_NonExistingPage_ThrowsInvalidOperationException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => bookService.SearchBooksAsync(page: 4, pageSize: 5));
    }

    [Fact]
    public async Task SearchBooks_NegativePageNumber_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => bookService.SearchBooksAsync(page: -4));

    }

    [Fact]
    public async Task SearchBooks_NegativePageSize_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => bookService.SearchBooksAsync(pageSize: -5));
    }
    #endregion

    #region GetBookById
    [Fact]
    public async Task GetBookById_ExistingBook_ReturnsBook()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        var result = await bookService.GetBookByIdAsync(guids["b1"].ToString());

        //Assert
        Assert.NotNull(result);
        Assert.Equal(guids["b1"], result.Id);
    }

    [Fact]
    public async Task GetBookById_NonExisitingBook_ThrowsKeyNotFoundException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => bookService.GetBookByIdAsync(Guid.Empty.ToString()));
    }

    [Fact]
    public async Task GetBookById_NegativeId_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => bookService.GetBookByIdAsync(Guid.Empty.ToString()));
    }
    #endregion

    #region CreateBook
    [Fact]
    public async Task CreateBook_CorrectInput_CreatesBook()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var panTadeusz = new BookPostDTO(
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
    }

    [Fact]
    public async Task CreateBook_EmptyTitle_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someUnknownBook = new BookPostDTO(
            "",
            "Desc of some unknown book",
            "1997-07-05",
            "Polish",
            ["novel"],
            guids["a2"].ToString(),
            guids["c2"].ToString()
        );

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => bookService.CreateBookAsync(someUnknownBook));
    }

    [Fact]
    public async Task CreateBook_InvalidDateFormat_ThrowsFormatException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someBook = new BookPostDTO(
            "Some book",
            "Desc of some book",
            "Hello World",
            "Polish",
            ["novel"],
            guids["a2"].ToString(),
            guids["c2"].ToString()
        );

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => bookService.CreateBookAsync(someBook));
    }

    [Fact]
    public async Task CreateBook_InvalidLanguageFormat_ThrowsFormatException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
            var someBook = new BookPostDTO(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Hello world",
            ["novel"],
            guids["a2"].ToString(),
            guids["c2"].ToString()
        );

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => bookService.CreateBookAsync(someBook));
    }

    [Fact]
    public async Task CreateBook_NonExistingAuthor_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someBook = new BookPostDTO(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Polish",
            ["novel"],
            Guid.Empty.ToString(),
            guids["c2"].ToString()
        );

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => bookService.CreateBookAsync(someBook));
    }

    [Fact]
    public async Task CreateBook_NonExistingCategory_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someBook = new BookPostDTO(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Polish",
            ["novel"],
            guids["a2"].ToString(),
            Guid.Empty.ToString()
        );

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => bookService.CreateBookAsync(someBook));
    }

    [Fact]
    public async Task CreateBook_SimiliarToExisting_ThrowsInvalidOperationException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var dziadyIII = new BookPostDTO ("Dziady część III", "", "1832-1-1", "Polish", ["tag", "another"], guids["a2"].ToString(), guids["c2"].ToString());

        //Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => bookService.CreateBookAsync(dziadyIII));
    }
    #endregion

    #region UpdateBook
    [Fact]
    public async Task UpdateBook_ChangedTitle_UpdatesBook()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someOlderBook = new BookPutDTO(
            Id: guids["b1"].ToString(),
            Title: "Some older book"
        );

        //Act
        var result = await bookService.UpdateBookAsync(someOlderBook);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<Book>(result);
    }

    [Fact]
    public async Task UpdateBook_NonExistingBook_ThrowsKeyNotFoundException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someUnknownBook = new BookPutDTO(
            Id: Guid.Empty.ToString(),
            Title: "Some book"
        );

        //Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => bookService.UpdateBookAsync(someUnknownBook));
    }

    [Fact]
    public async Task UpdateBook_EmptyTitle_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
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
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
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
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            Id: guids["b1"].ToString(),
            Language: "Simlish"
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Contains("Invalid language format.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_NonExistingAuthor_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            Id: guids["b1"].ToString(),
            AuthorId: Guid.Empty.ToString()
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Contains("Author with the given id is not present in the system.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_NonExistingCategory_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            Id: guids["b1"].ToString(),
            CategoryId: Guid.Empty.ToString()
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Contains("Category with the given id is not present in the system.", ex.Message);
    }

    [Fact]
    public async Task UpdateBook_SimilarBookExisting_ThrowsInvalidOperationException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            Id: guids["b4"].ToString(),
            Title: "Dziady część III"
        );

        //Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => bookService.UpdateBookAsync(someUpdatedBook));
        Assert.Contains("There is a similar book in the system.", ex.Message);
    }
    #endregion

    #region DeleteBook
    [Fact]
    public async Task DeleteBook_ExistingBookWithoutActiveBorrowings_DeletesBookAndCopies()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act
        await bookService.DeleteBookAsync(guids["b6"].ToString());

        //Assert
        mockBookRepository.Verify(r => r.DeleteBook(guids["b6"]));
    }

    [Fact]
    public async Task DeleteBook_ExistingBookWithActiveBorrowings_ThrowsInvalidOperation()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => bookService.DeleteBookAsync(guids["b2"].ToString()));
        Assert.Contains("The book can not be deleted. There are still active borrowings in the system.", ex.Message);
    }

    [Fact]
    public async Task DeleteBook_NegativeId_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => bookService.DeleteBookAsync(Guid.Empty.ToString()));
        Assert.Contains("Invalid id.", ex.Message);
    }

    [Fact]
    public async Task DeleteBook_NonExistingBook_ThrowsKeyNotFoundException()
    {
        //Arrange
        BookService bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => bookService.DeleteBookAsync(Guid.Empty.ToString()));
    }
    #endregion

    #region Init
    private void InitializeGuids()
    {
        guids["a1"]    = Guid.NewGuid();
        guids["a2"]    = Guid.NewGuid();
        guids["a3"]    = Guid.NewGuid();
        guids["c1"]    = Guid.NewGuid();
        guids["c2"]    = Guid.NewGuid();
        guids["b1"]    = Guid.NewGuid();
        guids["b2"]    = Guid.NewGuid();
        guids["b3"]    = Guid.NewGuid();
        guids["b4"]    = Guid.NewGuid();
        guids["b5"]    = Guid.NewGuid();
        guids["b6"]    = Guid.NewGuid();
        guids["b1_c1"] = Guid.NewGuid();
        guids["b1_c2"] = Guid.NewGuid();
        guids["b2_c3"] = Guid.NewGuid();
        guids["b2_c4"] = Guid.NewGuid();
        guids["b3_c5"] = Guid.NewGuid();
        guids["b4_c6"] = Guid.NewGuid();
        guids["r"]     = Guid.NewGuid();
        guids["bor1"]  = Guid.NewGuid();
        guids["bor2"]  = Guid.NewGuid();
        guids["bor3"]  = Guid.NewGuid();
        guids["bor4"]  = Guid.NewGuid();
        guids["bor5"]  = Guid.NewGuid();
        guids["bor6"]  = Guid.NewGuid();
    }

    private void InitializeAuthors()
    {
        Author a1 = new() { Id = guids["a1"], FirstName = "N/A", LastName = "N/A", BornDate = null };
        Author a2 = new() { Id = guids["a2"], FirstName = "Adam", LastName = "Mickiewicz", BornDate = new DateTime(1798, 12, 24) };
        Author a3 = new() { Id = guids["a3"], FirstName = "Jan", LastName = "Kowalski", BornDate = new DateTime(1968, 12, 09) };
        List<Author> authors = [a1, a2, a3];

        mockAuthorService.Setup(repo => repo.GetAllAuthorsAsync()).ReturnsAsync(authors);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a1"])).ReturnsAsync(a1);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a2"])).ReturnsAsync(a2);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a3"])).ReturnsAsync(a3);
    }

    private void InitializeCategories()
    {
        Category c1 = new() { Id = guids["c1"], Name = "Novel" };
        Category c2 = new() { Id = guids["c2"], Name = "Other" };
        List<Category> categories = [c1, c2];

        mockCategoryRepository.Setup(repo => repo.GetAllCategories()).Returns(categories);
        mockCategoryRepository.Setup(repo => repo.GetCategories()).Returns(categories.AsQueryable());
        mockCategoryRepository.Setup(repo => repo.GetCategory(guids["c1"])).Returns(c1);
        mockCategoryRepository.Setup(repo => repo.GetCategory(guids["c2"])).Returns(c2);
    }

    private async Task InitializeBooks()
    {
        var a1 = await mockAuthorService.Object.GetAuthorByIdAsync(guids["a1"]);
        var a2 = await mockAuthorService.Object.GetAuthorByIdAsync(guids["a1"]);
        var a3 = await mockAuthorService.Object.GetAuthorByIdAsync(guids["a1"]);
        Category c1 = mockCategoryRepository.Object.GetCategory(guids["c1"]) ?? throw new KeyNotFoundException("Category c1 not found.");
        Category c2 = mockCategoryRepository.Object.GetCategory(guids["c2"]) ?? throw new KeyNotFoundException("Category c2 not found.");

        Book b1 = new() { Id = guids["b1"], Title = "Some old book", Author = a1, AuthorId = guids["a1"], Category = c1, CategoryId = guids["c1"], Language = Language.English, ReleaseDate = new DateTime(1900, 1, 1) };
        Book b2 = new() { Id = guids["b2"], Title = "Some old German book", Author = a1, AuthorId = guids["a1"], Category = c1, CategoryId = guids["c1"], Language = Language.German, ReleaseDate = new DateTime(1800, 1, 1) };
        Book b3 = new() { Id = guids["b3"], Title = "Some new French book", Author = a1, AuthorId = guids["a1"], Category = c2, CategoryId = guids["c2"], Language = Language.French, ReleaseDate = new DateTime(2010, 5, 7) };
        Book b4 = new() { Id = guids["b4"], Title = "Dziady część II", Author = a2, AuthorId = guids["a2"], Category = c2, CategoryId = guids["c2"], Language = Language.Polish, ReleaseDate = new DateTime(1823, 1, 1) };
        Book b5 = new() { Id = guids["b5"], Title = "Dziady część III", Author = a2, AuthorId = guids["a2"], Category = c2, CategoryId = guids["c2"], Language = Language.Polish, ReleaseDate = new DateTime(1832, 1, 1) };
        Book b6 = new() { Id = guids["b6"], Title = "Another book", Author = a3, AuthorId = guids["a3"], Category = c1, CategoryId = guids["c1"], Language = Language.English, ReleaseDate = new DateTime(1985, 1, 9), Tags = "Some" };
        List<Book> books = [b1, b2, b3, b4, b5, b6];

        mockBookRepository.Setup(repo => repo.GetAllBooks()).Returns(books);
        mockBookRepository.Setup(repo => repo.GetBooks()).Returns(books.AsQueryable());
        mockBookRepository.Setup(repo => repo.GetBook(guids["b1"])).Returns(b1);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b2"])).Returns(b2);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b3"])).Returns(b3);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b4"])).Returns(b4);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b5"])).Returns(b5);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b6"])).Returns(b6);
    }

    private void InitializeCopies()
    {
        Book b1 = mockBookRepository.Object.GetBook(guids["b1"]) ?? throw new KeyNotFoundException("Book b1 not found.");
        Book b2 = mockBookRepository.Object.GetBook(guids["b2"]) ?? throw new KeyNotFoundException("Book b2 not found.");
        Book b3 = mockBookRepository.Object.GetBook(guids["b3"]) ?? throw new KeyNotFoundException("Book b3 not found.");
        Book b4 = mockBookRepository.Object.GetBook(guids["b4"]) ?? throw new KeyNotFoundException("Book b4 not found.");

        Copy c1 = new() { Id = guids["b1_c1"], Book = b1, BookId = guids["b1"] };
        Copy c2 = new() { Id = guids["b1_c2"], Book = b1, BookId = guids["b1"] };
        Copy c3 = new() { Id = guids["b2_c3"], Book = b2, BookId = guids["b2"] };
        Copy c4 = new() { Id = guids["b2_c4"], Book = b2, BookId = guids["b2"] };
        Copy c5 = new() { Id = guids["b3_c5"], Book = b3, BookId = guids["b3"] };
        Copy c6 = new() { Id = guids["b4_c6"], Book = b4, BookId = guids["b4"] };
        List<Copy> copies = [c1, c2, c3, c4, c5, c6];

        mockCopyRepository.Setup(repo => repo.GetAllCopies()).Returns(copies);
        mockCopyRepository.Setup(repo => repo.GetCopies()).Returns(copies.AsQueryable());
        mockCopyRepository.Setup(repo => repo.GetCopy(guids["b1_c1"])).Returns(c1);
        mockCopyRepository.Setup(repo => repo.GetCopy(guids["b1_c2"])).Returns(c2);
        mockCopyRepository.Setup(repo => repo.GetCopy(guids["b2_c3"])).Returns(c3);
        mockCopyRepository.Setup(repo => repo.GetCopy(guids["b2_c4"])).Returns(c4);
        mockCopyRepository.Setup(repo => repo.GetCopy(guids["b3_c5"])).Returns(c5);
        mockCopyRepository.Setup(repo => repo.GetCopy(guids["b4_c6"])).Returns(c6);
    }

    private void InitializeReaders()
    {
        Reader r = new() { Id = guids["r"], CardNumber = "000-111-222", FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@mail.com", Phone = "+48 789 456 123" };
        List<Reader> readers = [r];

        mockReaderRepository.Setup(repo => repo.GetAllReaders()).Returns(readers);
        mockReaderRepository.Setup(repo => repo.GetReaders()).Returns(readers.AsQueryable());
        mockReaderRepository.Setup(repo => repo.GetReader(guids["r"])).Returns(r);
    }

    private void InitializeBorrowings()
    {
        Copy c1 = mockCopyRepository.Object.GetCopy(guids["b1_c1"]) ?? throw new KeyNotFoundException("Copy c1 not found.");
        Copy c2 = mockCopyRepository.Object.GetCopy(guids["b1_c2"]) ?? throw new KeyNotFoundException("Copy c2 not found.");
        Copy c3 = mockCopyRepository.Object.GetCopy(guids["b2_c3"]) ?? throw new KeyNotFoundException("Copy c3 not found.");
        Copy c5 = mockCopyRepository.Object.GetCopy(guids["b3_c5"]) ?? throw new KeyNotFoundException("Copy c5 not found.");
        Copy c6 = mockCopyRepository.Object.GetCopy(guids["b4_c6"]) ?? throw new KeyNotFoundException("Copy c6 not found.");
        Reader r1 = mockReaderRepository.Object.GetReader(guids["r"]) ?? throw new KeyNotFoundException("Reader r1 not found.");

        Borrowing bor1 = new() { Id = guids["bor1"], Copy = c1, CopyId = guids["b1_c1"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5) };
        Borrowing bor2 = new() { Id = guids["bor2"], Copy = c2, CopyId = guids["b1_c2"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5) };
        Borrowing bor3 = new() { Id = guids["bor3"], Copy = c3, CopyId = guids["b2_c3"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5) };
        Borrowing bor4 = new() { Id = guids["bor4"], Copy = c5, CopyId = guids["b3_c5"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5), ActualReturnDate = DateTime.Now.AddDays(-2) };
        Borrowing bor5 = new() { Id = guids["bor5"], Copy = c6, CopyId = guids["b4_c6"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-15), ActualReturnDate = DateTime.Now.AddDays(-10) };
        Borrowing bor6 = new() { Id = guids["bor6"], Copy = c6, CopyId = guids["b4_c6"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5) };
        List<Borrowing> borrowings = [bor1, bor2, bor3, bor4, bor5, bor6];

        mockBorrowingRepository.Setup(repo => repo.GetAllBorrowings()).Returns(borrowings);
        mockBorrowingRepository.Setup(repo => repo.GetBorrowings()).Returns(borrowings.AsQueryable());
        mockBorrowingRepository.Setup(repo => repo.GetBorrowing(guids["bor1"])).Returns(bor1);
        mockBorrowingRepository.Setup(repo => repo.GetBorrowing(guids["bor2"])).Returns(bor2);
        mockBorrowingRepository.Setup(repo => repo.GetBorrowing(guids["bor3"])).Returns(bor3);
        mockBorrowingRepository.Setup(repo => repo.GetBorrowing(guids["bor4"])).Returns(bor4);
        mockBorrowingRepository.Setup(repo => repo.GetBorrowing(guids["bor5"])).Returns(bor5);
        mockBorrowingRepository.Setup(repo => repo.GetBorrowing(guids["bor6"])).Returns(bor6);
    }
    #endregion
}