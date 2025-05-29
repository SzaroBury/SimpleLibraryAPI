using System.Security.Claims;
using Microsoft.Extensions.Options;

using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Application.Common;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Application.Commands.Authentication;
using SimpleLibrary.Application.Results.Authentication;

namespace SimpleLibrary.Application.Services;

public class AuthService: IAuthService
{
    private readonly IUnitOfWork uow;
    private readonly IJwtService jwtService;
    private readonly IUserService userService;
    private readonly JwtSettings jwtSettings;

    public AuthService(IJwtService jwtService, IUnitOfWork uow, IUserService userService, IOptions<JwtSettings> jwtSettings)
    {
        this.uow = uow;
        this.jwtService = jwtService;
        this.userService = userService;
        this.jwtSettings = jwtSettings.Value;
    }

    public async Task<Tokens> LoginAsync(string username, string password)
    {
        var user = await userService.GetUserByUsernameAsync(username);

        if(!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new ArgumentException("The given password is not correct!");
        }

        var tokens = GenerateTokens(user.Username, user.Role);
        await SaveRefreshTokenAsync(user, tokens.RefreshToken, tokens.RefreshTokenExpiration);

        return tokens;
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await userService.GetUserByRefreshTokenAsync(refreshToken);

        user.RefreshToken = null;
        user.RefreshTokenExpiration = null;
        uow.GetRepository<User>().Update(user);
        await uow.SaveChangesAsync();
    }

    public async Task<Tokens> RefreshTokensAsync(string refreshToken)
    {
        var user = await userService.GetUserByRefreshTokenAsync(refreshToken);

        var tokens = GenerateTokens(user.Username, user.Role);
        await SaveRefreshTokenAsync(user, tokens.RefreshToken, tokens.RefreshTokenExpiration);

        return tokens;
    }

    public async Task<User> AddUserAsync(AddUserCommand command)
    {
        // Sprawdź, czy użytkownik z takim loginem już istnieje
        var newUsername = await userService.GenerateUniqueUsernameAsync(command.Firstname, command.Lastname);
        
        // Hashuj hasło
        var newPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);

        // Generowanie tokenów
        var role = command.Role ?? UserType.Librarian;
        var tokens = GenerateTokens(newUsername, role);

        // Dodaj użytkownika do bazy danych
        var newUser = new User
        {
            Firstname = command.Firstname,
            Lastname = command.Lastname,
            PasswordHash = newPassword,
            Username = newUsername,
            RefreshToken = tokens.RefreshToken,
            RefreshTokenExpiration = tokens.RefreshTokenExpiration,
            Role = role
        };
        await uow.GetRepository<User>().AddAsync(newUser);
        await uow.SaveChangesAsync();

        return newUser;
    }

    private Tokens GenerateTokens(string username, UserType userType)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, userType.ToString())
        };

        var accessToken = jwtService.GenerateAccessToken(claims);
        var newRefreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays);

        return new Tokens(accessToken, newRefreshToken, refreshTokenExpiration);
    }

    private async Task SaveRefreshTokenAsync(User user, string refreshToken, DateTime refreshTokenExpiration)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = refreshTokenExpiration;
        uow.GetRepository<User>().Update(user);
        await uow.SaveChangesAsync();
    }
}