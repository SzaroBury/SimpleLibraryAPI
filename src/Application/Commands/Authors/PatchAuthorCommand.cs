namespace SimpleLibrary.Application.Commands.Authors;

public record PatchAuthorCommand
(
    Guid Id,
    string? FirstName = null, 
    string? LastName = null,
    string? Description = null, 
    DateTime? BornDate = null, 
    IEnumerable<string>? Tags = null
);