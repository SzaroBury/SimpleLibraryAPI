using SimpleLibrary.API.Requests.Borrowings;
using SimpleLibrary.Application.Commands.Borrowings;
using static SimpleLibrary.API.Mappers.ParsingExtensions;

namespace SimpleLibrary.API.Mappers;

public static class BorrowingCommandsMappingExtensions
{
    public static PostBorrowingCommand ToCommand(this PostBorrowingRequest request)
    {
        return new PostBorrowingCommand(
            Guid.Parse(request.CopyId),
            Guid.Parse(request.ReaderId),
            ParseDateTimeOrNull(request.StartedDate),
            ParseDateTimeOrNull(request.ActualReturnDate)
        );
    }

    public static PatchBorrowingCommand ToCommand(this PatchBorrowingRequest request)
    {
        return new PatchBorrowingCommand(
            Guid.Parse(request.Id),
            ParseGuidOrNull(request.CopyId),
            ParseGuidOrNull(request.ReaderId),
            ParseDateTimeOrNull(request.StartedDate),
            ParseDateTimeOrNull(request.ActualReturnDate)
        );
    }
}