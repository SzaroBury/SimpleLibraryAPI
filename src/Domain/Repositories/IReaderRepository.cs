using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Domain.Repositories;

public interface IReaderRepository
{
    public List<Reader> GetAllReaders();
    public IQueryable<Reader> GetReaders();
    public Reader? GetReader(Guid id);
    public void CreateReader(Reader category);
    public void UpdateReader(Reader category);
    public void DeleteReader(Guid id);
}
