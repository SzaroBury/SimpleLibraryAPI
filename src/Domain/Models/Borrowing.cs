using System.ComponentModel.DataAnnotations;

namespace SimpleLibrary.Domain.Models;

public class Borrowing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime StartedDate { get; set; }
    public DateTime ExpectedReturnDate => StartedDate.AddDays(14);  
    public DateTime? ActualReturnDate { get; set; } = null;

    [Required] public virtual Copy Copy { get; set; }
    public Guid CopyId { get; set; }
    
    [Required] public virtual Reader Reader { get; set; }
    public Guid ReaderId { get; set; }
}