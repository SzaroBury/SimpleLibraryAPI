using SimpleLibrary.Application.Commands.Authors;
using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IAuthorService
{
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<Author> GetAuthorByIdAsync(string id);
    Task<Author> GetAuthorByIdAsync(Guid id);
    Task<Author> CreateAuthorAsync(PostAuthorCommand author);
    Task<Author> UpdateAuthorAsync(PatchAuthorCommand author);
    Task DeleteAuthorAsync(string id);
    Task<IEnumerable<Author>> SearchAuthorsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? youngerThan = null, 
        int page = 1, 
        int pageSize = 25);
}