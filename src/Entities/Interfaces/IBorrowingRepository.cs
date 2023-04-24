using Entities.Models;

namespace Entities.Interfaces
{
    public interface IBorrowingRepository
    {
        public List<Borrowing> GetBorrowings();
        public Borrowing GetBorrowing(int id);
        public void CreateBorrowing(Borrowing category);
        public void UpdateBorrowing(Borrowing category);
        public void DeleteBorrowing(int id);
    }
}
