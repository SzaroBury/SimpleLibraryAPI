namespace SimpleLibrary.Domain.DTO;
    
public record BorrowingPutDTO
(
    string Id,
    string? CopyId = null,
    string? ReaderId = null,
    string? StartedDate = null,
    string? ActualReturnDate = null
);