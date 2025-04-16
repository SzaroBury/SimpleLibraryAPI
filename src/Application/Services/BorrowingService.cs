using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.DTO;
using System.Globalization;

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
        var borrowingGuid = ValidateGuid(id);
        return await GetBorrowingByIdAsync(borrowingGuid);
    }
    public async Task<Borrowing> GetBorrowingByIdAsync(Guid id)
    {
        return await unitOfWork.GetRepository<Borrowing>().GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"A borrowing with the specified ID ({id}) was not found in the system.");
    }
    public async Task<Borrowing> CreateBorrowingAsync(BorrowingPostDTO borrowing)
    {
        DateTime startedDate = DateTime.Now;
        if(!string.IsNullOrWhiteSpace(borrowing.StartedDate))
        {
            startedDate = ValidateAndParseDateTime(borrowing.StartedDate, "started");
            if(startedDate > DateTime.Now)
            {
                throw new InvalidOperationException("The start date cannot be set to a future date.");
            }
        }

        DateTime? actualReturnDate = null;
        if(!string.IsNullOrWhiteSpace(borrowing.ActualReturnDate))
        {
            actualReturnDate = ValidateAndParseDateTime(borrowing.ActualReturnDate, "actual return"); 
            if(actualReturnDate < startedDate)
            {
                throw new InvalidOperationException($"The start date {borrowing.StartedDate} must not be set after the actual return date {borrowing.ActualReturnDate}.");
            }
        }

        var copy = await ValidateCopyAsync(borrowing.CopyId);

        var reader = await ValidateReaderAsync(borrowing.ReaderId);

        Borrowing newBorrowing = new()
        {
            StartedDate = startedDate,
            ActualReturnDate = actualReturnDate,
            ReaderId = reader.Id,
            Reader = reader,
            CopyId = copy.Id,
            Copy = copy,
        };

        await unitOfWork.GetRepository<Borrowing>().AddAsync(newBorrowing);
        await unitOfWork.SaveChangesAsync();

        return newBorrowing;
    }
    public async Task<Borrowing> UpdateBorrowingAsync(BorrowingPatchDTO borrowing)
    {
        Borrowing existingBorrowing = await GetBorrowingByIdAsync(borrowing.Id);

        if(borrowing.StartedDate is not null)
        {
            if(string.IsNullOrWhiteSpace(borrowing.StartedDate))
            {
                throw new ArgumentException("The start date cannot be empty.");
            }
            DateTime startedDate = ValidateAndParseDateTime(borrowing.StartedDate, "started");
            if(startedDate > DateTime.Now)
            {
                throw new InvalidOperationException("The start date cannot be set to a future date.");
            }
            existingBorrowing.StartedDate = startedDate;
        }

        if(borrowing.ActualReturnDate is not null)
        {
            DateTime? actualReturnDate = null;
            if(!string.IsNullOrWhiteSpace(borrowing.ActualReturnDate))
            {
                actualReturnDate = ValidateAndParseDateTime(borrowing.ActualReturnDate, "actual return"); 
                if(actualReturnDate > DateTime.Now)
                {
                    throw new InvalidOperationException("The actual return date cannot be set to a future date.");
                }
                if(actualReturnDate < existingBorrowing.StartedDate)
                {
                    throw new InvalidOperationException($"The start date {borrowing.StartedDate} must not be set after the actual return date {borrowing.ActualReturnDate}.");
                }
            }
            existingBorrowing.ActualReturnDate = actualReturnDate;
        }

        if(borrowing.CopyId is not null)
        {
            var copy = await ValidateCopyAsync(borrowing.CopyId);
            existingBorrowing.Copy = copy;
            existingBorrowing.CopyId = copy.Id;
        }

        if(borrowing.ReaderId is not null)
        {
            var reader = await ValidateReaderAsync(borrowing.ReaderId);
            existingBorrowing.Reader = reader;
            existingBorrowing.ReaderId = reader.Id;
        }
        
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
    public async Task<IEnumerable<Borrowing>> SearchBorrowingsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? newerThan = null, 
        string? copyId = null,
        string? readerId = null,
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

        if(olderThan is not null)
        {
            var olderThanDate = ValidateAndParseDateTime(olderThan, "olderThan");
            searchBorrowingsQuery = searchBorrowingsQuery.Where(b => b.StartedDate < olderThanDate); 
        }

        if(newerThan is not null)
        {
            var newerThanDate = ValidateAndParseDateTime(newerThan, "newerThan");
            searchBorrowingsQuery = searchBorrowingsQuery.Where(b => b.StartedDate > newerThanDate);
        }

        if(copyId is not null)
        {
            var copyGuid = ValidateGuid(copyId, "copy");
            var copy = await unitOfWork.GetRepository<Copy>().GetByIdAsync(copyGuid)
                ?? throw new KeyNotFoundException($"A copy with the specified ID ({copyId}) was not found in the system.");

            searchBorrowingsQuery = searchBorrowingsQuery.Where(b => b.CopyId == copyGuid); 
        }

        if (readerId is not null)
        {
            var readerGuid = ValidateGuid(readerId, "reader");
            var reader = await unitOfWork.GetRepository<Reader>().GetByIdAsync(readerGuid)
                ?? throw new KeyNotFoundException($"A reader with the specified ID ({readerId}) was not found in the system.");

            searchBorrowingsQuery = searchBorrowingsQuery.Where(b => b.ReaderId == readerGuid); 
        }
        
        var count = searchBorrowingsQuery.Count();
        if (count > 0  && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException("Invalid page. Not so many borrowings.");
        }
        searchBorrowingsQuery = searchBorrowingsQuery.Skip((page - 1) * pageSize).Take(pageSize);

        return searchBorrowingsQuery.AsEnumerable();
    }

    private async Task<Copy> ValidateCopyAsync(string copyId)
    {
        var copyGuid = ValidateGuid(copyId, "copy");
        var copy = await unitOfWork.GetRepository<Copy>().GetByIdAsync(copyGuid)
            ?? throw new KeyNotFoundException($"A copy with the specified ID ({copyId}) was not found in the system.");
        
        if (copy.IsLost)
            throw new InvalidOperationException($"The specified copy ({copy.Id}) is marked as lost and cannot be borrowed.");
        
        if (copy.Borrowings.Any(b => b.ActualReturnDate == null))
            throw new InvalidOperationException($"The specified copy ({copy.Id}) is currently borrowed and cannot be borrowed again until it is returned.");

        return copy;
    }

    private async Task<Reader> ValidateReaderAsync(string readerId)
    {
        var readerGuid = ValidateGuid(readerId, "reader");
        var reader = await unitOfWork.GetRepository<Reader>().GetByIdAsync(readerGuid)
            ?? throw new KeyNotFoundException($"A reader with the specified ID ({readerId}) was not found in the system.");

        if (reader.IsBanned)
            throw new InvalidOperationException($"The specified reader ({reader.Id}) is banned and cannot borrow more books.");

        return reader;
    }

    private static Guid ValidateGuid(string id, string entity = "borrowing")
    {
        if(!Guid.TryParse(id, out var borrowingGuid))
        {
            throw new FormatException($"Invalid {entity} ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return borrowingGuid;
    }

    private static DateTime ValidateAndParseDateTime(string date, string propertyName)
    {
        string[] AcceptedFormats = [ "yyyy-MM-dd HH:mm", "yyyy-MM-dd" ];
        if (!DateTime.TryParseExact(date, AcceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            throw new FormatException($"'{date}' is invalid {propertyName} date format. Please use one of the formats: 'dd-MM-yyyy' or 'dd-MM-yyyy HH:mm'.");
        }

        return result;
    }
}