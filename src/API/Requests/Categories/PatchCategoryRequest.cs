namespace SimpleLibrary.API.Requests.Categories;
    
public record PatchCategoryRequest
(
    string Id,
    string? Name = null,
    string? Description = null,
    IEnumerable<string>? Tags = null,
    string? ParentCategoryId = null
);