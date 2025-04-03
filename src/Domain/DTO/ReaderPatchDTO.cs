namespace SimpleLibrary.Domain.DTO;
    
public record ReaderPatchDTO
(
    string Id,
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? Phone = null,
    bool? IsBanned = null
);