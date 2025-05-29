namespace SimpleLibrary.Application.Commands.Borrowings;
    
public record PostBorrowingCommand
(
    Guid CopyId,
    Guid ReaderId,
    DateTime? StartedDate = null,
    DateTime? ActualReturnDate = null
);