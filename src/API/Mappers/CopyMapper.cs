namespace SimpleLibrary.API.Mappers;

using SimpleLibrary.API.Requests.Copies;
using SimpleLibrary.Application.Commands.Copies;

public static class CopyMapper
{
    public static PostCopyCommand ToCommand(PostCopyRequest request)
    {
        return new PostCopyCommand(
            request.BookId,
            request.Shelf,
            request.Condition,
            request.AcquisitionDate,
            request.LastInspectionDate
        );
    }

    public static PatchCopyCommand ToCommand(PatchCopyRequest request)
    {
        return new PatchCopyCommand(
            request.Id,
            request.BookId,
            request.Shelf,
            request.IsLost,
            request.Condition,
            request.AcquisitionDate,
            request.LastInspectionDate,
            request.CopyNumber
        );
    }
}