namespace SimpleLibrary.API.Requests.Authentication;

public record RefreshTokenRequest
(
    string UserId,
    string RefreshToken, 
    string AccessToken
);