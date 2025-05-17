namespace SimpleLibrary.API.Requests.Books;

public record PostBookRequest
(
    string Title, 
    string Description, 
    string ReleaseDate, 
    string Language, 
    IEnumerable<string> Tags, 
    string AuthorId, 
    string CategoryId
);