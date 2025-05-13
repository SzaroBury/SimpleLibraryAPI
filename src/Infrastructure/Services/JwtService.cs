using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SimpleLibrary.Infrastructure.Services;

public class JwtService: IJwtService
{
    private readonly IConfiguration config;

    public JwtService(IConfiguration configuration)
    {
        config = configuration;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var accessTokenSecret = config["JwtSettings:AccessTokenSecret"] ?? throw new Exception("JwtSettings:AccessTokenSecret can not be empty");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var accessTokenExpirationMinutes = double.Parse(config["JwtSettings:AccessTokenExpirationMinutes"] ?? "0");

        var token = new JwtSecurityToken(
            issuer: config["JwtSettings:Issuer"],
            audience: config["JwtSettings:Audience"],
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