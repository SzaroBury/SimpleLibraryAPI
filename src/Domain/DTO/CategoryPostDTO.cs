namespace SimpleLibrary.Domain.DTO;
    
public record CategoryPostDTO
(
    string Name,
    string Tags = "",
    string Description = "",
    string? ParentCategoryId = null
);