namespace SimpleLibrary.API.Validators.Shared;

public static class GuidValidator
{
    public static bool BeAValidGuid(string guid)
    {
        return Guid.TryParse(guid, out _);
    }

    public static bool BeAValidNullableGuid(string? guid)
    {
        if(!string.IsNullOrWhiteSpace(guid))
        {
            return Guid.TryParse(guid, out _);
        }

        return true;
    }
}