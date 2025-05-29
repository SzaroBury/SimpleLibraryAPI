using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using SimpleLibrary.Application.Common;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.Infrastructure.Services;

public class JwtService: IJwtService
{
    private readonly JwtSettings jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        this.jwtSettings = jwtSettings.Value;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var accessTokenSecret = jwtSettings.AccessTokenSecret ?? throw new Exception("JwtSettings:AccessTokenSecret can not be empty");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var accessTokenExpirationMinutes = jwtSettings.AccessTokenExpirationMinutes;

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        var randomPart = Convert.ToBase64String(randomBytes);
        var uniquePart = Guid.NewGuid();
        return $"{randomPart}.{uniquePart}";
    }
}