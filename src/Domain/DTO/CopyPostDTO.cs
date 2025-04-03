namespace SimpleLibrary.Domain.DTO;
    
public record CopyPostDTO
(
    string BookId,
    int Shelf,
    string? Condition = null,
    string? AcquisitionDate = null,
    string? LastInspectionDate = null
);