namespace SimpleLibrary.Application.Commands.Categories;
    
public record PatchCategoryCommand
(
    Guid Id,
    string? Name = null,
    string? Description = null,
    IEnumerable<string>? Tags = null,
    Guid? ParentCategoryId = null
);