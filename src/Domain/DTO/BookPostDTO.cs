namespace SimpleLibrary.Domain.DTO;

public record BookPostDTO(string title, string desc, string releaseDate, string language, string tags, int authorId, int categoryId);