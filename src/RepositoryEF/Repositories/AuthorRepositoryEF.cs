using Entities.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace RepositoryEF.Repositories
{
    public class AuthorRepositoryEF : IAuthorRepository
    {
        private readonly LibraryEFContext context;
        public AuthorRepositoryEF(LibraryEFContext libraryEFContext)
        {
            context = libraryEFContext;
        }

        public List<Author> GetAllAuthors()
        {
            return context.Authors.AsNoTracking().ToList();
        }

        public IQueryable<Author> GetAuthors()
        {
            return context.Authors.AsQueryable();
        }

        public Author GetAuthor(int id)
        {
            Author? author = context.Authors.AsNoTracking().FirstOrDefault(a => a.Id == id);

            if (author is null)
            {
                throw new KeyNotFoundException();
            }

            return author;
        }

        public void CreateAuthor(Author author)
        {
            if (context.Authors.Any(a => a.Id == author.Id))
            {
                throw new ArgumentException();
            }
            context.Add(author);
            context.SaveChanges();
        }

        public void UpdateAuthor(Author temp)
        {
            Author? author = context.Authors.Find(temp.Id);
            if (author == null)
            {
                throw new KeyNotFoundException();
            }
            
            author.FirstName = string.IsNullOrEmpty(temp.FirstName) ? author.FirstName : temp.FirstName;
            author.LastName = string.IsNullOrEmpty(temp.LastName) ? author.LastName : temp.LastName;
            author.BornDate = temp.BornDate == DateTime.MinValue ? author.BornDate : temp.BornDate;
            author.Description = string.IsNullOrEmpty(temp.Description) ? author.Description : temp.Description;
            author.Tags = string.IsNullOrEmpty(temp.Tags) ? author.Tags : temp.Tags;
            context.SaveChanges();
        }

        public void DeleteAuthor(int id)
        {
            context.Authors.Remove(GetAuthor(id));
            context.SaveChanges();
        }
    }
}
