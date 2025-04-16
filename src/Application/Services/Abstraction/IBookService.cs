using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.DTO;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IBookService
{
    public Task<IEnumerable<Book>> GetAllBooksAsync();
    public Task<Book> GetBookByIdAsync(string id);
    public Task<Book> GetBookByIdAsync(Guid id);
    public Task<Book> CreateBookAsync(BookPostDTO book);
    public Task<Book> UpdateBookAsync(BookPatchDTO book);
    public Task DeleteBookAsync(string id);
    public Task<IEnumerable<Book>> SearchBooksAsync(
        string? searchTerm = null, 
        bool? isAvailable = null, 
        string? olderThan = null, 
        string? newerThan = null, 
        string? authorId = null, 
        string? categoryId = null, 
        int page = 1, 
        int pageSize = 25
    );
}
