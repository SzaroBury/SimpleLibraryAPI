using Entities.Enumerations;
using Entities.Models;

namespace Entities.Interfaces
{
    public interface IBookRepository
    {
        public List<Book> GetBooks();
        public Book GetBook(int id);
        public void CreateBook(Book book);
        public void UpdateBook(Book book);
        public void DeleteBook(int id);
    }
}
