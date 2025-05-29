using SimpleLibrary.Application.Commands.Authentication;
using SimpleLibrary.Application.Results.Authentication;
using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IAuthService
{
    Task<Tokens> LoginAsync(string username, string password);
    Task<Tokens> RefreshTokensAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
    Task<User> AddUserAsync(AddUserCommand command);
}