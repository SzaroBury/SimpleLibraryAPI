namespace SimpleLibrary.Application.Commands.Readers;
    
public record PatchReaderCommand
(
    Guid Id,
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? Phone = null,
    bool? IsBanned = null
);