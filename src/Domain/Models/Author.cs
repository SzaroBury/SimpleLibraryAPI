namespace SimpleLibrary.Domain.Models;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime? BornDate { get; set; }
    public string Tags { get; set; } = "";
}
