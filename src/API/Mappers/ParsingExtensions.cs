using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.API.Mappers;

public static class ParsingExtensions
{
    public static Guid? ParseGuidOrNull(string? guid) 
        => string.IsNullOrWhiteSpace(guid) ? null : Guid.Parse(guid);

    public static DateTime? ParseDateTimeOrNull(string? date) 
        => string.IsNullOrWhiteSpace(date) ? null : DateTime.Parse(date);

    public static Language? ParseLanguageOrNull(string? language)
        => string.IsNullOrWhiteSpace(language) ? null : Enum.Parse<Language>(language);

    public static UserType? ParseUserTypeOrNull(string? userType)
        => string.IsNullOrWhiteSpace(userType) ? null : Enum.Parse<UserType>(userType);

    public static CopyCondition? ParseCopyConditionOrNull(string? userType)
        => string.IsNullOrWhiteSpace(userType) ? null : Enum.Parse<CopyCondition>(userType);
}