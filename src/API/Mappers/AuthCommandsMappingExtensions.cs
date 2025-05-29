using SimpleLibrary.API.Requests.Authentication;
using SimpleLibrary.Application.Commands.Authentication;
using static SimpleLibrary.API.Mappers.ParsingExtensions;

namespace SimpleLibrary.API.Mappers;

public static class AuthCommandsMappingExtensions
{
    public static AddUserCommand ToCommand(this AddUserRequest request)
    {
        return new AddUserCommand(
            request.Firstname,
            request.Lastname,
            request.Password,
            request.ConfirmPassword,
            ParseUserTypeOrNull(request.Role)
        );
    }
}