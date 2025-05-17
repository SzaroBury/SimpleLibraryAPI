namespace SimpleLibrary.API.Requests.Authentication;

public record AddEmployeeRequest
(
    string Firstname,
    string Lastname,
    string Password,
    string ConfirmPassword,
    string? Role
);