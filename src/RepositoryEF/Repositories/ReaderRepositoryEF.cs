using Entities.Interfaces;
using Entities.Models;

namespace RepositoryEF.Repositories
{
    public class ReaderRepositoryEF : IReaderRepository
    {
        private readonly LibraryEFContext context;
        public ReaderRepositoryEF(LibraryEFContext libraryEFContext)
        {
            context = libraryEFContext;
        }

        public List<Reader> GetReaders()
        {
            return context.Readers.ToList();
        }

        public Reader GetReader(int id)
        {
            var reader = context.Readers.FirstOrDefault(c => c.Id == id);
            if (reader == null)
            {
                throw new KeyNotFoundException();
            }
            return reader;
        }

        public void CreateReader(Reader reader)
        {
            if (context.Readers.Any(c => c.Id == reader.Id))
            {
                throw new ArgumentException();
            }
            context.Readers.Add(reader);
            context.SaveChanges();
        }

        public void UpdateReader(Reader temp)
        {
            Reader? reader = context.Readers.Find(temp.Id);
            if (reader == null)
            {
                throw new KeyNotFoundException();
            }
            
            reader.FirstName = string.IsNullOrEmpty(temp.FirstName) ? reader.FirstName : temp.FirstName.Trim();
            reader.LastName = string.IsNullOrEmpty(temp.LastName) ? reader.LastName : temp.LastName.Trim();
            reader.Email = string.IsNullOrEmpty(temp.Email) ? reader.Email : temp.Email.Trim();
            reader.Phone = string.IsNullOrEmpty(temp.Phone) ? reader.Phone : temp.Phone.Trim();
            context.SaveChanges();
        }

        public void DeleteReader(int id)
        {
            context.Readers.Remove(GetReader(id));
            context.SaveChanges();
        }
    }
}
