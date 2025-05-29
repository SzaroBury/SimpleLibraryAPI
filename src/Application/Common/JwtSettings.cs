namespace SimpleLibrary.Application.Common;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string AccessTokenSecret { get; set; } = string.Empty;
    public string RefreshTokenSecret { get; set; } = string.Empty;
    public double AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
}