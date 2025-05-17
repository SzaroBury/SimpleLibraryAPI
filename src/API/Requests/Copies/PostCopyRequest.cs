namespace SimpleLibrary.API.Requests.Copies;
    
public record PostCopyRequest
(
    string BookId,
    int Shelf,
    string? Condition = null,
    string? AcquisitionDate = null,
    string? LastInspectionDate = null
);