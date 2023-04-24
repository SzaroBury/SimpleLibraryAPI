using Entities.Models;

namespace Entities.Interfaces
{
    public interface ICopyRepository
    {
        public List<Copy> GetCopies();
        public Copy GetCopy(int id);
        public void CreateCopy(Copy category);
        public void UpdateCopy(Copy category);
        public void DeleteCopy(int id);
    }
}
