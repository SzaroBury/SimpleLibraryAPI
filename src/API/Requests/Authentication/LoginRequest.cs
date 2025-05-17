namespace SimpleLibrary.API.Requests.Authentication;
public record LoginRequest
(
    string Username, 
    string Password
);