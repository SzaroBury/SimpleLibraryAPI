using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork uow;

    public UserService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.");
        }

        var user = uow.GetRepository<User>().GetQueryable().FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            throw new KeyNotFoundException("The user was not found in the system.");
        }

        return await Task.FromResult(user);
    }

    public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var user = uow.GetRepository<User>().GetQueryable().FirstOrDefault(u => u.RefreshToken == refreshToken);
        if (user == null)
        {
            throw new KeyNotFoundException("The user was not found in the system.");
        }

        if (user.RefreshTokenExpiration < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("The refresh token has expired. Please log in again.");
        }

        return await Task.FromResult(user);
    }

    public async Task<string> GenerateUniqueUsernameAsync(string firstname, string lastname)
    {
        var tryCount = 1;
        var newUsername = $"{firstname.ToLower()}.{lastname.ToLower()}";
        var isUsernameTaken = await IsUsernameTakenAsync(newUsername); // uow.GetRepository<User>().GetQueryable().Any(u => u.Username == newUsername);
        while(isUsernameTaken)
        {
            if(tryCount > 100 )
            {
                throw new InvalidOperationException("Unable to generate a unique username.");
            } 
            newUsername = $"{firstname.ToLower()}.{lastname.ToLower()}{tryCount}";
            isUsernameTaken = await IsUsernameTakenAsync(newUsername); // uow.GetRepository<User>().GetQueryable().Any(u => u.Username == newUsername);
            tryCount++;
        }
        return newUsername;
    }

    public async Task<bool> IsUsernameTakenAsync(string username)
    {
        var result = uow.GetRepository<User>().GetQueryable().Any(u => u.Username == username);
        return await Task.FromResult(result);
    }
}