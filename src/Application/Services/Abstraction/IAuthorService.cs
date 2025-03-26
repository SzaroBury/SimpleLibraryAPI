using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IAuthorService
{
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<Author> GetAuthorByIdAsync(int id);
    Task<Author> CreateAuthorAsync(Author author);
    Task<Author> UpdateAuthorAsync(Author author);
    Task DeleteAuthorAsync(int id);
    Task<IEnumerable<Author>> SearchAuthorsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? youngerThan = null, 
        int page = 1, 
        int pageSize = 25);
}