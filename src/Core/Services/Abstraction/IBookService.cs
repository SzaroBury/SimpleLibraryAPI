using Entities.Models;
using Entities.DTO;

namespace Core.Services.Abstraction;

public interface IBookService
{
    public List<Book> SearchBooks(string? searchTerm = null, bool? isAvailable = null, string? olderThan = null, string? newerThan = null, int? author = null, int? category = null, int page = 1, int pageSize = 25);
    public Book GetBookById(int id);
    public Book CreateBook(BookPostDTO book);
    public Book UpdateBook(BookPutDTO book);
    public void DeleteBook(int id);
}
