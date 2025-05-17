namespace SimpleLibrary.API.Requests.Readers;
    
public record PatchReaderRequest
(
    string Id,
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? Phone = null,
    bool? IsBanned = null
);