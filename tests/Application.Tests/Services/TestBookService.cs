using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Application.Services;

namespace SimpleLibrary.Tests.Application.Services;

public class TestBookService
{
    private readonly Dictionary<string, Guid> guids = DataInitializer.InitializeGuids();
    private readonly Mock<IRepository<Book>> mockBookRepository;
    private readonly BookService bookService;

    public TestBookService()
    {   
        Mock<IRepository<Author>> mockAuthorRepository = DataInitializer.InitializeAuthorRepository(guids);
        Mock<IAuthorService> mockAuthorService = DataInitializer.InitializeAuthorService(guids);
        Mock<IRepository<Category>> mockCategoryRepository = DataInitializer.InitializeCategories(guids);
        mockBookRepository = DataInitializer.InitializeBookRepositoryAsync(guids, mockAuthorRepository, mockCategoryRepository).GetAwaiter().GetResult();
        Mock<IRepository<Copy>> mockCopyRepository = DataInitializer.InitializeCopies(guids, mockBookRepository).GetAwaiter().GetResult();
        Mock<IRepository<Reader>> mockReaderRepository = DataInitializer.InitializeReaders(guids);
        Mock<IRepository<Borrowing>> mockBorrowingRepository = DataInitializer.InitializeBorrowingsAsync(guids, mockCopyRepository, mockReaderRepository).GetAwaiter().GetResult();

        bookService = new(mockBookRepository.Object, mockAuthorService.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockCategoryRepository.Object);
    }

    #region SearchBooks
    [Fact]
    public async Task SearchBooks_NoFilters_ReturnsAllBooks()
    {
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
        //Act && Assert
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(olderThan: "123"));
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(olderThan: "test"));
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
    public async Task SearchBooks_InvalidFormatOfNewerThan_ThrowsFormatException()
    {
        //Act && Assert
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(newerThan: "123"));
        await Assert.ThrowsAsync<FormatException>(() => bookService.SearchBooksAsync(newerThan: "test"));
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
    public async Task SearchBooks_ByNonExistingAuthor_ThrowsArgumentOutOfRangeException()
    {
        //Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => bookService.SearchBooksAsync(authorId: Guid.Empty.ToString()));
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

    [Fact]
    public async Task SearchBooks_ByNonExistingCategory_ThrowsArgumentOutOfRangeException()
    {
        //Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => bookService.SearchBooksAsync(categoryId: Guid.Empty.ToString()));
    }

    [Fact]
    public async Task SearchBooks_PageTwoOfSizeTwo_ReturnsTwoBooks()
    {
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
        //Act
        var result = await bookService.GetBookByIdAsync(guids["b1"].ToString());

        //Assert
        Assert.NotNull(result);
        Assert.Equal(guids["b1"], result.Id);
    }

    [Fact]
    public async Task GetBookById_NonExisitingBook_ThrowsKeyNotFoundException()
    {
        //Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => bookService.GetBookByIdAsync(Guid.Empty.ToString()));
    }

    [Fact]
    public async Task GetBookById_NegativeId_ThrowsArgumentException()
    {
        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => bookService.GetBookByIdAsync(Guid.Empty.ToString()));
    }
    #endregion

    #region CreateBook
    [Fact]
    public async Task CreateBook_CorrectInput_CreatesBook()
    {
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
        var dziadyIII = new BookPostDTO ("Dziady część III", "", "1832-1-1", "Polish", ["tag", "another"], guids["a2"].ToString(), guids["c2"].ToString());

        //Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => bookService.CreateBookAsync(dziadyIII));
    }
    #endregion

    #region UpdateBook
    [Fact]
    public async Task UpdateBook_ChangedTitle_UpdatesBook()
    {
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
        //Act
        await bookService.DeleteBookAsync(guids["b6"].ToString());

        //Assert
        mockBookRepository.Verify(r => r.DeleteAsync(guids["b6"]));
    }

    [Fact]
    public async Task DeleteBook_ExistingBookWithActiveBorrowings_ThrowsInvalidOperation()
    {
        //Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => bookService.DeleteBookAsync(guids["b2"].ToString()));
        Assert.Contains("The book can not be deleted. There are still active borrowings in the system.", ex.Message);
    }

    [Fact]
    public async Task DeleteBook_NegativeId_ThrowsArgumentException()
    {
        //Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => bookService.DeleteBookAsync(Guid.Empty.ToString()));
        Assert.Contains("Invalid id.", ex.Message);
    }

    [Fact]
    public async Task DeleteBook_NonExistingBook_ThrowsKeyNotFoundException()
    {
        //Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => bookService.DeleteBookAsync(Guid.Empty.ToString()));
    }
    #endregion
}