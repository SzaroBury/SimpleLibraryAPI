using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IUserService
{
    public Task<User> GetUserByUsernameAsync(string username);
    public Task<User> GetUserByRefreshTokenAsync(string refreshToken);
    public Task<string> GenerateUniqueUsernameAsync(string firstname, string lastname);
    public Task<bool> IsUsernameTakenAsync(string username);
}