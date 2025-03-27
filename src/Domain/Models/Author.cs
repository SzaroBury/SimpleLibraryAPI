using System.ComponentModel.DataAnnotations;

namespace SimpleLibrary.Domain.Models;

public class Author
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? BornDate { get; set; }
    public string Tags { get; set; } = string.Empty;

    public virtual ICollection<Book> Books { get; set; } = [];
}