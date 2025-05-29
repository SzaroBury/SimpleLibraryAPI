using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.Application.Commands.Books;

public record PatchBookCommand
(
    Guid Id,
    string? Title = null, 
    string? Description = null,
    DateTime? ReleaseDate = null,
    Language? Language = null,
    IEnumerable<string>? Tags = null,
    Guid? AuthorId = null,
    Guid? CategoryId = null
);