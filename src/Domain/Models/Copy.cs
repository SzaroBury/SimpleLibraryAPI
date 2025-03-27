using System.ComponentModel.DataAnnotations;

namespace SimpleLibrary.Domain.Models;

public class Copy
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public required virtual Book Book { get; set; }
    public Guid BookId { get; set; }

    public virtual ICollection<Borrowing> Borrowings { get; set; } = [];
}