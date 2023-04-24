using Entities.Models;

namespace Entities.Interfaces
{
    public interface IAuthorRepository
    {
        public List<Author> GetAuthors();
        public Author GetAuthor(int id);
        public void CreateAuthor(Author author);
        public void UpdateAuthor(Author author);
        public void DeleteAuthor(int id);
    }
}
