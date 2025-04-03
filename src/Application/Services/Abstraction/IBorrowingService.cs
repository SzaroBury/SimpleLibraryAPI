using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IBorrowingService
{
    Task<IEnumerable<Borrowing>> GetAllBorrowingsAsync();
    Task<Borrowing> GetBorrowingByIdAsync(string id);
    Task<Borrowing> CreateBorrowingAsync(Borrowing borrowing);
    Task<Borrowing> UpdateBorrowingAsync(Borrowing borrowing);
    Task DeleteBorrowingAsync(string id);
    Task<IEnumerable<Borrowing>> SearchBorrowingsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? newerThan = null, 
        int? copyId = null,
        int? readerId = null,
        int page = 1, 
        int pageSize = 25);
}