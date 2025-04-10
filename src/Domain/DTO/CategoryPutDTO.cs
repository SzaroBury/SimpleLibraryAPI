namespace SimpleLibrary.Domain.DTO;
    
public record CategoryPutDTO
(
    string Id,
    string? Name = null,
    string? Description = null,
    IEnumerable<string>? Tags = null,
    string? ParentCategoryId = null
);