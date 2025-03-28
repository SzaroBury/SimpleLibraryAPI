using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;

namespace SimpleLibrary.Tests.Application;

public static class DataInitializer
{
    public static Dictionary<string, Guid> InitializeGuids()
    {
        return new Dictionary<string, Guid>
        {
            ["a1"] = Guid.NewGuid(),
            ["a2"] = Guid.NewGuid(),
            ["a3"] = Guid.NewGuid(),
            ["c1"] = Guid.NewGuid(),
            ["c2"] = Guid.NewGuid(),
            ["b1"] = Guid.NewGuid(),
            ["b2"] = Guid.NewGuid(),
            ["b3"] = Guid.NewGuid(),
            ["b4"] = Guid.NewGuid(),
            ["b5"] = Guid.NewGuid(),
            ["b6"] = Guid.NewGuid(),
            ["b1_c1"] = Guid.NewGuid(),
            ["b1_c2"] = Guid.NewGuid(),
            ["b2_c3"] = Guid.NewGuid(),
            ["b2_c4"] = Guid.NewGuid(),
            ["b3_c5"] = Guid.NewGuid(),
            ["b4_c6"] = Guid.NewGuid(),
            ["r"] = Guid.NewGuid(),
            ["bor1"] = Guid.NewGuid(),
            ["bor2"] = Guid.NewGuid(),
            ["bor3"] = Guid.NewGuid(),
            ["bor4"] = Guid.NewGuid(),
            ["bor5"] = Guid.NewGuid(),
            ["bor6"] = Guid.NewGuid()
        };
    }

    public static Mock<IRepository<Author>> InitializeAuthorRepository(Dictionary<string, Guid> guids)
    {
        Author a1 = new() { Id = guids["a1"], FirstName = "N/A", LastName = "N/A", BornDate = null };
        Author a2 = new() { Id = guids["a2"], FirstName = "Adam", LastName = "Mickiewicz", BornDate = new DateTime(1798, 12, 24) };
        Author a3 = new() { Id = guids["a3"], FirstName = "Jan", LastName = "Kowalski", BornDate = new DateTime(1968, 12, 09) };
        List<Author> authors = [a1, a2, a3];

        Mock<IRepository<Author>> mockAuthorRepository = new();
        mockAuthorRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(authors);
        mockAuthorRepository.Setup(repo => repo.GetByIdAsync(guids["a1"])).ReturnsAsync(a1);
        mockAuthorRepository.Setup(repo => repo.GetByIdAsync(guids["a2"])).ReturnsAsync(a2);
        mockAuthorRepository.Setup(repo => repo.GetByIdAsync(guids["a3"])).ReturnsAsync(a3);
        return mockAuthorRepository;
    }

    public static Mock<IAuthorService> InitializeAuthorService(Dictionary<string, Guid> guids)
    {
        Author a1 = new() { Id = guids["a1"], FirstName = "N/A", LastName = "N/A", BornDate = null };
        Author a2 = new() { Id = guids["a2"], FirstName = "Adam", LastName = "Mickiewicz", BornDate = new DateTime(1798, 12, 24) };
        Author a3 = new() { Id = guids["a3"], FirstName = "Jan", LastName = "Kowalski", BornDate = new DateTime(1968, 12, 09) };
        List<Author> authors = [a1, a2, a3];

        Mock<IAuthorService> mockAuthorService = new();
        mockAuthorService.Setup(repo => repo.GetAllAuthorsAsync()).ReturnsAsync(authors);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a1"])).ReturnsAsync(a1);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a2"])).ReturnsAsync(a2);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a3"])).ReturnsAsync(a3);
        return mockAuthorService;
    }

    public static Mock<IRepository<Category>> InitializeCategories(Dictionary<string, Guid> guids)
    {
        Category c1 = new() { Id = guids["c1"], Name = "Novel" };
        Category c2 = new() { Id = guids["c2"], Name = "Other" };
        List<Category> categories = [c1, c2];

        Mock<IRepository<Category>> mockCategoryRepository = new();
        mockCategoryRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);
        mockCategoryRepository.Setup(repo => repo.GetQueryable()).Returns(categories.AsQueryable());
        mockCategoryRepository.Setup(repo => repo.GetByIdAsync(guids["c1"])).ReturnsAsync(c1);
        mockCategoryRepository.Setup(repo => repo.GetByIdAsync(guids["c2"])).ReturnsAsync(c2);
        return mockCategoryRepository;
    }

    public static async Task<Mock<IBookRepository>> InitializeBookRepositoryAsync(
        Dictionary<string, Guid> guids, 
        Mock<IAuthorService> mockAuthorService, 
        Mock<IRepository<Category>> mockCategoryRepository)
    {
        var a1 = await mockAuthorService.Object.GetAuthorByIdAsync(guids["a1"]);
        var a2 = await mockAuthorService.Object.GetAuthorByIdAsync(guids["a1"]);
        var a3 = await mockAuthorService.Object.GetAuthorByIdAsync(guids["a1"]);
        var c1 = await mockCategoryRepository.Object.GetByIdAsync(guids["c1"]) ?? throw new KeyNotFoundException("Category c1 not found.");
        var c2 = await mockCategoryRepository.Object.GetByIdAsync(guids["c2"]) ?? throw new KeyNotFoundException("Category c2 not found.");

        Book b1 = new() { Id = guids["b1"], Title = "Some old book", Author = a1, AuthorId = guids["a1"], Category = c1, CategoryId = guids["c1"], Language = Language.English, ReleaseDate = new DateTime(1900, 1, 1) };
        Book b2 = new() { Id = guids["b2"], Title = "Some old German book", Author = a1, AuthorId = guids["a1"], Category = c1, CategoryId = guids["c1"], Language = Language.German, ReleaseDate = new DateTime(1800, 1, 1) };
        Book b3 = new() { Id = guids["b3"], Title = "Some new French book", Author = a1, AuthorId = guids["a1"], Category = c2, CategoryId = guids["c2"], Language = Language.French, ReleaseDate = new DateTime(2010, 5, 7) };
        Book b4 = new() { Id = guids["b4"], Title = "Dziady część II", Author = a2, AuthorId = guids["a2"], Category = c2, CategoryId = guids["c2"], Language = Language.Polish, ReleaseDate = new DateTime(1823, 1, 1) };
        Book b5 = new() { Id = guids["b5"], Title = "Dziady część III", Author = a2, AuthorId = guids["a2"], Category = c2, CategoryId = guids["c2"], Language = Language.Polish, ReleaseDate = new DateTime(1832, 1, 1) };
        Book b6 = new() { Id = guids["b6"], Title = "Another book", Author = a3, AuthorId = guids["a3"], Category = c1, CategoryId = guids["c1"], Language = Language.English, ReleaseDate = new DateTime(1985, 1, 9), Tags = "Some" };
        List<Book> books = [b1, b2, b3, b4, b5, b6];

        Mock<IBookRepository> mockBookRepository = new();
        mockBookRepository.Setup(repo => repo.GetAllBooks()).Returns(books);
        mockBookRepository.Setup(repo => repo.GetBooks()).Returns(books.AsQueryable());
        mockBookRepository.Setup(repo => repo.GetBook(guids["b1"])).Returns(b1);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b2"])).Returns(b2);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b3"])).Returns(b3);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b4"])).Returns(b4);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b5"])).Returns(b5);
        mockBookRepository.Setup(repo => repo.GetBook(guids["b6"])).Returns(b6);

        return mockBookRepository;
    }

    public static Mock<IRepository<Copy>> InitializeCopies(
        Dictionary<string, Guid> guids, 
        Mock<IBookRepository> mockBookRepository)
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

        Mock<IRepository<Copy>> mockCopyRepository = new();
        mockCopyRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(copies);
        mockCopyRepository.Setup(repo => repo.GetQueryable()).Returns(copies.AsQueryable());
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b1_c1"])).ReturnsAsync(c1);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b1_c2"])).ReturnsAsync(c2);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b2_c3"])).ReturnsAsync(c3);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b2_c4"])).ReturnsAsync(c4);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b3_c5"])).ReturnsAsync(c5);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b4_c6"])).ReturnsAsync(c6);

        return mockCopyRepository;
    }

    public static Mock<IReaderRepository> InitializeReaders(Dictionary<string, Guid> guids)
    {
        Reader r = new() { Id = guids["r"], CardNumber = "000-111-222", FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@mail.com", Phone = "+48 789 456 123" };
        List<Reader> readers = [r];

        Mock<IReaderRepository> mockReaderRepository = new();
        mockReaderRepository.Setup(repo => repo.GetAllReaders()).Returns(readers);
        mockReaderRepository.Setup(repo => repo.GetReaders()).Returns(readers.AsQueryable());
        mockReaderRepository.Setup(repo => repo.GetReader(guids["r"])).Returns(r);

        return mockReaderRepository;
    }

    public async static Task<Mock<IRepository<Borrowing>>> InitializeBorrowingsAsync(
        Dictionary<string, Guid> guids, 
        Mock<IRepository<Copy>> mockCopyRepository,
        Mock<IReaderRepository> mockReaderRepository)
    {
        var c1 = await mockCopyRepository.Object.GetByIdAsync(guids["b1_c1"]) ?? throw new KeyNotFoundException("Copy c1 not found.");
        var c2 = await mockCopyRepository.Object.GetByIdAsync(guids["b1_c2"]) ?? throw new KeyNotFoundException("Copy c2 not found.");
        var c3 = await mockCopyRepository.Object.GetByIdAsync(guids["b2_c3"]) ?? throw new KeyNotFoundException("Copy c3 not found.");
        var c5 = await mockCopyRepository.Object.GetByIdAsync(guids["b3_c5"]) ?? throw new KeyNotFoundException("Copy c5 not found.");
        var c6 = await mockCopyRepository.Object.GetByIdAsync(guids["b4_c6"]) ?? throw new KeyNotFoundException("Copy c6 not found.");
        var r1 = mockReaderRepository.Object.GetReader(guids["r"]) ?? throw new KeyNotFoundException("Reader r1 not found.");

        Borrowing bor1 = new() { Id = guids["bor1"], Copy = c1, CopyId = guids["b1_c1"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5) };
        Borrowing bor2 = new() { Id = guids["bor2"], Copy = c2, CopyId = guids["b1_c2"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5) };
        Borrowing bor3 = new() { Id = guids["bor3"], Copy = c3, CopyId = guids["b2_c3"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5) };
        Borrowing bor4 = new() { Id = guids["bor4"], Copy = c5, CopyId = guids["b3_c5"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5), ActualReturnDate = DateTime.Now.AddDays(-2) };
        Borrowing bor5 = new() { Id = guids["bor5"], Copy = c6, CopyId = guids["b4_c6"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-15), ActualReturnDate = DateTime.Now.AddDays(-10) };
        Borrowing bor6 = new() { Id = guids["bor6"], Copy = c6, CopyId = guids["b4_c6"], Reader = r1, ReaderId = guids["r"], StartedDate = DateTime.Now.AddDays(-5) };
        List<Borrowing> borrowings = [bor1, bor2, bor3, bor4, bor5, bor6];

        Mock<IRepository<Borrowing>> mockBorrowingRepository = new();
        mockBorrowingRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(borrowings);
        mockBorrowingRepository.Setup(repo => repo.GetQueryable()).Returns(borrowings.AsQueryable());
        mockBorrowingRepository.Setup(repo => repo.GetByIdAsync(guids["bor1"])).ReturnsAsync(bor1);
        mockBorrowingRepository.Setup(repo => repo.GetByIdAsync(guids["bor2"])).ReturnsAsync(bor2);
        mockBorrowingRepository.Setup(repo => repo.GetByIdAsync(guids["bor3"])).ReturnsAsync(bor3);
        mockBorrowingRepository.Setup(repo => repo.GetByIdAsync(guids["bor4"])).ReturnsAsync(bor4);
        mockBorrowingRepository.Setup(repo => repo.GetByIdAsync(guids["bor5"])).ReturnsAsync(bor5);
        mockBorrowingRepository.Setup(repo => repo.GetByIdAsync(guids["bor6"])).ReturnsAsync(bor6);

        return mockBorrowingRepository;
    }
}