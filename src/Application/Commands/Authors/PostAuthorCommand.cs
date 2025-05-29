namespace SimpleLibrary.Application.Commands.Authors;

public record PostAuthorCommand
(
    string FirstName, 
    string LastName,
    string? Description, 
    DateTime? BornDate, 
    IEnumerable<string>? Tags
);