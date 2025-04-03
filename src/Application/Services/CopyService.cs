using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.Application.Services;

public class CopyService: ICopyService
{
    private readonly IUnitOfWork unitOfWork;

    public CopyService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Copy>> GetAllCopiesAsync()
    {
        return await unitOfWork.GetRepository<Copy>().GetAllAsync();
    }
    public async Task<Copy> GetCopyByIdAsync(string id)
    {
        var CopyGuid = ValidateGuid(id);
        return await GetCopyByIdAsync(CopyGuid);
    }
    public async Task<Copy> GetCopyByIdAsync(Guid id)
    {
        return await unitOfWork.GetRepository<Copy>().GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"A copy with the specified ID ({id}) was not found in the system.");
    }
    public async Task<Copy> CreateCopyAsync(Copy copy)
    {
        await unitOfWork.GetRepository<Copy>().AddAsync(copy);
        await unitOfWork.SaveChangesAsync();

        return copy;
    }
    public async Task<Copy> UpdateCopyAsync(Copy copy)
    {
        Copy existingCopy = await GetCopyByIdAsync(copy.Id);
        
        unitOfWork.GetRepository<Copy>().Update(existingCopy);
        await unitOfWork.SaveChangesAsync();

        return existingCopy;
    }
    public async Task DeleteCopyAsync(string id)
    {
        var copy = await GetCopyByIdAsync(id);

        await unitOfWork.GetRepository<Copy>().DeleteAsync(copy.Id);
        await unitOfWork.SaveChangesAsync();
    }
    public Task<IEnumerable<Copy>> SearchCopiesAsync(
        string? searchTerm = null, 
        int? bookId = null,
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

        var searchCopysQuery = unitOfWork.GetRepository<Copy>().GetQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            if(searchTerm.Length < 3)
            {
                throw new ArgumentException($"The searching term need to have at least three letters.");
            }
            searchCopysQuery = searchCopysQuery.Where(c =>
                c.Book.Title   .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || c.Book.Author.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            );
        }
        
        var count = searchCopysQuery.Count();
        if (count > 0  && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException("Invalid page. Not so many Copys.");
        }

        searchCopysQuery = searchCopysQuery.Skip((page - 1) * pageSize);
        searchCopysQuery = searchCopysQuery.Count() > pageSize ? searchCopysQuery.Take(pageSize) : searchCopysQuery;

        return Task.FromResult(searchCopysQuery.AsEnumerable());
    }

    private static Guid ValidateGuid(string id)
    {
        if(!Guid.TryParse(id, out var CopyGuid))
        {
            throw new FormatException("Invalid Copy ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return CopyGuid;
    }
}