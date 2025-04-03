using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Models;

namespace SimpleLibrary.Application.Services.Abstraction;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(string id);
    Task<Category> CreateCategoryAsync(CategoryPostDTO Category);
    Task<Category> UpdateCategoryAsync(CategoryPatchDTO Category);
    Task DeleteCategoryAsync(string id);
    Task<IEnumerable<Category>> SearchCategoriesAsync(
        string? searchTerm = null, 
        string? parentCategoryId = null,
        int page = 1, 
        int pageSize = 25);
}