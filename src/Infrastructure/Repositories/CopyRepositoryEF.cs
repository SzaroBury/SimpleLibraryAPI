﻿using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;

namespace SimpleLibrary.Infrastructure.Repositories;

public class CopyRepositoryEF : ICopyRepository
{
    private readonly LibraryEFContext context;
    public CopyRepositoryEF(LibraryEFContext libraryEFContext)
    {
        context = libraryEFContext;
    }

    public List<Copy> GetAllCopies()
    {
        return context.Copies.ToList();
    }

    public IQueryable<Copy> GetCopies()
    {
        return context.Copies.AsQueryable();
    }

    public Copy GetCopy(Guid id)
    {
        var copy = context.Copies.FirstOrDefault(c => c.Id == id);
        if (copy == null)
        {
            throw new KeyNotFoundException();
        }
        return copy;
    }

    public void CreateCopy(Copy copy)
    {
        if (context.Copies.Any(c => c.Id == copy.Id))
        {
            throw new ArgumentException();
        }
        context.Copies.Add(copy);
        context.SaveChanges();
    }

    public void UpdateCopy(Copy temp)
    {
        Copy? copy = context.Copies.Find(temp.Id);
        if (copy == null)
        {
            throw new KeyNotFoundException();
        }
        copy.BookId = temp.BookId;
        context.SaveChanges();
    }

    public void DeleteCopy(Guid id)
    {
        context.Copies.Remove(GetCopy(id));
        context.SaveChanges();
    }
}
