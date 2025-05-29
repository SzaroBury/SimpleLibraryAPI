using SimpleLibrary.API.Requests.Copies;
using SimpleLibrary.Application.Commands.Copies;
using static SimpleLibrary.API.Mappers.ParsingExtensions;

namespace SimpleLibrary.API.Mappers;

public static class CopyCommandsMappingExtensions
{
    public static PostCopyCommand ToCommand(this PostCopyRequest request)
    {
        return new PostCopyCommand(
            Guid.Parse(request.BookId),
            request.Shelf,
            ParseCopyConditionOrNull(request.Condition),
            ParseDateTimeOrNull(request.AcquisitionDate),
            ParseDateTimeOrNull(request.LastInspectionDate)
        );
    }

    public static PatchCopyCommand ToCommand(this PatchCopyRequest request)
    {
        return new PatchCopyCommand(
            Guid.Parse(request.Id),
            ParseGuidOrNull(request.BookId),
            request.Shelf,
            request.IsLost,
            ParseCopyConditionOrNull(request.Condition),
            ParseDateTimeOrNull(request.AcquisitionDate),
            ParseDateTimeOrNull(request.LastInspectionDate),
            request.CopyNumber
        );
    }
}