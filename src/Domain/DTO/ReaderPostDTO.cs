namespace SimpleLibrary.Domain.DTO;
    
public record ReaderPostDTO
(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone
);