namespace SimpleLibrary.Domain.DTO;

public record BookPatchDTO
(
    string Id,
    string? Title = null, 
    string? Description = null,
    string? ReleaseDate = null,
    string? Language = null,
    IEnumerable<string>? Tags = null,
    string? AuthorId = null,
    string? CategoryId = null
);