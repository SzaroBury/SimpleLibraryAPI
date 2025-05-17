namespace SimpleLibrary.API.Requests.Books;

public record PatchBookRequest
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