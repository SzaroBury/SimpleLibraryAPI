﻿using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Domain.Repositories;

public interface IAuthorRepository
{
    public List<Author> GetAllAuthors();
    public IQueryable<Author> GetAuthors();
    public Author? GetAuthor(Guid id);
    public void CreateAuthor(Author author);
    public void UpdateAuthor(Author author);
    public void DeleteAuthor(Guid id);
}
