using SimpleLibrary.API.Requests.Categories;
using SimpleLibrary.Application.Commands.Categories;
using static SimpleLibrary.API.Mappers.ParsingExtensions;

namespace SimpleLibrary.API.Mappers;

public static class CategoryCommandsMappingExtensions
{
    public static PostCategoryCommand ToCommand(this PostCategoryRequest request)
    {
        return new PostCategoryCommand(
            request.Name,
            request.Description,
            request.Tags,
            ParseGuidOrNull(request.ParentCategoryId)
        );
    }

    public static PatchCategoryCommand ToCommand(this PatchCategoryRequest request)
    {
        return new PatchCategoryCommand(
            Guid.Parse(request.Id),
            request.Name,
            request.Description,
            request.Tags,
            ParseGuidOrNull(request.ParentCategoryId)
        );
    }
}