using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.Application.Commands.Authentication;

public record AddUserCommand
(
    string Firstname,
    string Lastname,
    string Password,
    string ConfirmPassword,
    UserType? Role
);