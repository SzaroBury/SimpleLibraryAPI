namespace SimpleLibrary.API.Requests;

public record RefreshTokenRequest(
    string UserId,
    string RefreshToken, 
    string AccessToken
);