namespace SimpleLibrary.Application.Commands.Authors;

public record PatchAuthorCommand
(
    string Id,
    string? FirstName = null, 
    string? LastName = null,
    string? Description = null, 
    string? BornDate = null, 
    IEnumerable<string>? Tags = null
);