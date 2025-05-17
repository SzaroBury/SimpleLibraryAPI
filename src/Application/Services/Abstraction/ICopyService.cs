using SimpleLibrary.Application.Commands.Copies;
using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface ICopyService
{
    Task<IEnumerable<Copy>> GetAllCopiesAsync();
    Task<Copy> GetCopyByIdAsync(string id);
    Task<Copy> CreateCopyAsync(PostCopyCommand Copy);
    Task<Copy> UpdateCopyAsync(PatchCopyCommand Copy);
    Task DeleteCopyAsync(string id);
    Task<IEnumerable<Copy>> SearchCopiesAsync(
        string? searchTerm = null, 
        string? bookId = null,
        bool? isAvailable = null,
        int page = 1, 
        int pageSize = 25);
}