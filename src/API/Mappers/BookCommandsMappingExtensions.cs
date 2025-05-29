using SimpleLibrary.API.Requests.Books;
using SimpleLibrary.Application.Commands.Books;
using SimpleLibrary.Domain.Enumerations;
using static SimpleLibrary.API.Mappers.ParsingExtensions;

namespace SimpleLibrary.API.Mappers;

public static class BookCommandsMappingExtensions
{
    public static PostBookCommand ToCommand(this PostBookRequest request)
    {
        return new PostBookCommand(
            request.Title,
            request.Description,
            DateTime.Parse(request.ReleaseDate),
            (Language)Enum.Parse(typeof(Language), request.Language),
            request.Tags,
            Guid.Parse(request.AuthorId),
            Guid.Parse(request.CategoryId)
        );
    }

    public static PatchBookCommand ToCommand(this PatchBookRequest request)
    {
        return new PatchBookCommand(
            Guid.Parse(request.Id),
            request.Title,
            request.Description,
            ParseDateTimeOrNull(request.ReleaseDate),
            ParseLanguageOrNull(request.Language),
            request.Tags,
            ParseGuidOrNull(request.AuthorId),
            ParseGuidOrNull(request.CategoryId)
        );
    }
}