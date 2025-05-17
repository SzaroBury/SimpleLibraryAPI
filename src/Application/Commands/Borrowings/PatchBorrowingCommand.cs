namespace SimpleLibrary.Application.Commands.Borrowings;
    
public record PatchBorrowingCommand
(
    string Id,
    string? CopyId = null,
    string? ReaderId = null,
    string? StartedDate = null,
    string? ActualReturnDate = null
);