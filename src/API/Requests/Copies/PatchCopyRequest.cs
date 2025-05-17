namespace SimpleLibrary.API.Requests.Copies;
    
public record PatchCopyRequest
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