namespace SimpleLibrary.API.Requests.Borrowings;
    
public record PatchBorrowingRequest
(
    string Id,
    string? CopyId = null,
    string? ReaderId = null,
    string? StartedDate = null,
    string? ActualReturnDate = null
);