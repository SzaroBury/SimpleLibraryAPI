namespace SimpleLibrary.Domain.DTO;
    
public record ReaderPutDTO
(
    string Id,
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? Phone = null,
    bool? IsBanned = null
);