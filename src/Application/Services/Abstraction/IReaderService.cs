using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IReaderService
{
    Task<IEnumerable<Reader>> GetAllReadersAsync();
    Task<Reader> GetReaderByIdAsync(int id);
    Task<Reader> CreateReaderAsync(Reader Reader);
    Task<Reader> UpdateReaderAsync(Reader Reader);
    Task DeleteReaderAsync(int id);
    Task<IEnumerable<Reader>> SearchReadersAsync(
        string? searchTerm = null,
        int? copyId = null,
        int page = 1, 
        int pageSize = 25);
}