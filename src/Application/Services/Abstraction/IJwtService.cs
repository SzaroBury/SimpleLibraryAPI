using System.Security.Claims;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IJwtService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}