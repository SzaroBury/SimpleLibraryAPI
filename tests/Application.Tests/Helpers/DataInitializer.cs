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
            ["a4"] = Guid.NewGuid(),
            ["c1"] = Guid.NewGuid(),
            ["c2"] = Guid.NewGuid(),
            ["c3"] = Guid.NewGuid(),
            ["c4"] = Guid.NewGuid(),
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
            ["b2_c7"] = Guid.NewGuid(),
            ["b6_c8"] = Guid.NewGuid(),
            ["r1"] = Guid.NewGuid(),
            ["r2"] = Guid.NewGuid(),
            ["r3"] = Guid.NewGuid(),
            ["r4"] = Guid.NewGuid(),
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
        Author a4 = new() { Id = guids["a4"], FirstName = "Some random", LastName = "Dude", BornDate = new DateTime(1997, 7, 5) };
        List<Author> authors = [a1, a2, a3, a4];

        Mock<IRepository<Author>> mockAuthorRepository = new();
        mockAuthorRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(authors);
        mockAuthorRepository.Setup(repo => repo.GetQueryable()).Returns(authors.AsQueryable());
        mockAuthorRepository.Setup(repo => repo.GetByIdAsync(guids["a1"])).ReturnsAsync(a1);
        mockAuthorRepository.Setup(repo => repo.GetByIdAsync(guids["a2"])).ReturnsAsync(a2);
        mockAuthorRepository.Setup(repo => repo.GetByIdAsync(guids["a3"])).ReturnsAsync(a3);
        mockAuthorRepository.Setup(repo => repo.GetByIdAsync(guids["a4"])).ReturnsAsync(a4);
        mockAuthorRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
        return mockAuthorRepository;
    }

    public static Mock<IAuthorService> InitializeAuthorService(Dictionary<string, Guid> guids)
    {
        Author a1 = new() { Id = guids["a1"], FirstName = "N/A", LastName = "N/A", BornDate = null };
        Author a2 = new() { Id = guids["a2"], FirstName = "Adam", LastName = "Mickiewicz", BornDate = new DateTime(1798, 12, 24) };
        Author a3 = new() { Id = guids["a3"], FirstName = "Jan", LastName = "Kowalski", BornDate = new DateTime(1968, 12, 9) };
        Author a4 = new() { Id = guids["a4"], FirstName = "Some random", LastName = "Dude", BornDate = new DateTime(1997, 7, 5) };
        List<Author> authors = [a1, a2, a3, a4];

        Mock<IAuthorService> mockAuthorService = new();
        mockAuthorService.Setup(repo => repo.GetAllAuthorsAsync()).ReturnsAsync(authors);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a1"].ToString())).ReturnsAsync(a1);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a1"])).ReturnsAsync(a1);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a2"].ToString())).ReturnsAsync(a2);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a2"])).ReturnsAsync(a2);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a3"].ToString())).ReturnsAsync(a3);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a3"])).ReturnsAsync(a3);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a4"].ToString())).ReturnsAsync(a4);
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(guids["a4"])).ReturnsAsync(a4);
        string capturedInput = string.Empty;
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(It.Is<string>(input => IsInAuthors(authors, input))))
            .Callback<string>(input => capturedInput = input)
            .Throws(() => new KeyNotFoundException($"An author with the specified id ({capturedInput}) was not found in the system."));
        mockAuthorService.Setup(repo => repo.GetAuthorByIdAsync(It.Is<string>(input => !IsValidGuid(input))))
            .Throws(new FormatException("Invalid author ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000."));

        return mockAuthorService;
    }

    public static Mock<IRepository<Category>> InitializeCategories(Dictionary<string, Guid> guids)
    {
        Category c1 = new() { Id = guids["c1"], Name = "Novel" };
        Category c2 = new() { Id = guids["c2"], Name = "Other" };
        Category c3 = new() { Id = guids["c3"], Name = "Fantasy" };
        Category c4 = new() { Id = guids["c4"], Name = "High Fantasy", ParentCategoryId = guids["c3"] };
        List<Category> categories = [c1, c2, c3, c4];

        Mock<IRepository<Category>> mockCategoryRepository = new();
        mockCategoryRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);
        mockCategoryRepository.Setup(repo => repo.GetQueryable()).Returns(categories.AsQueryable());
        mockCategoryRepository.Setup(repo => repo.GetByIdAsync(guids["c1"])).ReturnsAsync(c1);
        mockCategoryRepository.Setup(repo => repo.GetByIdAsync(guids["c2"])).ReturnsAsync(c2);
        mockCategoryRepository.Setup(repo => repo.GetByIdAsync(guids["c3"])).ReturnsAsync(c3);
        mockCategoryRepository.Setup(repo => repo.GetByIdAsync(guids["c4"])).ReturnsAsync(c4);
        return mockCategoryRepository;
    }

    public static async Task<Mock<IRepository<Book>>> InitializeBookRepositoryAsync(
        Dictionary<string, Guid> guids, 
        Mock<IRepository<Author>>? mockAuthorRepository = null, 
        Mock<IRepository<Category>>? mockCategoryRepository = null)
    {
        mockAuthorRepository ??= InitializeAuthorRepository(guids);

        mockCategoryRepository ??= InitializeCategories(guids);

        var a1 = await mockAuthorRepository.Object.GetByIdAsync(guids["a1"]) ?? throw new KeyNotFoundException("Author a1 not found.");
        var a2 = await mockAuthorRepository.Object.GetByIdAsync(guids["a2"]) ?? throw new KeyNotFoundException("Author a2 not found.");
        var a3 = await mockAuthorRepository.Object.GetByIdAsync(guids["a3"]) ?? throw new KeyNotFoundException("Author a3 not found.");
        var c1 = await mockCategoryRepository.Object.GetByIdAsync(guids["c1"]) ?? throw new KeyNotFoundException("Category c1 not found.");
        var c2 = await mockCategoryRepository.Object.GetByIdAsync(guids["c2"]) ?? throw new KeyNotFoundException("Category c2 not found.");

        Book b1 = new() { Id = guids["b1"], Title = "Some old book", Author = a1, AuthorId = guids["a1"], Category = c1, CategoryId = guids["c1"], Language = Language.English, ReleaseDate = new DateTime(1900, 1, 1), Tags = "fantasy" };
        Book b2 = new() { Id = guids["b2"], Title = "Some old German book", Author = a1, AuthorId = guids["a1"], Category = c1, CategoryId = guids["c1"], Language = Language.German, ReleaseDate = new DateTime(1800, 1, 1) };
        Book b3 = new() { Id = guids["b3"], Title = "Some new French book", Author = a1, AuthorId = guids["a1"], Category = c2, CategoryId = guids["c2"], Language = Language.French, ReleaseDate = new DateTime(2010, 5, 7) };
        Book b4 = new() { Id = guids["b4"], Title = "Dziady część II", Author = a2, AuthorId = guids["a2"], Category = c2, CategoryId = guids["c2"], Language = Language.Polish, ReleaseDate = new DateTime(1823, 1, 1) };
        Book b5 = new() { Id = guids["b5"], Title = "Dziady część III", Author = a2, AuthorId = guids["a2"], Category = c2, CategoryId = guids["c2"], Language = Language.Polish, ReleaseDate = new DateTime(1832, 1, 1) };
        Book b6 = new() { Id = guids["b6"], Title = "Another book", Author = a3, AuthorId = guids["a3"], Category = c1, CategoryId = guids["c1"], Language = Language.English, ReleaseDate = new DateTime(1985, 1, 9), Tags = "Some" };
        List<Book> books = [b1, b2, b3, b4, b5, b6];

        Mock<IRepository<Book>> mockBookRepository = new();
        mockBookRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(books);
        mockBookRepository.Setup(repo => repo.GetQueryable()).Returns(books.AsQueryable());
        mockBookRepository.Setup(repo => repo.GetByIdAsync(guids["b1"])).ReturnsAsync(b1);
        mockBookRepository.Setup(repo => repo.GetByIdAsync(guids["b2"])).ReturnsAsync(b2);
        mockBookRepository.Setup(repo => repo.GetByIdAsync(guids["b3"])).ReturnsAsync(b3);
        mockBookRepository.Setup(repo => repo.GetByIdAsync(guids["b4"])).ReturnsAsync(b4);
        mockBookRepository.Setup(repo => repo.GetByIdAsync(guids["b5"])).ReturnsAsync(b5);
        mockBookRepository.Setup(repo => repo.GetByIdAsync(guids["b6"])).ReturnsAsync(b6);

        return mockBookRepository;
    }

    public async static Task<Mock<IRepository<Copy>>> InitializeCopiesAsync(
        Dictionary<string, Guid> guids, 
        Mock<IRepository<Book>>? mockBookRepository = null)
    {
        mockBookRepository ??= await InitializeBookRepositoryAsync(guids);

        Book b1 = await mockBookRepository.Object.GetByIdAsync(guids["b1"]) ?? throw new KeyNotFoundException("Book b1 not found.");
        Book b2 = await mockBookRepository.Object.GetByIdAsync(guids["b2"]) ?? throw new KeyNotFoundException("Book b2 not found.");
        Book b3 = await mockBookRepository.Object.GetByIdAsync(guids["b3"]) ?? throw new KeyNotFoundException("Book b3 not found.");
        Book b4 = await mockBookRepository.Object.GetByIdAsync(guids["b4"]) ?? throw new KeyNotFoundException("Book b4 not found.");
        Book b6 = await mockBookRepository.Object.GetByIdAsync(guids["b6"]) ?? throw new KeyNotFoundException("Book b6 not found.");

        Copy c1 = new() { Id = guids["b1_c1"], Book = b1, BookId = guids["b1"], CopyNumber = 1, ShelfNumber = 1, Condition = CopyCondition.Good, AcquisitionDate = DateTime.Today.AddDays(-30), LastInspectionDate = DateTime.Today.AddDays(-30) };
        Copy c2 = new() { Id = guids["b1_c2"], Book = b1, BookId = guids["b1"], CopyNumber = 2, ShelfNumber = 1, Condition = CopyCondition.Good, AcquisitionDate = DateTime.Today.AddDays(-30), LastInspectionDate = DateTime.Today.AddDays(-30) };
        Copy c3 = new() { Id = guids["b2_c3"], Book = b2, BookId = guids["b2"], CopyNumber = 1, ShelfNumber = 1, Condition = CopyCondition.Good, AcquisitionDate = DateTime.Today.AddDays(-30), LastInspectionDate = DateTime.Today.AddDays(-30) };
        Copy c4 = new() { Id = guids["b2_c4"], Book = b2, BookId = guids["b2"], CopyNumber = 2, ShelfNumber = 1, Condition = CopyCondition.Good, AcquisitionDate = DateTime.Today.AddDays(-30), LastInspectionDate = DateTime.Today.AddDays(-30) };
        Copy c5 = new() { Id = guids["b3_c5"], Book = b3, BookId = guids["b3"], CopyNumber = 1, ShelfNumber = 2, Condition = CopyCondition.Good, AcquisitionDate = DateTime.Today.AddDays(-30), LastInspectionDate = DateTime.Today.AddDays(-30) };
        Copy c6 = new() { Id = guids["b4_c6"], Book = b4, BookId = guids["b4"], CopyNumber = 1, ShelfNumber = 2, Condition = CopyCondition.Good, AcquisitionDate = DateTime.Today.AddDays(-30), LastInspectionDate = DateTime.Today.AddDays(-30) };
        Copy c7 = new() { Id = guids["b2_c7"], Book = b2, BookId = guids["b2"], CopyNumber = 4, ShelfNumber = 1, Condition = CopyCondition.Bad,  AcquisitionDate = DateTime.Today.AddDays(-90), LastInspectionDate = DateTime.Today.AddDays(-30), IsLost = true };
        Copy c8 = new() { Id = guids["b6_c8"], Book = b6, BookId = guids["b6"], CopyNumber = 1, ShelfNumber = 2, Condition = CopyCondition.New,  AcquisitionDate = DateTime.Today.AddDays(-30), LastInspectionDate = DateTime.Today.AddDays(-30) };
        List<Copy> copies = [c1, c2, c3, c4, c5, c6, c7, c8];

        Mock<IRepository<Copy>> mockCopyRepository = new();
        mockCopyRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(copies);
        mockCopyRepository.Setup(repo => repo.GetQueryable()).Returns(copies.AsQueryable());
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b1_c1"])).ReturnsAsync(c1);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b1_c2"])).ReturnsAsync(c2);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b2_c3"])).ReturnsAsync(c3);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b2_c4"])).ReturnsAsync(c4);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b3_c5"])).ReturnsAsync(c5);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b4_c6"])).ReturnsAsync(c6);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b2_c7"])).ReturnsAsync(c7);
        mockCopyRepository.Setup(repo => repo.GetByIdAsync(guids["b6_c8"])).ReturnsAsync(c8);

        return mockCopyRepository;
    }

    public static Mock<IRepository<Reader>> InitializeReaders(Dictionary<string, Guid> guids)
    {
        Reader r  = new() { Id = guids["r1"],  CardNumber = "012", FirstName = "Jan",   LastName = "Kowalski", Email = "jan.kowalski@mail.com",  Phone = "+48789456123" };
        Reader r2 = new() { Id = guids["r2"], CardNumber = "123", FirstName = "John",  LastName = "Stones",   Email = "john.stones@mail.com",   Phone = "+44123456789" };
        Reader r3 = new() { Id = guids["r3"], CardNumber = "234", FirstName = "Marek", LastName = "Konarek",  Email = "marek.konarek@mail.com", Phone = "+48879456789" };
        Reader r4 = new() { Id = guids["r4"], CardNumber = "345", FirstName = "Piotr", LastName = "Nowak",    Email = "example@mail.com",       Phone = "+48978456789", IsBanned = true, BannedDate = DateTime.Today };
        List<Reader> readers = [r, r2, r3, r4];

        Mock<IRepository<Reader>> mockReaderRepository = new();
        mockReaderRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(readers);
        mockReaderRepository.Setup(repo => repo.GetQueryable()).Returns(readers.AsQueryable());
        mockReaderRepository.Setup(repo => repo.GetByIdAsync(guids["r1"])).ReturnsAsync(r);
        mockReaderRepository.Setup(repo => repo.GetByIdAsync(guids["r2"])).ReturnsAsync(r2);
        mockReaderRepository.Setup(repo => repo.GetByIdAsync(guids["r3"])).ReturnsAsync(r3);
        mockReaderRepository.Setup(repo => repo.GetByIdAsync(guids["r4"])).ReturnsAsync(r4);

        return mockReaderRepository;
    }

    public async static Task<Mock<IRepository<Borrowing>>> InitializeBorrowingsAsync(
        Dictionary<string, Guid> guids, 
        Mock<IRepository<Copy>>? mockCopyRepository = null,
        Mock<IRepository<Reader>>? mockReaderRepository = null)
    {
        mockCopyRepository ??= await InitializeCopiesAsync(guids);

        mockReaderRepository ??= InitializeReaders(guids);

        var c1 = await mockCopyRepository.Object.GetByIdAsync(guids["b1_c1"]) ?? throw new KeyNotFoundException("Copy c1 not found.");
        var c2 = await mockCopyRepository.Object.GetByIdAsync(guids["b1_c2"]) ?? throw new KeyNotFoundException("Copy c2 not found.");
        var c3 = await mockCopyRepository.Object.GetByIdAsync(guids["b2_c3"]) ?? throw new KeyNotFoundException("Copy c3 not found.");
        var c5 = await mockCopyRepository.Object.GetByIdAsync(guids["b3_c5"]) ?? throw new KeyNotFoundException("Copy c5 not found.");
        var c6 = await mockCopyRepository.Object.GetByIdAsync(guids["b4_c6"]) ?? throw new KeyNotFoundException("Copy c6 not found.");
        var r1 = await mockReaderRepository.Object.GetByIdAsync(guids["r1"]) ?? throw new KeyNotFoundException("Reader r1 not found.");
        var r4 = await mockReaderRepository.Object.GetByIdAsync(guids["r4"]) ?? throw new KeyNotFoundException("Reader r4 not found.");

        Borrowing bor1 = new() { Id = guids["bor1"], Copy = c1, CopyId = guids["b1_c1"], Reader = r1, ReaderId = guids["r1"],  StartedDate = DateTime.Now.AddDays(-5) };
        Borrowing bor2 = new() { Id = guids["bor2"], Copy = c2, CopyId = guids["b1_c2"], Reader = r1, ReaderId = guids["r1"],  StartedDate = DateTime.Now.AddDays(-5),  ActualReturnDate = DateTime.Now.AddDays(-2) };
        Borrowing bor3 = new() { Id = guids["bor3"], Copy = c3, CopyId = guids["b2_c3"], Reader = r1, ReaderId = guids["r1"],  StartedDate = DateTime.Now.AddDays(-5) };
        Borrowing bor4 = new() { Id = guids["bor4"], Copy = c5, CopyId = guids["b3_c5"], Reader = r1, ReaderId = guids["r1"],  StartedDate = DateTime.Now.AddDays(-5),  ActualReturnDate = DateTime.Now.AddDays(-2) };
        Borrowing bor5 = new() { Id = guids["bor5"], Copy = c6, CopyId = guids["b4_c6"], Reader = r1, ReaderId = guids["r1"],  StartedDate = DateTime.Now.AddDays(-15), ActualReturnDate = DateTime.Now.AddDays(-10) };
        Borrowing bor6 = new() { Id = guids["bor6"], Copy = c6, CopyId = guids["b4_c6"], Reader = r4, ReaderId = guids["r4"],  StartedDate = DateTime.Now.AddDays(-5) };
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

    public async static Task<Mock<IRepository<Copy>>> AddBorrowingsAsync(
        this Mock<IRepository<Copy>> copyRepo, 
        Mock<IRepository<Borrowing>> borrowingRepo)
    {
        List<Copy> copies = (await copyRepo.Object.GetAllAsync()).ToList();
        var borrowings = await borrowingRepo.Object.GetAllAsync();

        for(int i = 0; i < copies.Count; i++)
        {
            copies[i].Borrowings = borrowings.Where(b => b.CopyId == copies[i].Id).ToList();
        }

        return copyRepo;
    }

    public async static Task<Mock<IRepository<Book>>> AddCopiesAsync(
        this Mock<IRepository<Book>> bookRepo, 
        Mock<IRepository<Copy>> copyRepo)
    {
        List<Book> books = (await bookRepo.Object.GetAllAsync()).ToList();
        var copies = await copyRepo.Object.GetAllAsync();

        for(int i = 0; i < books.Count(); i++)
        {
            books[i].Copies = copies.Where(b => b.BookId == books[i].Id).ToList();
        }

        return bookRepo;
    }

    public async static Task<Mock<IUnitOfWork>> InitializeUnitOfWorkAsync(
        Dictionary<string, Guid> guids, 
        Mock<IRepository<Author>>?      mockAuthorRepository = null,
        Mock<IRepository<Category>>?    mockCategoryRepository = null,
        Mock<IRepository<Book>>?        mockBookRepository = null,
        Mock<IRepository<Copy>>?        mockCopyRepository = null,
        Mock<IRepository<Reader>>?      mockReaderRepository = null,
        Mock<IRepository<Borrowing>>?   mockBorrowingRepository = null)
    {
        var authorRepository    = mockAuthorRepository    ?? InitializeAuthorRepository(guids);
        var categoryRepository  = mockCategoryRepository  ?? InitializeCategories(guids);
        var bookRepository      = mockBookRepository      ?? InitializeBookRepositoryAsync(guids, authorRepository, categoryRepository).GetAwaiter().GetResult();
        var copyRepository      = mockCopyRepository      ?? InitializeCopiesAsync(guids, bookRepository).GetAwaiter().GetResult();
        var readerRepository    = mockReaderRepository    ?? InitializeReaders(guids);
        var borrowingRepository = mockBorrowingRepository ?? InitializeBorrowingsAsync(guids, copyRepository, readerRepository).GetAwaiter().GetResult();
        copyRepository = await copyRepository.AddBorrowingsAsync(borrowingRepository);
        bookRepository = await bookRepository.AddCopiesAsync(copyRepository);

        Mock<IUnitOfWork> mockUnitOfWork = new();
        mockUnitOfWork.Setup(uow => uow.GetRepository<Author>()).Returns(authorRepository.Object);
        mockUnitOfWork.Setup(uow => uow.GetRepository<Category>()).Returns(categoryRepository.Object);
        mockUnitOfWork.Setup(uow => uow.GetRepository<Book>()).Returns(bookRepository.Object);
        mockUnitOfWork.Setup(uow => uow.GetRepository<Copy>()).Returns(copyRepository.Object);
        mockUnitOfWork.Setup(uow => uow.GetRepository<Reader>()).Returns(readerRepository.Object);
        mockUnitOfWork.Setup(uow => uow.GetRepository<Borrowing>()).Returns(borrowingRepository.Object);
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(0);
        return mockUnitOfWork;
    }

    private static bool IsValidGuid(string input)
    {
        var isValid = Guid.TryParse(input, out _);
        Console.WriteLine($"Checking '{input}' → IsValid: {isValid}");
        return isValid;
    }

    private static bool IsInAuthors(List<Author> authors, string guid)
    {
        var isIn = !authors.Any(author => author.Id.ToString() == guid);
        Console.WriteLine($"Checking '{guid}' → IsInAuthors: {isIn}");
        return isIn;
    }
}