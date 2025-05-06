using SimpleLibrary.Domain.Enumerations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibrary.Domain.Models;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public Language Language { get; set; } 
    public string Tags { get; set; } = string.Empty;

    [Required] public virtual Author Author { get; set; }
    public Guid AuthorId { get; set; }
    
    [Required] public virtual Category Category { get; set; }
    public Guid CategoryId { get; set; }

    public virtual ICollection<Copy> Copies { get; set; } = [];
}