namespace SimpleLibrary.API.Mappers;

using SimpleLibrary.API.Requests.Categories;
using SimpleLibrary.Application.Commands.Categories;

public static class CategoryMapper
{
    public static PostCategoryCommand ToCommand(PostCategoryRequest request)
    {
        return new PostCategoryCommand(
            request.Name,
            request.Description,
            request.Tags,
            request.ParentCategoryId
        );
    }

    public static PatchCategoryCommand ToCommand(PatchCategoryRequest request)
    {
        return new PatchCategoryCommand(
            request.Id,
            request.Name,
            request.Description,
            request.Tags,
            request.ParentCategoryId
        );
    }
}