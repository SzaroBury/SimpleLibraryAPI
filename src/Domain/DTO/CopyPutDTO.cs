namespace SimpleLibrary.Domain.DTO;
    
public record CopyPutDTO
(
    string Id,
    string? BookId = null,
    int? Shelf = null,
    string? AcquisitionDate = null,
    string? Condition = null,
    string? LastInspectionDate = null
);