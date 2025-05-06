using System.ComponentModel.DataAnnotations;
using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.Domain.Models;

public class Copy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int CopyNumber { get; set; }
    public int ShelfNumber { get; set; }
    public bool IsLost { get; set; } = false;
    public CopyCondition Condition { get; set; } = CopyCondition.New;
    public DateTime AcquisitionDate { get; set; } = DateTime.Today;
    public DateTime? LastInspectionDate { get; set; }

    [Required] public virtual Book Book { get; set; }
    public Guid BookId { get; set; }

    public virtual ICollection<Borrowing> Borrowings { get; set; } = [];
}