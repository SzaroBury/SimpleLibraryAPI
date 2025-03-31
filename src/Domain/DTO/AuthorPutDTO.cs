namespace SimpleLibrary.Domain.DTO;

public record AuthorPutDTO(
    string? FirstName = null, 
    string? LastName = null,
    string? Description = null, 
    string? BornDate = null, 
    IEnumerable<string>? Tags = null
);