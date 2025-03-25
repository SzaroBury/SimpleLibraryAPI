using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IAuthorService
{
    public Task<List<Author>> GetAllAuthors();
    public Task<Author> GetAuthorById(int id);
    public Task<Author> CreateAuthorAsync(Author author);
    public Task<Author> UpdateAuthorAsync(Author author);
    public Task DeleteAuthorAsync(int id);
    public List<Author> SearchAuthorsAsync(string? searchTerm = null, string? olderThan = null, string? youngerThan = null, int page = 1, int pageSize = 25);
}