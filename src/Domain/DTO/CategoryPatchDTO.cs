namespace SimpleLibrary.Domain.DTO;
    
public record CategoryPatchDTO
(
    string Id,
    string? Name = null,
    string? Description = null,
    IEnumerable<string>? Tags = null,
    string? ParentCategoryId = null
);