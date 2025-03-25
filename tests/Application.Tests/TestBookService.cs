using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services;
using Moq;

namespace SimpleLibrary.Application.Services.Tests;

public class TestBookService
{
    Mock<IBookRepository> mockBookRepository = new();
    Mock<ICopyRepository> mockCopyRepository = new();
    Mock<IBorrowingRepository> mockBorrowingRepository = new();
    Mock<IAuthorRepository> mockAuthorRepository = new();
    Mock<ICategoryRepository> mockCategoryRepository = new();

    public TestBookService()
    {
        Author a1 = new Author { Id = 1, FirstName = @"N/A", LastName = @"N/A", BornDate = null };
        Author a2 = new Author { Id = 2, FirstName = @"Adam", LastName = @"Mickiewicz", BornDate = new DateTime(1798, 12, 24) };
        Author a3 = new Author { Id = 3, FirstName = @"Jan", LastName = @"Kowalski", BornDate = new DateTime(1968, 12, 09) };

        Category c1 = new Category { Id = 1, Name = "Novel" };
        Category c2 = new Category { Id = 2, Name = "Other" };

        Reader r1 = new Reader { Id = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@mail.com", Phone = "+48 789 456 123" };

        Book b1 = new Book { Id = 1, Title = "Some old book", Author = a1, AuthorId = 1, Category = c1, CategoryId = 1, Language = Language.English, ReleaseDate = new DateTime(1900, 1, 1) };
        Copy b1_c1 = new Copy { Id = 1, Book = b1, BookId = 1 };
        Borrowing bor1 = new Borrowing { Id = 1, Copy = b1_c1, CopyId = 1, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5) };
        Copy b1_c2 = new Copy { Id = 2, Book = b1, BookId = 1 };
        Borrowing bor2 = new Borrowing { Id = 2, Copy = b1_c2, CopyId = 2, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5) };

        Book b2 = new Book { Id = 2, Title = "Some old German book", Author = a1, AuthorId = 1, Category = c1, CategoryId = 1, Language = Language.German, ReleaseDate = new DateTime(1800, 1, 1) };
        Copy b2_c3 = new Copy { Id = 3, Book = b2, BookId = 2 };
        Borrowing bor3 = new Borrowing { Id = 3, Copy = b2_c3, CopyId = 3, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5) };
        Copy b2_c4 = new Copy { Id = 4, Book = b2, BookId = 2 };

        Book b3 = new Book { Id = 3, Title = "Some new French book", Author = a1, AuthorId = 1, Category = c2, CategoryId = 2, Language = Language.French, ReleaseDate = new DateTime(2010, 5, 7) };
        Copy b3_c5 = new Copy { Id = 5, Book = b3, BookId = 3 };
        Borrowing bor4 = new Borrowing { Id = 4, Copy = b3_c5, CopyId = 5, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5), ActualReturnDate = DateTime.Now.AddDays(-2) };

        Book b4 = new Book { Id = 4, Title = "Dziady część II", Author = a2, AuthorId = 2, Category = c2, CategoryId = 2, Language = Language.Polish, ReleaseDate = new DateTime(1823, 1, 1) };
        Copy b4_c6 = new Copy { Id = 6, Book = b4, BookId = 4 };
        Borrowing bor5 = new Borrowing { Id = 5, Copy = b4_c6, CopyId = 6, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-15), ActualReturnDate = DateTime.Now.AddDays(-10) };
        Borrowing bor6 = new Borrowing { Id = 6, Copy = b4_c6, CopyId = 6, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5) };

        Book b5 = new Book { Id = 5, Title = "Dziady część III", Author = a2, AuthorId = 2, Category = c2, CategoryId = 2, Language = Language.Polish, ReleaseDate = new DateTime(1832, 1, 1) };

        Book b6 = new Book { Id = 6, Title = "Another book", Author = a3, AuthorId = 3, Category = c1, CategoryId = 1, Language = Language.English, ReleaseDate = new DateTime(1985, 1, 9), Tags = "Some" };

        //Arrange 
        mockBookRepository.Setup(repo => repo.GetAllBooks()).Returns(new List<Book>() 
        {
            b1, b2, b3, b4, b5, b6
        });
        mockBookRepository.Setup(repo => repo.GetBooks()).Returns(new List<Book>() 
        {
            b1, b2, b3, b4, b5, b6
        }.AsQueryable());
        mockBookRepository.Setup(repo => repo.GetBook(1)).Returns(b1);
        mockBookRepository.Setup(repo => repo.GetBook(2)).Returns(b2);
        mockBookRepository.Setup(repo => repo.GetBook(4)).Returns(b4);
        mockBookRepository.Setup(repo => repo.GetBook(6)).Returns(b6);
        mockCopyRepository.Setup(repo => repo.GetAllCopies()).Returns(new List<Copy>()
        {
            b1_c1, b1_c2, b2_c3, b2_c4, b3_c5, b4_c6
        });
        mockCopyRepository.Setup(repo => repo.GetCopies()).Returns(new List<Copy>()
        {
            b1_c1, b1_c2, b2_c3, b2_c4, b3_c5, b4_c6
        }.AsQueryable());
        mockBorrowingRepository.Setup(repo => repo.GetAllBorrowings()).Returns(new List<Borrowing>()
        {
            bor1, bor2, bor3, bor4, bor5, bor6
        });
        mockBorrowingRepository.Setup(repo => repo.GetBorrowings()).Returns(new List<Borrowing>()
        {
            bor1, bor2, bor3, bor4, bor5, bor6
        }.AsQueryable());
        mockAuthorRepository.Setup(repo => repo.GetAuthor(2)).Returns(a2);
        mockCategoryRepository.Setup(repo => repo.GetCategory(1)).Returns(c1);
        mockCategoryRepository.Setup(repo => repo.GetCategory(2)).Returns(c2);
    }

    #region SearchBooks
    [Fact]
    public void SearchBooks_NoFilters_ReturnsAllBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks();

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(6, response.Count);
        Assert.Contains(response, b => b.Id == 1);
        Assert.Contains(response, b => b.Id == 2);
        Assert.Contains(response, b => b.Id == 3);
        Assert.Contains(response, b => b.Id == 4);
        Assert.Contains(response, b => b.Id == 5);
        Assert.Contains(response, b => b.Id == 6);
    }

    [Fact]
    public void SearchBooks_SearchSome_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(searchTerm: "Some");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(4, response.Count);
        Assert.Contains(response, b => b.Id == 1);
        Assert.Contains(response, b => b.Id == 2);
        Assert.Contains(response, b => b.Id == 3);
        Assert.Contains(response, b => b.Id == 6);
    }

    [Fact]
    public void SearchBooks_SearchMickiewicz_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(searchTerm: "Mickiewicz");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(2, response.Count);
        Assert.Contains(response, b => b.Id == 4);
        Assert.Contains(response, b => b.Id == 5);
    }

    [Fact]
    public void SearchBooks_SearchNovel_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(searchTerm: "Novel");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.Count);
        Assert.Contains(response, b => b.Id == 1);
        Assert.Contains(response, b => b.Id == 2);
        Assert.Contains(response, b => b.Id == 6);
    }

    [Fact]
    public void SearchBooks_SearchTomato_ReturnsEmptyList()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(searchTerm: "Tomato");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Empty(response);
    }

    [Fact]
    public void SearchBooks_OnlyAvailable_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(isAvailable: true);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(2, response.Count);
        Assert.Contains(response, b => b.Id == 2);
        Assert.Contains(response, b => b.Id == 3);
    }

    [Fact]
    public void SearchBooks_OnlyNotAvailable_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(isAvailable: false);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(4, response.Count);
        Assert.Contains(response, b => b.Id == 1);
        Assert.Contains(response, b => b.Id == 4);
        Assert.Contains(response, b => b.Id == 5);
        Assert.Contains(response, b => b.Id == 6);
    }

    [Fact]
    public void SearchBooks_OlderThan_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(olderThan: "1900-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(4, response.Count);
        Assert.Contains(response, b => b.Id == 1);
        Assert.Contains(response, b => b.Id == 2);
        Assert.Contains(response, b => b.Id == 4);
        Assert.Contains(response, b => b.Id == 5);
    }

    [Fact]
    public void SearchBooks_InvalidFormatOfOlderThan_ThrowsFormatException()
    {
        //Arrange 
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act && Assert
        Assert.Throws<FormatException>(() => bookService.SearchBooks(olderThan: "123"));
        Assert.Throws<FormatException>(() => bookService.SearchBooks(olderThan: "test"));
    }

    [Fact]
    public void SearchBooks_NewerThan_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(newerThan: "1900-01-01");

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.Count);
        Assert.Contains(response, b => b.Id == 1);
        Assert.Contains(response, b => b.Id == 3);
        Assert.Contains(response, b => b.Id == 6);
    }

    [Fact]
    public void SearchBooks_InvalidFormatOfNewerThan_ThrowsFormatException()
    {
        //Arrange 
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act && Assert
        Assert.Throws<FormatException>(() => bookService.SearchBooks(newerThan: "123"));
        Assert.Throws<FormatException>(() => bookService.SearchBooks(newerThan: "test"));
    }

    [Fact]
    public void SearchBooks_ByExistingAuthor_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(author: 2);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(2, response.Count);
        Assert.Contains(response, b => b.Id == 4);
        Assert.Contains(response, b => b.Id == 5);
    }

    [Fact]
    public void SearchBooks_ByNonExistingAuthor_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        Assert.Throws<ArgumentOutOfRangeException>(() => bookService.SearchBooks(author: -10));
    }

    [Fact]
    public void SearchBooks_ByExistingCategory_ReturnsCertainBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(category: 1);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.Count);
        Assert.Contains(response, b => b.Id == 1);
        Assert.Contains(response, b => b.Id == 2);
        Assert.Contains(response, b => b.Id == 6);
    }

    [Fact]
    public void SearchBooks_ByNonExistingCategory_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        Assert.Throws<ArgumentOutOfRangeException>(() => bookService.SearchBooks(category: -10));
    }

    [Fact]
    public void SearchBooks_PageTwoOfSizeTwo_ReturnsTwoBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(page: 2, pageSize: 2);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(2, response.Count);
        Assert.Contains(response, b => b.Id == 3);
        Assert.Contains(response, b => b.Id == 4);
    }

    [Fact]
    public void SearchBooks_PageOneOfSizeThree_ReturnsThreeBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(page: 1, pageSize: 3);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.Count);
    }

    [Fact]
    public void SearchBooks_PageTwoOfSizeThree_ReturnsThreeBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(page: 2, pageSize: 3);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(3, response.Count);
    }

    [Fact]
    public void SearchBooks_PageOneOfSizeFive_ReturnsFiveBooks()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(pageSize: 5);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Equal(5, response.Count);
    }

    [Fact]
    public void SearchBooks_PageTwoOfSizeFive_ReturnsOneBook()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var response = bookService.SearchBooks(page: 2, pageSize: 5);

        //Assert
        Assert.NotNull(response);
        Assert.IsType<List<Book>>(response);
        Assert.Single(response);
    }

    [Fact]
    public void SearchBooks_NonExistingPage_ThrowsInvalidOperationException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        Assert.Throws<InvalidOperationException>(() => bookService.SearchBooks(page: 4, pageSize: 5));
    }

    [Fact]
    public void SearchBooks_NegativePageNumber_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        Assert.Throws<ArgumentOutOfRangeException>(() => bookService.SearchBooks(page: -4));

    }

    [Fact]
    public void SearchBooks_NegativePageSize_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        Assert.Throws<ArgumentOutOfRangeException>(() => bookService.SearchBooks(pageSize: -5));
    }
    #endregion

    #region GetBookById
    [Fact]
    public void GetBookById_ExistingBook_ReturnsBook()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        var result = bookService.GetBookById(1);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void GetBookById_NonExisitingBook_ThrowsKeyNotFoundException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        Assert.Throws<KeyNotFoundException>(() => bookService.GetBookById(8));
    }

    [Fact]
    public void GetBookById_NegativeId_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        Assert.Throws<ArgumentException>(() => bookService.GetBookById(-1));
    }
    #endregion

    #region CreateBook
    [Fact]
    public void CreateBook_CorrectInput_CreatesBook()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var panTadeusz = new BookPostDTO(
            "Pan Tadeusz",
            "Full title: Sir Thaddeus, or the Last Foray in Lithuania: A Nobility's Tale of the Years 1811–1812, in Twelve Books of Verse.",
            "1834-06-28",
            "Polish",
            "poem, epic, national, compulsory reading, unesco",
            2,
            2
        );

        //Act
        var result = bookService.CreateBook(panTadeusz);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<Book>(result);
    }

    [Fact]
    public void CreateBook_EmptyTitle_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someUnknownBook = new BookPostDTO(
            "",
            "Desc of some unknown book",
            "1997-07-05",
            "Polish",
            "novel",
            2,
            2
        );

        //Act & Assert
        Assert.Throws<ArgumentException>(() => bookService.CreateBook(someUnknownBook));
    }

    [Fact]
    public void CreateBook_InvalidDateFormat_ThrowsFormatException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someBook = new BookPostDTO(
            "Some book",
            "Desc of some book",
            "Hello World",
            "Polish",
            "novel",
            2,
            2
        );

        //Act & Assert
        Assert.Throws<FormatException>(() => bookService.CreateBook(someBook));
    }

    [Fact]
    public void CreateBook_InvalidLanguageFormat_ThrowsFormatException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
            var someBook = new BookPostDTO(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Hello world",
            "novel",
            2,
            2
        );

        //Act & Assert
        Assert.Throws<FormatException>(() => bookService.CreateBook(someBook));
    }

    [Fact]
    public void CreateBook_NonExistingAuthor_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someBook = new BookPostDTO(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Polish",
            "novel",
            10,
            2
        );

        //Act & Assert
        Assert.Throws<ArgumentException>(() => bookService.CreateBook(someBook));
    }

    [Fact]
    public void CreateBook_NonExistingCategory_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someBook = new BookPostDTO(
            "Some book",
            "Desc of some book",
            "1997-07-05",
            "Polish",
            "novel",
            2,
            10
        );

        //Act & Assert
        Assert.Throws<ArgumentException>(() => bookService.CreateBook(someBook));
    }

    [Fact]
    public void CreateBook_SimiliarToExisting_ThrowsInvalidOperationException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var dziadyIII = new BookPostDTO ("Dziady część III", "", "1832-1-1", "Polish", "tag, another", 2, 2);

        //Act
        Assert.Throws<InvalidOperationException>(() => bookService.CreateBook(dziadyIII));
    }
    #endregion

    #region UpdateBook
    [Fact]
    public void UpdateBook_ChangedTitle_UpdatesBook()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someOlderBook = new BookPutDTO(
            id: 1,
            title: "Some older book"
        );

        //Act
        var result = bookService.UpdateBook(someOlderBook);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<Book>(result);
    }

    [Fact]
    public void UpdateBook_NonExistingBook_ThrowsKeyNotFoundException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someUnknownBook = new BookPutDTO(
            id: 10,
            title: "Some book"
        );

        //Act & Assert
        Assert.Throws<KeyNotFoundException>(() => bookService.UpdateBook(someUnknownBook));
    }

    [Fact]
    public void UpdateBook_EmptyTitle_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            id: 1,
            title: ""
        );

        //Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => bookService.UpdateBook(someUpdatedBook));
        Assert.Contains("Title can not be empty.", ex.Message);
    }

    [Fact]
    public void UpdateBook_InvalidDateFormat_ThrowsFormatException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            id: 1,
            releaseDate: "Hello world"
        );

        //Act & Assert
        var ex = Assert.Throws<FormatException>(() => bookService.UpdateBook(someUpdatedBook));
        Assert.Contains("Invalid date format.", ex.Message);
    }

    [Fact]
    public void UpdateBook_InvalidLanguageFormat_ThrowsFormatException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            id: 1,
            language: "Simlish"
        );

        //Act & Assert
        var ex = Assert.Throws<FormatException>(() => bookService.UpdateBook(someUpdatedBook));
        Assert.Contains("Invalid language format.", ex.Message);
    }

    [Fact]
    public void UpdateBook_NonExistingAuthor_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            id: 1,
            authorId: 10
        );

        //Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => bookService.UpdateBook(someUpdatedBook));
        Assert.Contains("Author with the given id is not present in the system.", ex.Message);
    }

    [Fact]
    public void UpdateBook_NonExistingCategory_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            id: 1,
            categoryId: 10
        );

        //Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => bookService.UpdateBook(someUpdatedBook));
        Assert.Contains("Category with the given id is not present in the system.", ex.Message);
    }

    [Fact]
    public void UpdateBook_SimilarBookExisting_ThrowsInvalidOperationException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);
        var someUpdatedBook = new BookPutDTO(
            id: 4,
            title: "Dziady część III"
        );

        //Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => bookService.UpdateBook(someUpdatedBook));
        Assert.Contains("There is a similar book in the system.", ex.Message);
    }
    #endregion

    #region DeleteBook
    [Fact]
    public void DeleteBook_ExistingBookWithoutActiveBorrowings_DeletesBookAndCopies()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act
        bookService.DeleteBook(6);

        //Assert
        mockBookRepository.Verify(r => r.DeleteBook(6));
    }

    [Fact]
    public void DeleteBook_ExistingBookWithActiveBorrowings_ThrowsInvalidOperation()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => bookService.DeleteBook(2));
        Assert.Contains("The book can not be deleted. There are still active borrowings in the system.", ex.Message);
    }

    [Fact]
    public void DeleteBook_NegativeId_ThrowsArgumentException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => bookService.DeleteBook(-1));
        Assert.Contains("Invalid id.", ex.Message);
    }

    [Fact]
    public void DeleteBook_NonExistingBook_ThrowsKeyNotFoundException()
    {
        //Arrange
        BookService bookService = new BookService(mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object, mockAuthorRepository.Object, mockCategoryRepository.Object);

        //Act & Assert
        Assert.Throws<KeyNotFoundException>(() => bookService.DeleteBook(10));
    }
    #endregion
}