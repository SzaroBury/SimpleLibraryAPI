namespace SimpleLibrary.API.Mappers;

using SimpleLibrary.API.Requests.Borrowings;
using SimpleLibrary.Application.Commands.Borrowings;

public static class BorrowingMapper
{
    public static PostBorrowingCommand ToCommand(PostBorrowingRequest request)
    {
        return new PostBorrowingCommand(
            request.CopyId,
            request.ReaderId,
            request.StartedDate,
            request.ActualReturnDate
        );
    }

    public static PatchBorrowingCommand ToCommand(PatchBorrowingRequest request)
    {
        return new PatchBorrowingCommand(
            request.Id,
            request.CopyId,
            request.ReaderId,
            request.StartedDate,
            request.ActualReturnDate
        );
    }
}