using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.DTO;

namespace SimpleLibrary.Application.Services;

public class AuthorService: IAuthorService
{
    private readonly IRepository<Author> authorRepository;
    private readonly IRepository<Book> bookRepository;

    public AuthorService(IRepository<Author> authorRepository, IRepository<Book> bookRepository)
    {
        this.authorRepository = authorRepository;
        this.bookRepository = bookRepository;
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
    public async Task<Author> CreateAuthorAsync(AuthorPostDTO author)
    {
        if(string.IsNullOrEmpty(author.FirstName) || string.IsNullOrEmpty(author.LastName))
        {
            throw new ArgumentException("The author's first name and last name cannot be left empty.");
        }
        DateTime? authorsBornDate = null;
        if(string.IsNullOrEmpty(author.BornDate))
        {
            if(!DateTime.TryParse(author.BornDate, out DateTime notNullAuthorsBornDate))
            {
                throw new FormatException("Invalid date format. Please use the following format: YYYY-MM-DD");
            }
            authorsBornDate = notNullAuthorsBornDate;
        }
        if(author.Tags.Any(tag => tag.Contains(',')))
        {
            throw new FormatException("Invalid tags format. Please do not use commas in tags.");
        }

        var tagsInString = author.Tags.ToList().Count > 1 
            ? string.Join(',', author.Tags) 
            : author.Tags.First() ?? "";

        Author newAuthor = new() 
        {
            FirstName = author.FirstName,
            LastName = author.LastName,
            Description = author.Description,
            BornDate = authorsBornDate,
            Tags = tagsInString
        };
        await authorRepository.AddAsync(newAuthor);

        return newAuthor;
    }
    public async Task<Author> UpdateAuthorAsync(string id, AuthorPutDTO author)
    {
        Author existingAuthor = await GetAuthorByIdAsync(id);

        if(author.FirstName is not null)
        {
            if(string.IsNullOrWhiteSpace(author.FirstName))
            {
                throw new ArgumentException("The author's first name cannot be left empty.");
            }
            existingAuthor.FirstName = author.FirstName;
        }
        if(author.LastName is not null)
        {
            if(string.IsNullOrWhiteSpace(author.LastName))
            {
                throw new ArgumentException("The author's last name cannot be left empty.");
            }
            existingAuthor.LastName = author.LastName;
        }
        if(author.Description is not null)
        {
            existingAuthor.Description = author.Description;
        }

        if(author.BornDate is not null)
        {
            DateTime? authorsBornDate = null;
            if(!string.IsNullOrWhiteSpace(author.BornDate))
            {
                if(!DateTime.TryParse(author.BornDate, out DateTime notNullAuthorsBornDate))
                {
                    throw new FormatException("Invalid date format. Please use the following format: YYYY-MM-DD");
                }
                authorsBornDate = notNullAuthorsBornDate;
            }
        }
        
        if(author.Tags is not null)
        {
            if(author.Tags.Any(tag => tag.Contains(',')))
            {
                throw new FormatException("Invalid tags format. Please do not use commas in tags.");
            }
            var tagsInString = author.Tags.ToList().Count > 1 
                ? string.Join(',', author.Tags) 
                : author.Tags.First() ?? "";
            existingAuthor.Tags = tagsInString;
        }
        
        await authorRepository.UpdateAsync(existingAuthor);
        return existingAuthor;
    }
    public async Task DeleteAuthorAsync(string id)
    {
        var author = await GetAuthorByIdAsync(id);

        var books = bookRepository.GetQueryable().Where(b => b.AuthorId == author.Id).ToList();

        if(books.Count > 0)
        {
            string joinedBooks = string.Join(", ", books.Select(b => b.Title));
            throw new InvalidOperationException($"The author cannot be deleted as there are still books associated with this author in the system: {joinedBooks}.");
        }
        await authorRepository.DeleteAsync(author.Id);
    }
    public Task<IEnumerable<Author>> SearchAuthorsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? youngerThan = null, 
        int page = 1, 
        int pageSize = 25)
    {
        if(page <= 0 || pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException($"Page ({page}) must be a positive number.");
        }
        if(pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException($"Size of a page ({pageSize}) must be a positive number.");
        }

        var searchAuthorsQuery = authorRepository.GetQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {       
            searchAuthorsQuery = searchAuthorsQuery.Where(a =>
                a.FirstName     .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || a.LastName   .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || a.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || a.Tags       .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            );
        }

        if (!string.IsNullOrEmpty(olderThan))
        {
            if (!DateTime.TryParse(olderThan, out DateTime olderThanDate))
            {
                throw new FormatException("Invalid date format of olderThan parameter. Please use the following format: YYYY-MM-DD");
            }
            searchAuthorsQuery = searchAuthorsQuery.Where(b => b.BornDate < olderThanDate);
        }

        if (!string.IsNullOrEmpty(youngerThan))
        {
            if(!DateTime.TryParse(youngerThan, out DateTime youngerThanDate))
            {
                throw new FormatException("Invalid date format of newerThan parameter. Please use the following format: YYYY-MM-DD");
            }
            searchAuthorsQuery = searchAuthorsQuery.Where(b => b.BornDate > youngerThanDate);
        }
        
        var count = searchAuthorsQuery.Count();
        if (count > 0  && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException("Invalid pgae. Not so many authors.");
        }

        searchAuthorsQuery = searchAuthorsQuery.Skip((page - 1) * pageSize);
        searchAuthorsQuery = searchAuthorsQuery.Count() > pageSize ? searchAuthorsQuery.Take(pageSize) : searchAuthorsQuery;

        return Task.FromResult(searchAuthorsQuery.AsEnumerable());
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