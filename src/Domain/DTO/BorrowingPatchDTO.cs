namespace SimpleLibrary.Domain.DTO;
    
public record BorrowingPatchDTO
(
    string Id,
    string? CopyId = null,
    string? ReaderId = null,
    string? StartedDate = null,
    string? ActualReturnDate = null
);