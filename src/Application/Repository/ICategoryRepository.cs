﻿using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Repositories;

public interface ICategoryRepository
{
    public List<Category> GetAllCategories();
    public IQueryable<Category> GetCategories();
    public Category GetCategory(int id);
    public void CreateCategory(Category category);
    public void UpdateCategory(Category category);
    public void DeleteCategory(int id);
}
