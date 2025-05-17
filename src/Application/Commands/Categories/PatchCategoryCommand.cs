namespace SimpleLibrary.Application.Commands.Categories;
    
public record PatchCategoryCommand
(
    string Id,
    string? Name = null,
    string? Description = null,
    IEnumerable<string>? Tags = null,
    string? ParentCategoryId = null
);