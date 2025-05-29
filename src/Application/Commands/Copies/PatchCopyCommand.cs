using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.Application.Commands.Copies;
    
public record PatchCopyCommand
(
    Guid Id,
    Guid? BookId = null,
    int? Shelf = null,
    bool? IsLost = null,
    CopyCondition? Condition = null,
    DateTime? AcquisitionDate = null,
    DateTime? LastInspectionDate = null,
    int? CopyNumber = null
);