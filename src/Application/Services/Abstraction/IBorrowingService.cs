using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IBorrowingService
{
    Task<IEnumerable<Borrowing>> GetAllBorrowingsAsync();
    Task<Borrowing> GetBorrowingByIdAsync(string id);
    Task<Borrowing> CreateBorrowingAsync(BorrowingPostDTO borrowing);
    Task<Borrowing> UpdateBorrowingAsync(BorrowingPutDTO borrowing);
    Task DeleteBorrowingAsync(string id);
    Task<IEnumerable<Borrowing>> SearchBorrowingsAsync(
        string? searchTerm = null, 
        string? olderThan = null, 
        string? newerThan = null, 
        string? copyId = null,
        string? readerId = null,
        int page = 1, 
        int pageSize = 25);
}