namespace SimpleLibrary.API.Requests.Authors;

public record PatchAuthorRequest
(
    string Id,
    string? FirstName = null, 
    string? LastName = null,
    string? Description = null, 
    string? BornDate = null, 
    IEnumerable<string>? Tags = null
);