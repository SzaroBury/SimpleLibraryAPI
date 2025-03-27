using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Domain.Repositories;

public interface ICategoryRepository
{
    public List<Category> GetAllCategories();
    public IQueryable<Category> GetCategories();
    public Category? GetCategory(Guid id);
    public void CreateCategory(Category category);
    public void UpdateCategory(Category category);
    public void DeleteCategory(Guid id);
}
