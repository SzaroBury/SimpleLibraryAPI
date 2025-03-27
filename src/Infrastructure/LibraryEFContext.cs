using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace SimpleLibrary.Infrastructure;

public class LibraryEFContext : DbContext
{
    private Dictionary<string, Guid> guids = [];
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Copy> Copies { get; set; }
    public DbSet<Reader> Readers { get; set; }
    public DbSet<Borrowing> Borrowings { get; set; }

    public LibraryEFContext(DbContextOptions<LibraryEFContext> options) :base(options)
    {
        InitializeGuids();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Author a1 = new() { Id = guids["a1"], FirstName = @"N/A", LastName = @"N/A", BornDate = null};
        Author a2 = new() { Id = guids["a2"], FirstName = @"Adam", LastName = @"Mickiewicz", BornDate = new DateTime(1798, 12, 24) };
        modelBuilder.Entity<Author>().HasData(a1, a2);


        Category c1 = new() { Id = guids["c1"], Name = "Novel" };
        Category c2 = new() { Id = guids["c2"], Name = "Other" };
        modelBuilder.Entity<Category>().HasData(c1, c2);

        Book b1 = new() { Id = guids["b1"], Title = "Some old book",        Author = a1, AuthorId = guids["a1"], Category = c1, CategoryId = guids["c1"], Language = Language.English, ReleaseDate = new DateTime(1900, 1, 1) };
        Book b2 = new() { Id = guids["b2"], Title = "Some old German book", Author = a1, AuthorId = guids["a1"], Category = c1, CategoryId = guids["c1"], Language = Language.German,  ReleaseDate = new DateTime(1800, 1, 1) };
        Book b3 = new() { Id = guids["b3"], Title = "Some new French book", Author = a1, AuthorId = guids["a1"], Category = c2, CategoryId = guids["c2"], Language = Language.French,  ReleaseDate = new DateTime(2010, 5, 7) };
        Book b4 = new() { Id = guids["b4"], Title = "Dziady część II",      Author = a2, AuthorId = guids["a2"], Category = c2, CategoryId = guids["c2"], Language = Language.Polish,  ReleaseDate = new DateTime(1823, 1, 1) };
        Book b5 = new() { Id = guids["b5"], Title = "Dziady część III",     Author = a2, AuthorId = guids["a2"], Category = c2, CategoryId = guids["c2"], Language = Language.Polish,  ReleaseDate = new DateTime(1832, 1, 1) };
        modelBuilder.Entity<Book>().HasData(b1, b2, b3, b4, b5);

        Copy b1_c1  = new() { Id = guids["b1_c1"],  Book = b1, BookId = guids["b1"] }; //Some old book
        Copy b1_c2  = new() { Id = guids["b1_c2"],  Book = b1, BookId = guids["b1"] }; //Some old book
        Copy b2_c3  = new() { Id = guids["b2_c3"],  Book = b2, BookId = guids["b2"] }; //Some old German book
        Copy b2_c4  = new() { Id = guids["b2_c4"],  Book = b2, BookId = guids["b2"] }; //Some old German book
        Copy b3_c5  = new() { Id = guids["b3_c5"],  Book = b3, BookId = guids["b3"] }; //Some new French book
        Copy b3_c6  = new() { Id = guids["b4_c6"],  Book = b3, BookId = guids["b3"] }; //Some new French book
        Copy b4_c7  = new() { Id = guids["b4_c7"],  Book = b4, BookId = guids["b4"] }; //Dziady część II
        Copy b4_c8  = new() { Id = guids["b4_c8"],  Book = b4, BookId = guids["b4"] }; //Dziady część II
        Copy b5_c9  = new() { Id = guids["b5_c9"],  Book = b5, BookId = guids["b5"] }; //Dziady część III
        Copy b5_c10 = new() { Id = guids["b5_c10"], Book = b5, BookId = guids["b5"] }; //Dziady część III
        modelBuilder.Entity<Copy>().HasData(b1_c1, b1_c2, b2_c3, b2_c4, b3_c5, b3_c5, b3_c6, b4_c7, b5_c9, b5_c10);

        Reader r1 = new() { Id = guids["r1"], FirstName = "Jan", LastName = "Kowalski", CardNumber = "000-111-222", Email = "jan.kowalski@mail.com", Phone = "+48 661 727 091"};
        Reader r2 = new() { Id = guids["r2"], FirstName = "Adam", LastName = "Nowak", CardNumber = "333-444-555", Email = "adam.nowak@mail.com", Phone = "+48 664 227 191" };
        modelBuilder.Entity<Reader>().HasData(r1, r2);

        Borrowing bor1 = new() { Id = guids["bor1"], Copy = b1_c1, CopyId = guids["b1_c1"], Reader = r1, ReaderId = guids["r1"], StartedDate = DateTime.Parse("2022-11-01") };
        Borrowing bor2 = new() { Id = guids["bor2"], Copy = b1_c2, CopyId = guids["b1_c2"], Reader = r1, ReaderId = guids["r1"], StartedDate = DateTime.Parse("2022-10-01") };
        Borrowing bor3 = new() { Id = guids["bor3"], Copy = b2_c3, CopyId = guids["b2_c3"], Reader = r2, ReaderId = guids["r2"], StartedDate = DateTime.Parse("2022-10-01") };
        Borrowing bor4 = new() { Id = guids["bor4"], Copy = b2_c4, CopyId = guids["b2_c4"], Reader = r1, ReaderId = guids["r1"], StartedDate = DateTime.Today };
        Borrowing bor5 = new() { Id = guids["bor5"], Copy = b3_c5, CopyId = guids["b3_c5"], Reader = r2, ReaderId = guids["r2"], StartedDate = DateTime.Today };
        modelBuilder.Entity<Borrowing>().HasData(bor1, bor2, bor3, bor4, bor5);
    }

    private void InitializeGuids()
    {
        guids["a1"] = Guid.NewGuid();
        guids["a2"] = Guid.NewGuid();

        guids["c1"] = Guid.NewGuid();
        guids["c2"] = Guid.NewGuid();

        guids["r1"] = Guid.NewGuid();
        guids["r2"] = Guid.NewGuid();

        guids["b1"] = Guid.NewGuid();
        guids["b2"] = Guid.NewGuid();
        guids["b3"] = Guid.NewGuid();
        guids["b4"] = Guid.NewGuid();
        guids["b5"] = Guid.NewGuid();

        guids["b1_c1"] = Guid.NewGuid();
        guids["b1_c2"] = Guid.NewGuid();
        guids["b2_c3"] = Guid.NewGuid();
        guids["b2_c4"] = Guid.NewGuid();
        guids["b3_c5"] = Guid.NewGuid();
        guids["b3_c6"] = Guid.NewGuid();
        guids["b4_c7"] = Guid.NewGuid();
        guids["b4_c8"] = Guid.NewGuid();
        guids["b5_c9"] = Guid.NewGuid();
        guids["b5_c10"] = Guid.NewGuid();

        guids["bor1"] = Guid.NewGuid();
        guids["bor2"] = Guid.NewGuid();
        guids["bor3"] = Guid.NewGuid();
        guids["bor4"] = Guid.NewGuid();
        guids["bor5"] = Guid.NewGuid();
    }
}