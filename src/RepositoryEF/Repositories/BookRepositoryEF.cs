using Entities.Enumerations;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace RepositoryEF.Repositories
{
    public class BookRepositoryEF : IBookRepository
    {
        private readonly LibraryEFContext context;
        public BookRepositoryEF(LibraryEFContext libraryEFContext)
        {
            context = libraryEFContext;
        }

        public List<Book> GetBooks()
        {
            return context.Books.Include(b => b.Author).Include(b => b.Category).AsNoTracking().ToList();
        }

        public Book GetBook(int id)
        {
            var book = context.Books.Include(b => b.Author).Include(b => b.Category).AsNoTracking().FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                throw new KeyNotFoundException();
            }
            return book;
        }

        public void CreateBook(Book book)
        {
            if (context.Books.Any(b => b.Id == book.Id))
            {
                throw new ArgumentException();
            }
            context.Books.Add(book);
            context.SaveChanges();
        }

        public void UpdateBook(Book temp)
        {
            Book? book = context.Books.Find(temp.Id);
            if (book == null)
            {
                throw new KeyNotFoundException();
            }

            book.Title = string.IsNullOrEmpty(temp.Title) ? book.Title : temp.Title;
            book.ReleaseDate = !temp.ReleaseDate.HasValue ? book.ReleaseDate : temp.ReleaseDate.Value;
            book.Language = temp.Language == null ? book.Language : temp.Language;
            book.AuthorId = temp.AuthorId == 0 ? book.AuthorId : temp.AuthorId;
            book.CategoryId = temp.CategoryId == 0 ? book.CategoryId : temp.CategoryId;
            book.Description = string.IsNullOrEmpty(temp.Description) ? book.Description : temp.Description;
            book.Tags = string.IsNullOrEmpty(temp.Tags) ? book.Tags : temp.Tags;

            context.SaveChanges();
        }

        public void DeleteBook(int id)
        {
            context.Books.Remove(GetBook(id));
            context.SaveChanges();
        }
    }
}
