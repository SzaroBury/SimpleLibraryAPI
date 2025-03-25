using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Repositories;

namespace SimpleLibrary.Infrastructure.Repositories;

public class CategoryRepositoryEF : ICategoryRepository
{
    private readonly LibraryEFContext context;
    public CategoryRepositoryEF(LibraryEFContext libraryEFContext)
    {
        context = libraryEFContext;
    }

    public List<Category> GetAllCategories()
    {
        return context.Categories.ToList();
    }

    public IQueryable<Category> GetCategories()
    {
        return context.Categories.AsQueryable();
    }
    
    public Category GetCategory(int id)
    {
        var category = context.Categories.FirstOrDefault(c => c.Id == id);
        if (category == null)
        {
            throw new KeyNotFoundException();
        }
        return category;
    }

    public void CreateCategory(Category category)
    {
        context.Categories.Add(category);
        context.SaveChanges();
    }

    public void UpdateCategory(Category temp)
    {
        Category? category = context.Categories.Find(temp.Id);
        if (category == null)
        {
            throw new KeyNotFoundException();
        }
        category.Name = string.IsNullOrEmpty(temp.Name) ? category.Name : temp.Name;
        category.Description = string.IsNullOrEmpty(temp.Description) ? category.Description : temp.Description;
        category.Tags = string.IsNullOrEmpty(temp.Tags) ? category.Tags : temp.Tags;
        category.ParentCategoryId = temp.ParentCategoryId == null ? category.ParentCategoryId : temp.ParentCategoryId;
        context.SaveChanges();
    }

    public void DeleteCategory(int id)
    {
        context.Categories.Remove(GetCategory(id));
        context.SaveChanges();
    }
}
