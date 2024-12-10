using Entities.Models;

namespace Core.Repositories;

public interface IReaderRepository
{
    public List<Reader> GetAllReaders();
    public IQueryable<Reader> GetReaders();
    public Reader GetReader(int id);
    public void CreateReader(Reader category);
    public void UpdateReader(Reader category);
    public void DeleteReader(int id);
}
