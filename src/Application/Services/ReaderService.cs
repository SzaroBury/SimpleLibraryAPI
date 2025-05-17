using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Application.Commands.Readers;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace SimpleLibrary.Application.Services;

public class ReaderService: IReaderService
{
    private readonly IUnitOfWork uow;

    public ReaderService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public async Task<IEnumerable<Reader>> GetAllReadersAsync()
    {
        return await uow.GetRepository<Reader>().GetAllAsync();
    }
    public async Task<Reader> GetReaderByIdAsync(string id)
    {
        var readerGuid = ValidateGuid(id);
        return await GetReaderByIdAsync(readerGuid);
    }
    public async Task<Reader> GetReaderByIdAsync(Guid id)
    {
        return await uow.GetRepository<Reader>().GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"A reader with the specified ID ({id}) was not found in the system.");
    }
    public async Task<Reader> CreateReaderAsync(PostReaderCommand reader)
    {
        if(string.IsNullOrEmpty(reader.FirstName))
        {
            throw new ArgumentException("A reader's first name must not be left blank.");
        }

        if(string.IsNullOrEmpty(reader.LastName))
        {
            throw new ArgumentException("A reader's last name must not be left blank.");
        }

        if(!string.IsNullOrWhiteSpace(reader.Email))
        {
            if(!IsValidEmail(reader.Email))
            {
                throw new FormatException("Invalid email format.");
            }

            var isEmailInUse = uow.GetRepository<Reader>().GetQueryable().Any(r => r.Email == reader.Email);
            if(isEmailInUse)
            {
                throw new InvalidOperationException($"The specified email address ({reader.Email} is already in use.");
            }
        }

        if(!string.IsNullOrEmpty(reader.Phone))
        {
            if(!IsValidPhoneNumber(reader.Phone))
            {
                throw new FormatException("Invalid phone number format. Please use E.164 format. Example: +11222333444");
            }

            var isPhoneNumberInUse = uow.GetRepository<Reader>().GetQueryable().Any(r => r.Phone == reader.Phone);
            if(isPhoneNumberInUse)
            {
                throw new InvalidOperationException($"The specified phone number ({reader.Phone} is already in use.");
            }
        }

        int i = 0;
        int readersCount = uow.GetRepository<Reader>().GetQueryable().Count();
        string newCardNumber = (readersCount + 1).ToString();
        while(!IsValidCardNumber(newCardNumber))
        {
            i++;
            if(i > 100)
            {
                throw new InvalidOperationException("There was a problem during generating a new card number.");
            }

            int cardNumber = int.Parse(newCardNumber) + 1;
            newCardNumber = cardNumber.ToString();
        }

        Reader newReader = new() {
            Id = Guid.NewGuid(),
            FirstName = reader.FirstName,
            LastName = reader.LastName,
            CardNumber = newCardNumber,
            Email = reader.Email,
            Phone = reader.Phone
        };      

        await uow.GetRepository<Reader>().AddAsync(newReader);
        await uow.SaveChangesAsync();

        return newReader;
    }
    public async Task<Reader> UpdateReaderAsync(PatchReaderCommand reader)
    {
        Reader existingReader = await GetReaderByIdAsync(reader.Id);

        if(reader.FirstName is not null)
        {
            if(string.IsNullOrWhiteSpace(reader.FirstName))
            {
                throw new ArgumentException("A reader's first name must not be left blank.");
            }
            existingReader.FirstName = reader.FirstName;
        }

        if(reader.LastName is not null)
        {
            if(string.IsNullOrWhiteSpace(reader.LastName))
            {
                throw new ArgumentException("A reader's last name must not be left blank.");
            }
            existingReader.LastName = reader.LastName;
        }

        if(reader.Email is not null)
        {
            if(!string.IsNullOrWhiteSpace(reader.Email))
            {
                if(!IsValidEmail(reader.Email))
                {
                    throw new FormatException("Invalid email format.");
                }

                var isEmailInUse = uow.GetRepository<Reader>().GetQueryable().Any(r => r.Email == reader.Email);
                if(isEmailInUse)
                {
                    throw new InvalidOperationException($"The specified email address ({reader.Email} is already in use.");
                }
            }
            existingReader.Email = reader.Email;
        }

        if(reader.Phone is not null)
        {
            if(!string.IsNullOrEmpty(reader.Phone))
            {
                if(!IsValidPhoneNumber(reader.Phone))
                {
                    throw new FormatException("Invalid phone number format. Please use E.164 format. Example: +11222333444");
                }

                var isPhoneNumberInUse = uow.GetRepository<Reader>().GetQueryable().Any(r => r.Phone == reader.Phone);
                if(isPhoneNumberInUse)
                {
                    throw new InvalidOperationException($"The specified phone number ({reader.Phone} is already in use.");
                }
            }
            existingReader.Phone = reader.Phone;
        }

        if(reader.IsBanned.HasValue)
        {
            existingReader.IsBanned = reader.IsBanned.Value;
            existingReader.BannedDate = reader.IsBanned.Value ? DateTime.Now : null;
        }  
        
        uow.GetRepository<Reader>().Update(existingReader);
        await uow.SaveChangesAsync();

        return existingReader;
    }
    public async Task DeleteReaderAsync(string id)
    {
        var reader = await GetReaderByIdAsync(id);

        var borrowings = uow.GetRepository<Borrowing>().GetQueryable().Where(b => b.ReaderId == reader.Id);
        var activeBorrowings = borrowings.Where(b => b.ActualReturnDate == null).ToList();

        if(activeBorrowings.Any())
        {
            var borrowedCopies = activeBorrowings.Select(b => $"\"{b.Copy.Book.Title}\" #{b.Copy.CopyNumber}").ToList();
            string joinedCopies = string.Join(", ", borrowedCopies);
            throw new InvalidOperationException($"The reader cannot be deleted, because she/he has still borrowed {activeBorrowings.Count} book{( activeBorrowings.Count > 1 ? "s": "" )}: {joinedCopies}.");
        }

        foreach(Borrowing bor in borrowings)
        {
            await uow.GetRepository<Borrowing>().DeleteAsync(bor.Id);
        }
        await uow.GetRepository<Reader>().DeleteAsync(reader.Id);
        await uow.SaveChangesAsync();
    }
    public async Task<IEnumerable<Reader>> SearchReadersAsync(
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

        var searchReadersQuery = uow.GetRepository<Reader>().GetQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            if(searchTerm.Length < 3)
            {
                throw new ArgumentException($"The searching term need to have at least three letters.");
            }
            searchReadersQuery = searchReadersQuery.Where(b =>
                b.FullName      .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.CardNumber .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Email != null && b.Email.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Phone != null && b.Phone.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            );
        }

        if(!string.IsNullOrEmpty(copyId))
        {
            var copyGuid = ValidateGuid(copyId, "copy");
            var copy = await uow.GetRepository<Copy>().GetByIdAsync(copyGuid)
                ?? throw new KeyNotFoundException($"A copy with the specified ID ({copyId}) was not found in the system.");
            
            var borrowings = uow.GetRepository<Borrowing>().GetQueryable().Where(b => b.CopyId == copyGuid);
            searchReadersQuery = searchReadersQuery.Where(r => borrowings.Any(b => b.ReaderId == r.Id));
        }

        if(isBanned.HasValue)
        {
            searchReadersQuery = searchReadersQuery.Where(r => r.IsBanned == isBanned.Value);
        }
        
        var count = searchReadersQuery.Count();
        if (count > 0  && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException("Invalid page. Not so many readers.");
        }

        searchReadersQuery = searchReadersQuery.Skip((page - 1) * pageSize);
        searchReadersQuery = searchReadersQuery.Count() > pageSize ? searchReadersQuery.Take(pageSize) : searchReadersQuery;

        return searchReadersQuery.AsEnumerable();
    }

    private static Guid ValidateGuid(string id, string entity = "reader")
    {
        if(!Guid.TryParse(id, out var ReaderGuid))
        {
            throw new FormatException($"Invalid {entity} ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return ReaderGuid;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        string pattern = @"^\+?[1-9]\d{1,14}$"; // E.164 format (międzynarodowy)
        return Regex.IsMatch(phoneNumber, pattern);
    }

    private bool IsValidCardNumber(string cardNumber)
    {
        return !uow.GetRepository<Reader>().GetQueryable().Any(r => r.CardNumber == cardNumber);
    }
}