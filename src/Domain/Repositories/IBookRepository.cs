using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Domain.Repositories;

public interface IBookRepository
{
    public List<Book> GetAllBooks();
    public IQueryable<Book> GetBooks();
    public Book GetBook(int id);
    public void CreateBook(Book book);
    public void UpdateBook(Book book);
    public void DeleteBook(int id);
}
