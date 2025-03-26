using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category Category);
    Task<Category> UpdateCategoryAsync(Category Category);
    Task DeleteCategoryAsync(int id);
    Task<IEnumerable<Category>> SearchCategoriesAsync(
        string? searchTerm = null, 
        int? parentCategoryId = null,
        int page = 1, 
        int pageSize = 25);
}