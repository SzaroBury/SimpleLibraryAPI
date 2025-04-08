namespace SimpleLibrary.Domain.DTO;
    
public record BorrowingPostDTO
(
    string StartedDate,
    string ActualReturnDate,
    string CopyId,
    string ReaderId
);