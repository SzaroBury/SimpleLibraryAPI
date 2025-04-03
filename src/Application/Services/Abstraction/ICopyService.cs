using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface ICopyService
{
    Task<IEnumerable<Copy>> GetAllCopiesAsync();
    Task<Copy> GetCopyByIdAsync(string id);
    Task<Copy> CreateCopyAsync(Copy Copy);
    Task<Copy> UpdateCopyAsync(Copy Copy);
    Task DeleteCopyAsync(string id);
    Task<IEnumerable<Copy>> SearchCopiesAsync(
        string? searchTerm = null, 
        int? bookId = null,
        int page = 1, 
        int pageSize = 25);
}