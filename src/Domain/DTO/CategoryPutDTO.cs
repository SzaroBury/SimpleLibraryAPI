namespace SimpleLibrary.Domain.DTO;
    
public record CategoryPutDTO
(
    string Id,
    string? Name = null,
    string? Tags = null,
    string? Description = null,
    string? ParentCategoryId = null
);