namespace SimpleLibrary.Domain.DTO;
    
public record BorrowingPutDTO
(
    string Id,
    string? StartedDate = null,
    string? ActualReturnDate = null,
    string? CopyId = null,
    string? ReaderId = null
);