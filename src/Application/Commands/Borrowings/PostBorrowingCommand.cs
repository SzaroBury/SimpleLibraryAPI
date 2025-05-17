namespace SimpleLibrary.Application.Commands.Borrowings;
    
public record PostBorrowingCommand
(
    string CopyId,
    string ReaderId,
    string? StartedDate = null,
    string? ActualReturnDate = null
);