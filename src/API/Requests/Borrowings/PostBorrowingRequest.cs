namespace SimpleLibrary.API.Requests.Borrowings;
    
public record PostBorrowingRequest
(
    string CopyId,
    string ReaderId,
    string? StartedDate = null,
    string? ActualReturnDate = null
);