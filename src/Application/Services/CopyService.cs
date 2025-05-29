using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.Enumerations;
using System.Globalization;
using SimpleLibrary.Application.Commands.Copies;

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
    public async Task<Copy> CreateCopyAsync(PostCopyCommand copy)
    {
        var book = await unitOfWork.GetRepository<Book>().GetByIdAsync(copy.BookId)
            ?? throw new KeyNotFoundException($"A book with  id '{copy.BookId}' was not found in the system.");

        var copyNumbers = unitOfWork.GetRepository<Copy>().GetQueryable().Where(c => c.BookId == book.Id).Select(c => c.CopyNumber);
        var maxCopyNumber = copyNumbers.Any() ? copyNumbers.Max() : 0;

        Copy newCopy = new() 
        {
            Book = book,
            BookId = book.Id,
            CopyNumber = maxCopyNumber + 1,
            ShelfNumber = copy.Shelf,
            AcquisitionDate = copy.AcquisitionDate ?? DateTime.Now,
            Condition = copy.Condition ?? CopyCondition.New,
            LastInspectionDate = copy.LastInspectionDate
        };

        await unitOfWork.GetRepository<Copy>().AddAsync(newCopy);
        await unitOfWork.SaveChangesAsync();

        return newCopy;
    }
    public async Task<Copy> UpdateCopyAsync(PatchCopyCommand copy)
    {
        Copy existingCopy = await GetCopyByIdAsync(copy.Id);

        if(copy.BookId.HasValue)
        {
            var book = await unitOfWork.GetRepository<Book>().GetByIdAsync(copy.BookId.Value)
                ?? throw new KeyNotFoundException($"A book with the specified ID ({copy.BookId}) was not found in the system.");
            existingCopy.BookId = book.Id;
        }

        if(copy.Shelf.HasValue)
        {
            if(copy.Shelf < 1)
            {
                throw new ArgumentException("Shelf number must be greater than zero.");
            }
            existingCopy.ShelfNumber = copy.Shelf.Value;
        }

        if(copy.IsLost.HasValue)
        {
            existingCopy.IsLost = copy.IsLost.Value;
        }

        if(copy.Condition is not null)
        {
            existingCopy.Condition = copy.Condition.Value;
        }

        if(copy.AcquisitionDate.HasValue)
        {
            existingCopy.AcquisitionDate = copy.AcquisitionDate.Value;
        }

        if(copy.LastInspectionDate.HasValue)
        {
            existingCopy.LastInspectionDate = copy.LastInspectionDate.Value;
        }

        if(copy.CopyNumber.HasValue)
        {
            var isTaken = unitOfWork.GetRepository<Copy>().GetQueryable()
                .Any(c => c.Id != existingCopy.Id 
                    && c.BookId == existingCopy.BookId 
                    && c.CopyNumber == copy.CopyNumber.Value);
                    
            if (isTaken)
            {
                throw new ArgumentException($"The specified copy number ({copy.CopyNumber.Value}) is already taken by some else copy of the book.");
            }

            existingCopy.CopyNumber = copy.CopyNumber.Value;
        }
        
        unitOfWork.GetRepository<Copy>().Update(existingCopy);
        await unitOfWork.SaveChangesAsync();

        return existingCopy;
    }
    public async Task DeleteCopyAsync(string id)
    {
        var copy = await GetCopyByIdAsync(id);

        var borrowings = unitOfWork
            .GetRepository<Borrowing>()
            .GetQueryable()
            .Where(b => b.CopyId == copy.Id);

        if(borrowings.Any(b => b.ActualReturnDate == null && !copy.IsLost))
        {
            throw new InvalidOperationException("You must not delete the copy, because it is still borrowed. If it is lost, mark it with IsLost flag with Update request.");
        }

        foreach(var borrowing in borrowings)
        {
            await unitOfWork.GetRepository<Borrowing>().DeleteAsync(borrowing.Id);
        }

        await unitOfWork.GetRepository<Copy>().DeleteAsync(copy.Id);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task<IEnumerable<Copy>> SearchCopiesAsync(
        string? searchTerm = null, 
        string? bookId = null,
        bool? isAvailable = null,
        int page = 1, 
        int pageSize = 25)
    {
        if(page < 1)
        {
            throw new ArgumentException($"Page must be greater than zero.");
        }
        if(pageSize < 1)
        {
            throw new ArgumentException($"Size of a page must be greater than zero.");
        }

        var searchCopiesQuery = unitOfWork.GetRepository<Copy>().GetQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            if(searchTerm.Length < 3)
            {
                throw new ArgumentException($"The searching term need to have at least three letters.");
            }
            searchCopiesQuery = searchCopiesQuery.Where(c =>
                c.Book.Title   .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || c.Book.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || c.Book.Tags.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || c.Book.Author.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || c.Book.Author.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || c.Book.Author.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || c.Book.Author.Tags.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            );
        }

        if(!string.IsNullOrEmpty(bookId))
        {
            var bookGuid = ValidateGuid(bookId, "book");
            var book = await unitOfWork.GetRepository<Book>().GetByIdAsync(bookGuid)
                ?? throw new KeyNotFoundException($"A book with the specified ID ({bookId}) was not found in the system.");

            searchCopiesQuery = searchCopiesQuery.Where(c => c.BookId == bookGuid);
        }

        if(isAvailable.HasValue)
        {
            if(isAvailable.Value)
            {
                searchCopiesQuery = searchCopiesQuery.Where(c => !c.IsLost && !c.Borrowings.Any(b => !b.ActualReturnDate.HasValue));
            }
            else
            {
                searchCopiesQuery = searchCopiesQuery.Where(c => c.IsLost || c.Borrowings.Any(b => !b.ActualReturnDate.HasValue));
            }
        }

        var count = searchCopiesQuery.Count();
        if (count > 0  && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException("Invalid page. Not so many copies.");
        }

        searchCopiesQuery = searchCopiesQuery.Skip((page - 1) * pageSize);
        searchCopiesQuery = searchCopiesQuery.Count() > pageSize ? searchCopiesQuery.Take(pageSize) : searchCopiesQuery;

        return await Task.FromResult(searchCopiesQuery.AsEnumerable());
    }

    private static Guid ValidateGuid(string id, string entity = "copy")
    {
        if(!Guid.TryParse(id, out var guid))
        {
            throw new FormatException($"Invalid {entity} ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return guid;
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