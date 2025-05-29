using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using SimpleLibrary.API.Mappers;
using SimpleLibrary.API.Requests.Authentication;
using SimpleLibrary.Application.Common;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly JwtSettings jwtSettings;
    private readonly ILogger<AuthController> logger;
    
    public AuthController(IAuthService authService, IOptions<JwtSettings> jwtSettings, ILogger<AuthController> logger)
    {
        this.authService = authService;
        this.jwtSettings = jwtSettings.Value;
        this.logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        logger.LogInformation("Login request received.");

        var (accessToken, refreshToken, refreshTokenExpires) = await authService.LoginAsync(request.Username, request.Password);

        AppendAccessToken(accessToken);
        AppendRefreshToken(refreshToken, refreshTokenExpires);

        return Ok("User logged in successfully.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        logger.LogInformation("Logout request received.");

        var refreshToken = Request.Cookies["RefreshToken"];
        if (refreshToken == null)
        {
            return BadRequest("User is not logged in.");
        }

        await authService.LogoutAsync(refreshToken);

        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");

        return Ok("Logged out successfully");
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        logger.LogInformation("RefreshToken request received.");

        var refreshToken = Request.Cookies["RefreshToken"];
        if (refreshToken == null)
        {
            return BadRequest("User is not logged in.");
        }

        var (newAccessToken, newRefreshToken, newRefreshTokenExpires) = await authService.RefreshTokensAsync(refreshToken);

        AppendAccessToken(newAccessToken);
        AppendRefreshToken(newRefreshToken, newRefreshTokenExpires);

        return Ok("Tokens refreshed successfully");
    }

    [HttpPost("new")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddUserAsync(AddUserRequest request)
    {
        logger.LogInformation("AddUser request received.");

        var user = await authService.AddUserAsync(request.ToCommand());

        return Ok($"A new user ({user.Username}) has been added successfully.");
    }

    private void AppendAccessToken(string accessToken)
    {
        Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes)
        });
    }

    private void AppendRefreshToken(string refreshToken, DateTime refreshTokenExpires)
    {
        Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshTokenExpires
        });
    }
}
