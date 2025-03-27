using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SimpleLibrary.Infrastructure.Repositories;

public class BookRepositoryEF : IBookRepository
{
    private readonly LibraryEFContext context;
    public BookRepositoryEF(LibraryEFContext libraryEFContext)
    {
        context = libraryEFContext;
    }

    public List<Book> GetAllBooks()
    {
        return context.Books.Include(b => b.Author).Include(b => b.Category).AsNoTracking().ToList();
    }

    public IQueryable<Book> GetBooks()
    {
        return context.Books.AsQueryable();
    }

    public Book? GetBook(Guid id)
    {
        var result = context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .AsNoTracking()
            .FirstOrDefault(b => b.Id == id);
            
        return result;
    }

    public void CreateBook(Book book)
    {
        context.Books.Add(book);
        context.SaveChanges();
    }

    public void UpdateBook(Book book)
    {
        context.Update(book);
        context.SaveChanges();
    }

    public void DeleteBook(Guid id)
    {
        context.Books.Remove(GetBook(id));
        context.SaveChanges();
    }
}
