using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.Application.Services;

public class AuthorService: IAuthorService
{
    private readonly IRepository<Author> authorRepository;

    public AuthorService(IRepository<Author> authorRepository)
    {
        this.authorRepository = authorRepository;
    }

    public async Task<List<Author>> GetAllAuthorsAsync()
    {
        return await authorRepository.GetAllAsync();
    }

    public async Task<Author> GetAuthorByIdAsync(int id)
    {
        var result = await authorRepository.GetByIdAsync(id);
        if(result is null)
        {
            throw new KeyNotFoundException($"Author with the given id ({id}) is not found in database.");
        }

        return result;
    }

    public Task<Author> CreateAuthorAsync(Author author)
    {
        throw new NotImplementedException();
    }

    public Task<Author> UpdateAuthorAsync(Author author)
    {
        throw new NotImplementedException();
    }
    public Task DeleteAuthorAsync(int id)
    {
        throw new NotImplementedException();
    }

    public List<Author> SearchAuthorsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? youngerThan = null, 
        int page = 1, 
        int pageSize = 25)
    {
        throw new NotImplementedException();
    }
}