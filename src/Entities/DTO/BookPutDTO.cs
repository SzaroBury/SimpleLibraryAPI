namespace Entities.DTO;

public record BookPutDTO(int id, string? title = null, string? desc = null, string? releaseDate = null, string? language = null, string? tags = null, int? authorId = null, int? categoryId = null);