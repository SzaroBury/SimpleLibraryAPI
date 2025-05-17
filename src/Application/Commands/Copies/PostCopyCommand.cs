namespace SimpleLibrary.Application.Commands.Copies;
    
public record PostCopyCommand
(
    string BookId,
    int Shelf,
    string? Condition = null,
    string? AcquisitionDate = null,
    string? LastInspectionDate = null
);