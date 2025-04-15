namespace SimpleLibrary.Domain.DTO;
    
public record BorrowingPostDTO
(
    string CopyId,
    string ReaderId,
    string? StartedDate = null,
    string? ActualReturnDate = null
);