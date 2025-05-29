namespace SimpleLibrary.Application.Commands.Borrowings;
    
public record PatchBorrowingCommand
(
    Guid Id,
    Guid? CopyId = null,
    Guid? ReaderId = null,
    DateTime? StartedDate = null,
    DateTime? ActualReturnDate = null
);