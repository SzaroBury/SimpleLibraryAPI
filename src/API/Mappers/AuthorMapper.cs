namespace SimpleLibrary.API.Mappers;

using SimpleLibrary.API.Requests.Authors;
using SimpleLibrary.Application.Commands.Authors;

public static class AuthorMapper
{
    public static PostAuthorCommand ToCommand(PostAuthorRequest request)
    {
        return new PostAuthorCommand(
            request.FirstName,
            request.LastName,
            request.Description,
            request.BornDate,
            request.Tags
        );
    }

    public static PatchAuthorCommand ToCommand(PatchAuthorRequest request)
    {
        return new PatchAuthorCommand(
            request.Id,
            request.FirstName,
            request.LastName,
            request.Description,
            request.BornDate,
            request.Tags
        );
    }
}