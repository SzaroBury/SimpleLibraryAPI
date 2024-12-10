using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace RepositoryEF
{
    public class LibraryEFContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Copy> Copies { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }

        public LibraryEFContext(DbContextOptions<LibraryEFContext> options) :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = @"N/A", LastName = @"N/A", BornDate = null},
                new Author { Id = 2, FirstName = @"Adam", LastName = @"Mickiewicz", BornDate = new DateTime(1798, 12, 24) });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Novel" },
                new Category { Id = 2, Name = "Other" });

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Some old book",         AuthorId = 1, CategoryId = 1, Language = Entities.Enumerations.Language.English,    ReleaseDate = new DateTime(1900, 1, 1) },
                new Book { Id = 2, Title = "Some old German book",  AuthorId = 1, CategoryId = 1, Language = Entities.Enumerations.Language.German,     ReleaseDate = new DateTime(1800, 1, 1) },
                new Book { Id = 3, Title = "Some new French book",  AuthorId = 1, CategoryId = 2, Language = Entities.Enumerations.Language.French,     ReleaseDate = new DateTime(2010, 5, 7) },
                new Book { Id = 4, Title = "Dziady część II",       AuthorId = 2, CategoryId = 2, Language = Entities.Enumerations.Language.Polish,     ReleaseDate = new DateTime(1823, 1, 1) },
                new Book { Id = 5, Title = "Dziady część III",      AuthorId = 2, CategoryId = 2, Language = Entities.Enumerations.Language.Polish,     ReleaseDate = new DateTime(1832, 1, 1) });

            modelBuilder.Entity<Copy>().HasData(
                new Copy { Id = 1, BookId = 1 }, //Some old book
                new Copy { Id = 2, BookId = 1 }, //Some old book
                new Copy { Id = 3, BookId = 2 }, //Some old German book
                new Copy { Id = 4, BookId = 2 }, //Some old German book
                new Copy { Id = 5, BookId = 3 }, //Some new French book
                new Copy { Id = 6, BookId = 3 }, //Some new French book
                new Copy { Id = 7, BookId = 4 }, //Dziady część II
                new Copy { Id = 8, BookId = 4 }, //Dziady część II
                new Copy { Id = 9, BookId = 5 }, //Dziady część III
                new Copy { Id = 10, BookId = 5 } //Dziady część III
                );

            modelBuilder.Entity<Reader>().HasData(
                new Reader { Id = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@mail.com", Phone = "+48 661 727 091"},
                new Reader { Id = 2, FirstName = "Adam", LastName = "Nowak", Email = "adam.nowak@mail.com", Phone = "+48 664 227 191" }
                );

            modelBuilder.Entity<Borrowing>().HasData(
                new Borrowing { Id = 1, CopyId = 1, ReaderId = 1, StartedDate = DateTime.Parse("2022-11-01") },
                new Borrowing { Id = 2, CopyId = 2, ReaderId = 1, StartedDate = DateTime.Parse("2022-10-01") },
                new Borrowing { Id = 3, CopyId = 3, ReaderId = 2, StartedDate = DateTime.Parse("2022-10-01") },
                new Borrowing { Id = 4, CopyId = 4, ReaderId = 1, StartedDate = DateTime.Today },
                new Borrowing { Id = 5, CopyId = 5, ReaderId = 2, StartedDate = DateTime.Today}
                );
        }
    }
}