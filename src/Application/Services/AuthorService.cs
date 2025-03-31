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

    public async Task<Author> GetAuthorByIdAsync(string id)
    {
        var authorGuid = ValidateGuid(id);
        return await GetAuthorByIdAsync(authorGuid);
    }

    public async Task<Author> GetAuthorByIdAsync(Guid id)
    {
        return await authorRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"An author with the specified id ({id}) was not found in the system");
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
    public Task DeleteAuthorAsync(string id)
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
    }

    private static Guid ValidateGuid(string id)
    {
        if(!Guid.TryParse(id, out var authorGuid))
        {
            throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return authorGuid;
    }
}