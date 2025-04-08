namespace SimpleLibrary.Domain.DTO;
    
public record CopyPostDTO
(
    string BookId,
    int Shelf,
    string? AcquisitionDate,
    string? Condition,
    string? LastInspectionDate
);