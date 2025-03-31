using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IAuthorService
{
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<Author> GetAuthorByIdAsync(string id);
    Task<Author> GetAuthorByIdAsync(Guid id);
    Task<Author> CreateAuthorAsync(AuthorPostDTO author);
    Task<Author> UpdateAuthorAsync(string id, AuthorPutDTO author);
    Task DeleteAuthorAsync(string id);
    Task<IEnumerable<Author>> SearchAuthorsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? youngerThan = null, 
        int page = 1, 
        int pageSize = 25);
}