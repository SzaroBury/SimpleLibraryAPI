namespace Entities.Models
{
    public class Borrowing
    {
        public int Id { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime ExpectedReturnDate => StartedDate.AddDays(14);  
        public DateTime? ActualReturnDate { get; set; } = null;
        public int CopyId { get; set; }
        public Copy? Copy { get; set; }
        public int ReaderId { get; set; }
        public Reader? Reader { get; set; }
    }
}
