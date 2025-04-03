using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.Application.Services;

public class CategoryService: ICategoryService
{
    private readonly IUnitOfWork unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await unitOfWork.GetRepository<Category>().GetAllAsync();
    }
    public async Task<Category> GetCategoryByIdAsync(string id)
    {
        var CategoryGuid = ValidateGuid(id);
        return await GetCategoryByIdAsync(CategoryGuid);
    }
    public async Task<Category> GetCategoryByIdAsync(Guid id)
    {
        return await unitOfWork.GetRepository<Category>().GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"A category with the specified ID ({id}) was not found in the system.");
    }
    public async Task<Category> CreateCategoryAsync(Category category)
    {
        await unitOfWork.GetRepository<Category>().AddAsync(category);
        await unitOfWork.SaveChangesAsync();

        return category;
    }
    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        Category existingCategory = await GetCategoryByIdAsync(category.Id);
        
        unitOfWork.GetRepository<Category>().Update(existingCategory);
        await unitOfWork.SaveChangesAsync();

        return existingCategory;
    }
    public async Task DeleteCategoryAsync(string id)
    {
        var category = await GetCategoryByIdAsync(id);

        await unitOfWork.GetRepository<Category>().DeleteAsync(category.Id);
        await unitOfWork.SaveChangesAsync();
    }
    public Task<IEnumerable<Category>> SearchCategoriesAsync(
        string? searchTerm = null, 
        int? parentCategoryId = null,
        int page = 1, 
        int pageSize = 25)
    {
        if(page < 1)
        {
            throw new ArgumentException($"Page ({page}) must be greater than zero.");
        }
        if(pageSize < 1)
        {
            throw new ArgumentException($"Size of a page ({pageSize}) must be greater than zero.");
        }

        var searchCategorysQuery = unitOfWork.GetRepository<Category>().GetQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            if(searchTerm.Length < 3)
            {
                throw new ArgumentException($"The searching term need to have at least three letters.");
            }
            searchCategorysQuery = searchCategorysQuery.Where(c =>
                c.Name   .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || c.ParentCategory.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            );
        }
        
        var count = searchCategorysQuery.Count();
        if (count > 0  && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException("Invalid page. Not so many Categorys.");
        }

        searchCategorysQuery = searchCategorysQuery.Skip((page - 1) * pageSize);
        searchCategorysQuery = searchCategorysQuery.Count() > pageSize ? searchCategorysQuery.Take(pageSize) : searchCategorysQuery;

        return Task.FromResult(searchCategorysQuery.AsEnumerable());
    }

    private static Guid ValidateGuid(string id)
    {
        if(!Guid.TryParse(id, out var CategoryGuid))
        {
            throw new FormatException("Invalid Category ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return CategoryGuid;
    }
}