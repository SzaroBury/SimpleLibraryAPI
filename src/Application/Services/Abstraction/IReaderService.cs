using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IReaderService
{
    Task<IEnumerable<Reader>> GetAllReadersAsync();
    Task<Reader> GetReaderByIdAsync(string id);
    Task<Reader> CreateReaderAsync(ReaderPostDTO Reader);
    Task<Reader> UpdateReaderAsync(ReaderPutDTO Reader);
    Task DeleteReaderAsync(string id);
    Task<IEnumerable<Reader>> SearchReadersAsync(
        string? searchTerm = null,
        string? copyId = null,
        bool? isBanned = null, 
        int page = 1, 
        int pageSize = 25);
}