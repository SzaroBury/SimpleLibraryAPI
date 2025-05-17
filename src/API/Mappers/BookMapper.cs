namespace SimpleLibrary.API.Mappers;

using SimpleLibrary.API.Requests.Books;
using SimpleLibrary.Application.Commands.Books;

public static class BookMapper
{
    public static PostBookCommand ToCommand(PostBookRequest request)
    {
        return new PostBookCommand(
            request.Title,
            request.Description,
            request.ReleaseDate,
            request.Language,
            request.Tags,
            request.AuthorId,
            request.CategoryId
        );
    }

    public static PatchBookCommand ToCommand(PatchBookRequest request)
    {
        return new PatchBookCommand(
            request.Id,
            request.Title,
            request.Description,
            request.ReleaseDate,
            request.Language,
            request.Tags,
            request.AuthorId,
            request.CategoryId
        );
    }
}