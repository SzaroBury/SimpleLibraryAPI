namespace SimpleLibrary.API.Requests;

public record AddEmployeeRequest(
    string Firstname,
    string Lastname,
    string Password,
    string ConfirmPassword,
    string? Role
);