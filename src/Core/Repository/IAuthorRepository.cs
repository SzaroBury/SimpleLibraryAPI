using Entities.Models;

namespace Core.Repositories;

public interface IAuthorRepository
{
    public List<Author> GetAllAuthors();
    public IQueryable<Author> GetAuthors();
    public Author GetAuthor(int id);
    public void CreateAuthor(Author author);
    public void UpdateAuthor(Author author);
    public void DeleteAuthor(int id);
}
