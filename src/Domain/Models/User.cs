using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.Domain.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiration { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public UserType Role { get; set; } = UserType.Librarian;
}