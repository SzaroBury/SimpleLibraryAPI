namespace SimpleLibrary.Domain.DTO;
    
public record CategoryPostDTO
(
    string Name,
    string Description = "",
    IEnumerable<string>? Tags = null,
    string? ParentCategoryId = null
);