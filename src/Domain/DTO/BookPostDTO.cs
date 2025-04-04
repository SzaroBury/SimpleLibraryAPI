namespace SimpleLibrary.Domain.DTO;

public record BookPostDTO
(
    string Title, 
    string Description, 
    string ReleaseDate, 
    string Language, 
    IEnumerable<string> Tags, 
    string AuthorId, 
    string CategoryId
);