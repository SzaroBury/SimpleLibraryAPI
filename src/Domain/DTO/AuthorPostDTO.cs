namespace SimpleLibrary.Domain.DTO;

public record AuthorPostDTO(
    string FirstName, 
    string LastName,
    string Description, 
    string BornDate, 
    IEnumerable<string> Tags
);