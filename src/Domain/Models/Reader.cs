namespace SimpleLibrary.Domain.Models;

public class Reader
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string CardNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsBanned  { get; set; } = false;
    public DateTime? BannedDate { get; set; }

    public virtual ICollection<Borrowing> Borrowings { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}".Trim();
}