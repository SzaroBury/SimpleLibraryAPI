using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.Application.Commands.Books;

public record PostBookCommand
(
    string Title, 
    string Description, 
    DateTime ReleaseDate, 
    Language Language, 
    IEnumerable<string> Tags, 
    Guid AuthorId, 
    Guid CategoryId
);