namespace SimpleLibrary.API.Requests.Readers;
    
public record PostReaderRequest
(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone
);