namespace SimpleLibrary.Application.Commands.Readers;
    
public record PostReaderCommand
(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone
);