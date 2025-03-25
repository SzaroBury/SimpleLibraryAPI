using SimpleLibrary.Domain.Enumerations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibrary.Domain.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = "";
    [Required]
    public DateTime? ReleaseDate { get; set; }
    [Required]
    public Language? Language { get; set; } 
    public string Tags { get; set; } = "";

    public int AuthorId { get; set; } = 0;
    public Author? Author { get; set; }
    public int CategoryId { get; set; } = 0;
    public Category? Category { get; set; }

}