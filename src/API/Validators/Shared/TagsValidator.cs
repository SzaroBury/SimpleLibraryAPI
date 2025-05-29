namespace SimpleLibrary.API.Validators.Shared;

public static class TagsValidator
{
    public static bool BeAValidTagFormat(IEnumerable<string>? tags)
    {
        var result = true;
        if(tags != null)
        {
            result = !tags.Any(tag => tag.Contains(','));
        }
        return result;
    }
}