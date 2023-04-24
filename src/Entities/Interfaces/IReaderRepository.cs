using Entities.Models;

namespace Entities.Interfaces
{
    public interface IReaderRepository
    {
        public List<Reader> GetReaders();
        public Reader GetReader(int id);
        public void CreateReader(Reader category);
        public void UpdateReader(Reader category);
        public void DeleteReader(int id);
    }
}
