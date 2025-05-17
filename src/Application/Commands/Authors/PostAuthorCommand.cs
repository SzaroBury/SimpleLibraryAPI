namespace SimpleLibrary.Application.Commands.Authors;

public record PostAuthorCommand
(
    string FirstName, 
    string LastName,
    string? Description, 
    string? BornDate, 
    IEnumerable<string>? Tags
);