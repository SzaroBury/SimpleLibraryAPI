namespace SimpleLibrary.API.Mappers;

using SimpleLibrary.API.Requests.Readers;
using SimpleLibrary.Application.Commands.Readers;

public static class ReaderMapper
{
    public static PostReaderCommand ToCommand(PostReaderRequest request)
    {
        return new PostReaderCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone
        );
    }

    public static PatchReaderCommand ToCommand(PatchReaderRequest request)
    {
        return new PatchReaderCommand(
            request.Id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.IsBanned
        );
    }
}