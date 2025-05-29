namespace SimpleLibrary.Application.Commands.Categories;
    
public record PostCategoryCommand
(
    string Name,
    string Description = "",
    IEnumerable<string>? Tags = null,
    Guid? ParentCategoryId = null
);