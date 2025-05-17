namespace SimpleLibrary.Application.Commands.Books;

public record PostBookCommand
(
    string Title, 
    string Description, 
    string ReleaseDate, 
    string Language, 
    IEnumerable<string> Tags, 
    string AuthorId, 
    string CategoryId
);