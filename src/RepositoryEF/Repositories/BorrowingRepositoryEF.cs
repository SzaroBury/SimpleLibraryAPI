using Entities.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace RepositoryEF.Repositories
{
    public class BorrowingRepositoryEF : IBorrowingRepository
    {
        private readonly LibraryEFContext context;
        public BorrowingRepositoryEF(LibraryEFContext libraryEFContext)
        {
            context = libraryEFContext;
        }

        public List<Borrowing> GetBorrowings()
        {
            return context.Borrowings.Include(b => b.Copy).AsNoTracking().ToList();
        }

        public Borrowing GetBorrowing(int id)
        {
            var borrowing = context.Borrowings.FirstOrDefault(c => c.Id == id);
            if (borrowing == null)
            {
                throw new KeyNotFoundException();
            }
            return borrowing;
        }

        public void CreateBorrowing(Borrowing borrowing)
        {
            if (context.Borrowings.Any(c => c.Id == borrowing.Id))
            {
                throw new ArgumentException();
            }
            context.Borrowings.Add(borrowing);
            context.SaveChanges();
        }

        public void UpdateBorrowing(Borrowing temp)
        {
            Borrowing? borrowing = context.Borrowings.Find(temp.Id);
            if (borrowing == null)
            {
                throw new KeyNotFoundException();
            }
            
            borrowing.StartedDate = temp.StartedDate.Equals(DateTime.MinValue) ? borrowing.StartedDate : temp.StartedDate;
            borrowing.ActualReturnDate = temp.ActualReturnDate.Equals(DateTime.MinValue) ? borrowing.ActualReturnDate : temp.ActualReturnDate;
            borrowing.CopyId = temp.CopyId == 0 ? borrowing.CopyId : temp.CopyId;
            borrowing.ReaderId = temp.ReaderId == 0 ? borrowing.ReaderId : temp.ReaderId;

            context.SaveChanges();
        }

        public void DeleteBorrowing(int id)
        {
            context.Borrowings.Remove(GetBorrowing(id));
            context.SaveChanges();
        }
    }
}
