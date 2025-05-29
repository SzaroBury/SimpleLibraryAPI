namespace SimpleLibrary.API.Mappers;

using SimpleLibrary.API.Requests.Readers;
using SimpleLibrary.Application.Commands.Readers;

public static class ReaderCommandsMappingExtensions
{
    public static PostReaderCommand ToCommand(this PostReaderRequest request)
    {
        return new PostReaderCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone
        );
    }

    public static PatchReaderCommand ToCommand(this PatchReaderRequest request)
    {
        return new PatchReaderCommand(
            Guid.Parse(request.Id),
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.IsBanned
        );
    }
}