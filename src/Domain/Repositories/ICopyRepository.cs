using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Domain.Repositories;

public interface ICopyRepository
{
    public List<Copy> GetAllCopies();
    public IQueryable<Copy> GetCopies();
    public Copy? GetCopy(Guid id);
    public void CreateCopy(Copy category);
    public void UpdateCopy(Copy category);
    public void DeleteCopy(Guid id);
}
