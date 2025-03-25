using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface IAuthorService
{
    public List<Author> SearchAuthors(string? searchTerm = null, string? olderThan = null, string? youngerThan = null, int page = 1, int pageSize = 25);
    public Author GetAuthorById(int id);
    public Author CreateAuthor(Author author);
    public Author UpdateAuthor(Author author);
    public void DeleteAuthor(int id);
}