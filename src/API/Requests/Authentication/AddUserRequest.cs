namespace SimpleLibrary.API.Requests.Authentication;

public record AddUserRequest
(
    string Firstname,
    string Lastname,
    string Password,
    string ConfirmPassword,
    string? Role
);