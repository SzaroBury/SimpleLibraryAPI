namespace SimpleLibrary.API.Validators.Shared;

public static class DateValidator
{
    public static bool BeAValidDate(string? date)
    {
        return DateTime.TryParse(date, out var parsedDate);
    }

    public static bool BeADateInThePast(string? date)
    {
        if (DateTime.TryParse(date, out var parsedDate))
        {
            return parsedDate < DateTime.Now;
        }
        return false;
    }
}