using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleLibrary.Api.Requests;
using SimpleLibrary.API.Attributes;
using SimpleLibrary.API.Requests;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Domain.Models;
using SimpleLibrary.Infrastructure.Services;

namespace SimpleLibrary.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork uow;
    private readonly IJwtService jwtService;
    private readonly IConfiguration config;
    private readonly ILogger<AuthController> logger;
    
    public AuthController(IUnitOfWork uow, IJwtService jwtService, IConfiguration config, ILogger<AuthController> logger)
    {
        this.uow = uow;
        this.jwtService = jwtService;
        this.config = config;
        this.logger = logger;
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        try
        {
            logger.LogInformation("RefreshToken request received.");

            // Pobierz token z ciasteczka "RefreshToken", jeśli nie został przesłany w treści żądania
            var refreshToken = Request.Cookies["RefreshToken"];
            
            // Znajdź użytkownika na podstawie RefreshToken
            var user = uow.GetRepository<User>().GetQueryable().FirstOrDefault(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiration < DateTime.UtcNow)
            {
                var message = "Invalid or expired refresh token.";
                logger.LogWarning(message);
                return Unauthorized(message);
            }

            // Wygeneruj nowe tokeny
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var accessToken = jwtService.GenerateAccessToken(claims);
            var newRefreshToken = jwtService.GenerateRefreshToken();

            Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            // Zaktualizuj refresh token w bazie danych
            var refreshTokenExpirationDays = int.Parse(config["JwtSettings:RefreshTokenExpirationDays"] ?? "0");
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(refreshTokenExpirationDays); // Przykładowa data wygaśnięcia
            uow.GetRepository<User>().Update(user);
            await uow.SaveChangesAsync();

            return Ok(new { message = "Tokens refreshed" });
        }
        catch(Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking RefreshToken(<RefreshTokenRequest Object>):");
            logger.LogError($"    {e.Message}");
            return Unauthorized($"Unexpected error.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            logger.LogInformation("Login request received.");

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            // Znajdź użytkownika w bazie danych
            var user = await uow.GetRepository<User>().GetQueryable().FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Zweryfikuj hasło
            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            Console.WriteLine(isValid); // true jeśli hasło pasuje

            if(isValid)
            {
                // Wygeneruj access i refresh tokeny
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var accessToken = jwtService.GenerateAccessToken(claims);
                var refreshToken = jwtService.GenerateRefreshToken();

                Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });

                Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                // Zaktualizuj refresh token w bazie danych
                var refreshTokenExpirationDays = int.Parse(config["JwtSettings:RefreshTokenExpirationDays"] ?? "0");
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(refreshTokenExpirationDays); // Przykładowa data wygaśnięcia
                uow.GetRepository<User>().Update(user);
                await uow.SaveChangesAsync();

                return Ok(new { message = "User logged in successfully." });
            }
            else
            {
                return Unauthorized();
            }
        }
        catch(Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking Login(<LoginRequest Object>):");
            logger.LogError($"    {e.Message}");
            return Unauthorized($"Unexpected error.");
        }
    }

    [ApiKey("Admin")]
    [HttpPost("employees")]
    public async Task<IActionResult> AddEmployeeAsync([FromBody] AddEmployeeRequest request)
    {
        try
        {
            logger.LogInformation("AddEmployee request received.");

            if(string.IsNullOrWhiteSpace(request.Firstname) 
                || string.IsNullOrWhiteSpace(request.Lastname) 
                || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Firstname and Lastname are required.");
            }

            if(request.Password != request.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            if(string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Password is required.");
            }

            // Sprawdź, czy użytkownik z takim loginem już istnieje
            var tryCount = 1;
            var newUsername = $"{request.Firstname.ToLower()}.{request.Lastname.ToLower()}";
            var isUsernameTaken = await uow.GetRepository<User>().GetQueryable().AnyAsync(u => u.Username == newUsername);
            while(isUsernameTaken)
            {
                if(tryCount > 100 )
                {
                    throw new InvalidOperationException("Unable to generate a unique username.");
                } 
                newUsername = $"{request.Firstname.ToLower()}.{request.Lastname.ToLower()}{tryCount}";
                isUsernameTaken = await uow.GetRepository<User>().GetQueryable().AnyAsync(u => u.Username == newUsername);
                tryCount++;
            }

            // Hashuj hasło
            var newPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Sprawdź rolę
            var newUserRole = UserType.Librarian;
            if(!Enum.TryParse(request.Role, out newUserRole))
            {
                return BadRequest("Invalid role specified. Allowed roles are: Admin, Librarian.");
            }

            // Wygeneruj access i refresh tokeny
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, newUsername),
                new Claim(ClaimTypes.Role, newUserRole.ToString())
            };

            var accessToken = jwtService.GenerateAccessToken(claims);
            var refreshToken = jwtService.GenerateRefreshToken();
            var refreshTokenExpirationDays = int.Parse(config["JwtSettings:RefreshTokenExpirationDays"] ?? "0");

            // Dodaj użytkownika do bazy danych
            var newUser = new User
            {
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                PasswordHash = newPassword,
                Username = newUsername,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
                Role = newUserRole
            };
            await uow.GetRepository<User>().AddAsync(newUser);

            return Ok(new { message = "A new user added successfully." });
        }
        catch(Exception e)
        {
            logger.LogError($"{DateTime.Now}: Unexpected error during invoking AddEmployeeAsync(<AddEmployee Object>):");
            logger.LogError($"    {e.Message}");
            return Unauthorized($"Unexpected error.");
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["RefreshToken"];
        if (refreshToken == null)
        {
            return NotFound("User is already logged out.");
        }

        var user = await uow.GetRepository<User>().GetQueryable().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiration = null;
        uow.GetRepository<User>().Update(user);
        await uow.SaveChangesAsync();

        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");

        return Ok(new { Message = "Logged out successfully" });
    }
}
