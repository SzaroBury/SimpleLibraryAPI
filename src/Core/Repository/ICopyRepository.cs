using Entities.Models;

namespace Core.Repositories;

public interface ICopyRepository
{
    public List<Copy> GetAllCopies();
    public IQueryable<Copy> GetCopies();
    public Copy GetCopy(int id);
    public void CreateCopy(Copy category);
    public void UpdateCopy(Copy category);
    public void DeleteCopy(int id);
}
