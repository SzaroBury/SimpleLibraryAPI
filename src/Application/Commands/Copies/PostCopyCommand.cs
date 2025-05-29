using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.Application.Commands.Copies;
    
public record PostCopyCommand
(
    Guid BookId,
    int Shelf,
    CopyCondition? Condition = null,
    DateTime? AcquisitionDate = null,
    DateTime? LastInspectionDate = null
);