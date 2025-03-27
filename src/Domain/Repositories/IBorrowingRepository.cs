using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Domain.Repositories;

public interface IBorrowingRepository
{
    public List<Borrowing> GetAllBorrowings();
    public IQueryable<Borrowing> GetBorrowings();
    public Borrowing? GetBorrowing(Guid id);
    public void CreateBorrowing(Borrowing category);
    public void UpdateBorrowing(Borrowing category);
    public void DeleteBorrowing(Guid id);
}

