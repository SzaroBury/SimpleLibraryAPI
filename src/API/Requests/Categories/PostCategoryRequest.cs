namespace SimpleLibrary.API.Requests.Categories;
    
public record PostCategoryRequest
(
    string Name,
    string Description = "",
    IEnumerable<string>? Tags = null,
    string? ParentCategoryId = null
);