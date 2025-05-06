using System.Security.Claims;

namespace SimpleLibrary.Infrastructure.Services;

public interface IJwtService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}