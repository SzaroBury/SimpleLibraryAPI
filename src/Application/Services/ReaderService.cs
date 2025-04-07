using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.DTO;

namespace SimpleLibrary.Application.Services;

public class ReaderService: IReaderService
{
    private readonly IUnitOfWork unitOfWork;

    public ReaderService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Reader>> GetAllReadersAsync()
    {
        return await unitOfWork.GetRepository<Reader>().GetAllAsync();
    }
    public async Task<Reader> GetReaderByIdAsync(string id)
    {
        var readerGuid = ValidateGuid(id);
        return await GetReaderByIdAsync(readerGuid);
    }
    public async Task<Reader> GetReaderByIdAsync(Guid id)
    {
        return await unitOfWork.GetRepository<Reader>().GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"A Reader with the specified ID ({id}) was not found in the system.");
    }
    public async Task<Reader> CreateReaderAsync(ReaderPostDTO reader)
    {
        Reader newReader = new() {
            Id = Guid.NewGuid(),
            FirstName = reader.FirstName,
            LastName = reader.LastName,
            CardNumber = "",
            Email = reader.Email,
            Phone = reader.Phone
        };

        await unitOfWork.GetRepository<Reader>().AddAsync(newReader);
        await unitOfWork.SaveChangesAsync();

        return newReader;
    }
    public async Task<Reader> UpdateReaderAsync(ReaderPutDTO reader)
    {
        Reader existingReader = await GetReaderByIdAsync(reader.Id);
        
        unitOfWork.GetRepository<Reader>().Update(existingReader);
        await unitOfWork.SaveChangesAsync();

        return existingReader;
    }
    public async Task DeleteReaderAsync(string id)
    {
        var reader = await GetReaderByIdAsync(id);

        await unitOfWork.GetRepository<Reader>().DeleteAsync(reader.Id);
        await unitOfWork.SaveChangesAsync();
    }
    public Task<IEnumerable<Reader>> SearchReadersAsync(
        string? searchTerm = null,
        string? copyId = null,
        bool? isBanned = null,
        int page = 1, 
        int pageSize = 25)
    {
        if(page < 1)
        {
            throw new ArgumentException($"Page ({page}) must be greater than zero.");
        }
        if(pageSize < 1)
        {
            throw new ArgumentException($"Size of a page ({pageSize}) must be greater than zero.");
        }

        var searchReadersQuery = unitOfWork.GetRepository<Reader>().GetQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            if(searchTerm.Length < 3)
            {
                throw new ArgumentException($"The searching term need to have at least three letters.");
            }
            searchReadersQuery = searchReadersQuery.Where(b =>
                b.FullName      .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.CardNumber .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            );
        }
        
        var count = searchReadersQuery.Count();
        if (count > 0  && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException("Invalid page. Not so many Readers.");
        }

        searchReadersQuery = searchReadersQuery.Skip((page - 1) * pageSize);
        searchReadersQuery = searchReadersQuery.Count() > pageSize ? searchReadersQuery.Take(pageSize) : searchReadersQuery;

        return Task.FromResult(searchReadersQuery.AsEnumerable());
    }

    private static Guid ValidateGuid(string id)
    {
        if(!Guid.TryParse(id, out var ReaderGuid))
        {
            throw new FormatException("Invalid Reader ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return ReaderGuid;
    }
}