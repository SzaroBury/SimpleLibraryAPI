using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Services.Abstraction;
using System.Globalization;
using SimpleLibrary.Application.Commands.Borrowings;

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
    public async Task<Borrowing> CreateBorrowingAsync(PostBorrowingCommand borrowing)
    {
        var startedDate = borrowing.StartedDate ?? DateTime.Now;
        if(borrowing.StartedDate.HasValue)
        {
            if(borrowing.StartedDate > DateTime.Now)
            {
                throw new InvalidOperationException("The start date cannot be set to a future date.");
            }
        }

        DateTime? actualReturnDate = null;
        if(borrowing.ActualReturnDate.HasValue)
        {
            actualReturnDate = borrowing.ActualReturnDate.Value;
            if (actualReturnDate < startedDate)
            {
                throw new InvalidOperationException($"The start date {borrowing.StartedDate} must not be set after the actual return date {borrowing.ActualReturnDate}.");
            }
        }

        var copy = await GetAndValidateCopyAsync(borrowing.CopyId);
        var reader = await GetAndValidateReaderAsync(borrowing.ReaderId);

        Borrowing newBorrowing = new()
        {
            StartedDate = startedDate,
            ActualReturnDate = actualReturnDate,
            ReaderId = reader.Id,
            Reader = reader,
            CopyId = borrowing.CopyId,
            Copy = copy,
        };

        await unitOfWork.GetRepository<Borrowing>().AddAsync(newBorrowing);
        await unitOfWork.SaveChangesAsync();

        return newBorrowing;
    }
    public async Task<Borrowing> UpdateBorrowingAsync(PatchBorrowingCommand borrowing)
    {
        Borrowing existingBorrowing = await GetBorrowingByIdAsync(borrowing.Id);

        if(borrowing.StartedDate.HasValue)
        {
            if(borrowing.StartedDate > DateTime.Now)
            {
                throw new InvalidOperationException("The start date cannot be set to a future date.");
            }
            existingBorrowing.StartedDate = borrowing.StartedDate.Value;
        }

        if(borrowing.ActualReturnDate.HasValue)
        {
            if(borrowing.ActualReturnDate > DateTime.Now)
            {
                throw new InvalidOperationException("The actual return date cannot be set to a future date.");
            }
            if(borrowing.ActualReturnDate < existingBorrowing.StartedDate)
            {
                throw new InvalidOperationException($"The start date {borrowing.StartedDate} must not be set after the actual return date {borrowing.ActualReturnDate}.");
            }
        }
        existingBorrowing.ActualReturnDate = borrowing.ActualReturnDate;

        if(borrowing.CopyId.HasValue)
        {
            var copy = await GetAndValidateCopyAsync(borrowing.CopyId.Value);
            existingBorrowing.Copy = copy;
            existingBorrowing.CopyId = copy.Id;
        }

        if(borrowing.ReaderId.HasValue)
        {
            var reader = await GetAndValidateReaderAsync(borrowing.ReaderId.Value);
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

    private async Task<Copy> GetAndValidateCopyAsync(Guid copyId)
    {
        var copy = await unitOfWork.GetRepository<Copy>().GetByIdAsync(copyId)
            ?? throw new KeyNotFoundException($"A copy with the specified ID ({copyId}) was not found in the system.");
        
        if (copy.IsLost)
            throw new InvalidOperationException($"The specified copy ({copy.Id}) is marked as lost and cannot be borrowed.");
        
        if (copy.Borrowings.Any(b => b.ActualReturnDate == null))
            throw new InvalidOperationException($"The specified copy ({copy.Id}) is currently borrowed and cannot be borrowed again until it is returned.");

        return copy;
    }

    private async Task<Reader> GetAndValidateReaderAsync(Guid readerId)
    {
        var reader = await unitOfWork.GetRepository<Reader>().GetByIdAsync(readerId)
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