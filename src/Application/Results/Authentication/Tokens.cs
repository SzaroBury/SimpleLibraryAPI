namespace SimpleLibrary.Application.Results.Authentication;

public record Tokens(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);