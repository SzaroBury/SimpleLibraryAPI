using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.Application.Services;

public class BorrowingService: IBorrowingService
{
    private readonly IUnitOfWork unitOfWork;

    public BorrowingService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Borrowing>> GetAllBorrowingsAsync()
    {
        return await unitOfWork.GetRepository<Borrowing>().GetAllAsync();
    }
    public async Task<Borrowing> GetBorrowingByIdAsync(string id)
    {
        var BorrowingGuid = ValidateGuid(id);
        return await GetBorrowingByIdAsync(BorrowingGuid);
    }
    public async Task<Borrowing> GetBorrowingByIdAsync(Guid id)
    {
        return await unitOfWork.GetRepository<Borrowing>().GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"A borrowing with the specified ID ({id}) was not found in the system.");
    }
    public async Task<Borrowing> CreateBorrowingAsync(Borrowing borrowing)
    {
        await unitOfWork.GetRepository<Borrowing>().AddAsync(borrowing);
        await unitOfWork.SaveChangesAsync();

        return borrowing;
    }
    public async Task<Borrowing> UpdateBorrowingAsync(Borrowing Borrowing)
    {
        Borrowing existingBorrowing = await GetBorrowingByIdAsync(Borrowing.Id);
        
        unitOfWork.GetRepository<Borrowing>().Update(existingBorrowing);
        await unitOfWork.SaveChangesAsync();

        return existingBorrowing;
    }
    public async Task DeleteBorrowingAsync(string id)
    {
        var borrowing = await GetBorrowingByIdAsync(id);

        await unitOfWork.GetRepository<Borrowing>().DeleteAsync(borrowing.Id);
        await unitOfWork.SaveChangesAsync();
    }
    public Task<IEnumerable<Borrowing>> SearchBorrowingsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? newerThan = null, 
        int? copyId = null,
        int? readerId = null,
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

        var searchBorrowingsQuery = unitOfWork.GetRepository<Borrowing>().GetQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            if(searchTerm.Length < 3)
            {
                throw new ArgumentException($"The searching term need to have at least three letters.");
            }
            searchBorrowingsQuery = searchBorrowingsQuery.Where(b =>
                b.Copy.Book.Title   .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Reader.FullName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            );
        }
        
        var count = searchBorrowingsQuery.Count();
        if (count > 0  && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException("Invalid page. Not so many Borrowings.");
        }

        searchBorrowingsQuery = searchBorrowingsQuery.Skip((page - 1) * pageSize);
        searchBorrowingsQuery = searchBorrowingsQuery.Count() > pageSize ? searchBorrowingsQuery.Take(pageSize) : searchBorrowingsQuery;

        return Task.FromResult(searchBorrowingsQuery.AsEnumerable());
    }

    private static Guid ValidateGuid(string id)
    {
        if(!Guid.TryParse(id, out var BorrowingGuid))
        {
            throw new FormatException("Invalid borrowing ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return BorrowingGuid;
    }
}