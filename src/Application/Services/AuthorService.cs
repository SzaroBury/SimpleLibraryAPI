using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.DTO;

namespace SimpleLibrary.Application.Services;

public class AuthorService: IAuthorService
{
    private readonly IRepository<Author> authorRepository;

    public AuthorService(IRepository<Author> authorRepository)
    {
        this.authorRepository = authorRepository;
    }

    public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
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

    public async Task<Author> CreateAuthorAsync(AuthorPostDTO authorDTO)
    {
        if(string.IsNullOrEmpty(authorDTO.FirstName) || string.IsNullOrEmpty(authorDTO.LastName))
        {
            throw new ArgumentException("The author's first name and last name cannot be left empty.");
        }
        DateTime? authorsBornDate = null;
        if(string.IsNullOrEmpty(authorDTO.BornDate))
        {
            if(!DateTime.TryParse(authorDTO.BornDate, out DateTime notNullAuthorsBornDate))
            {
                throw new FormatException("Invalid date format. Please use the following format: YYYY-MM-DD");
            }
            authorsBornDate = notNullAuthorsBornDate;
        }
        if(authorDTO.Tags.Any(tag => tag.Contains(',')))
        {
            throw new FormatException("Invalid tags format. Please do not use commas in tags.");
        }

        var tagsInString = authorDTO.Tags.ToList().Count > 1 
            ? string.Join(',', authorDTO.Tags) 
            : authorDTO.Tags.First() ?? "";

        Author newAuthor = new() 
        {
            FirstName = authorDTO.FirstName,
            LastName = authorDTO.LastName,
            Description = authorDTO.Description,
            BornDate = authorsBornDate,
            Tags = tagsInString
        };
        await authorRepository.AddAsync(newAuthor);

        return newAuthor;
    }

    public Task<Author> UpdateAuthorAsync(Author author)
    {
        throw new NotImplementedException();
    }
    public Task DeleteAuthorAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Author>> SearchAuthorsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? youngerThan = null, 
        int page = 1, 
        int pageSize = 25)
    {
        throw new NotImplementedException();
    }
}