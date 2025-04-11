namespace SimpleLibrary.Domain.DTO;
    
public record CopyPutDTO
(
    string Id,
    string? BookId = null,
    int? Shelf = null,
    bool? IsLost = null,
    string? Condition = null,
    string? AcquisitionDate = null,
    string? LastInspectionDate = null,
    int? CopyNumber = null
);