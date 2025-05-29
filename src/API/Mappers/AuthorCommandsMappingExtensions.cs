using SimpleLibrary.API.Requests.Authors;
using SimpleLibrary.Application.Commands.Authors;
using static SimpleLibrary.API.Mappers.ParsingExtensions;

namespace SimpleLibrary.API.Mappers;

public static class AuthorCommandsMappingExtensions
{
    public static PostAuthorCommand ToCommand(this PostAuthorRequest request)
    {
        return new PostAuthorCommand(
            request.FirstName,
            request.LastName,
            request.Description,
            ParseDateTimeOrNull(request.BornDate),
            request.Tags
        );
    }

    public static PatchAuthorCommand ToCommand(this PatchAuthorRequest request)
    {
        return new PatchAuthorCommand(
            Guid.Parse(request.Id),
            request.FirstName,
            request.LastName,
            request.Description,
            ParseDateTimeOrNull(request.BornDate),
            request.Tags
        );
    }
}