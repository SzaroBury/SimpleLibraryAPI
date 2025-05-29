using SimpleLibrary.Domain.Models;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Application.Commands.Categories;

namespace SimpleLibrary.Application.Services;

public class CategoryService: ICategoryService
{
    private readonly IUnitOfWork uow;

    public CategoryService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await uow.GetRepository<Category>().GetAllAsync();
    }
    public async Task<Category> GetCategoryByIdAsync(string id)
    {
        var categoryGuid = ValidateGuid(id);
        return await GetCategoryByIdAsync(categoryGuid);
    }
    public async Task<Category> GetCategoryByIdAsync(Guid id)
    {
        return await uow.GetRepository<Category>().GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"A category with the specified ID ({id}) was not found in the system.");
    }
    public async Task<Category> CreateCategoryAsync(PostCategoryCommand category)
    {
        ValidateName(category.Name);

        string tagsInString = "";
        if(category.Tags is not null)
        {
            tagsInString = ValidateAndFormatTags(category.Tags);
        }

        Category? parentCategory = null;
        if(category.ParentCategoryId.HasValue)
        {
            parentCategory = await GetCategoryByIdAsync(category.ParentCategoryId.Value);
        }

        Category newCategory = new()
        {
            Name = category.Name,
            Description = category.Description,
            Tags = tagsInString,
            ParentCategoryId = parentCategory?.Id ?? null
        };

        await uow.GetRepository<Category>().AddAsync(newCategory);
        await uow.SaveChangesAsync();

        return newCategory;
    }
    public async Task<Category> UpdateCategoryAsync(PatchCategoryCommand category)
    {
        Category existingCategory = await GetCategoryByIdAsync(category.Id);

        if(category.Name is not null)
        {
            existingCategory.Name = ValidateName(category.Name);
        }

        if(category.Description is not null)
        {
            existingCategory.Description = category.Description;
        }

        if(category.Tags is not null)
        {
            existingCategory.Tags = ValidateAndFormatTags(category.Tags);
        }

        if(category.ParentCategoryId.HasValue)
        {
            if (category.ParentCategoryId == category.Id)
            {
                throw new InvalidOperationException("A category cannot be its own parent.");
            }
            var parentCategory = await GetCategoryByIdAsync(category.ParentCategoryId.Value);

            existingCategory.ParentCategoryId = parentCategory.Id;
        }
        
        uow.GetRepository<Category>().Update(existingCategory);
        await uow.SaveChangesAsync();

        return existingCategory;
    }
    public async Task DeleteCategoryAsync(string id)
    {
        var category = await GetCategoryByIdAsync(id);

        var booksInCategory = uow.GetRepository<Book>().GetQueryable().Any(b => b.CategoryId == category.Id);
        if(booksInCategory)
        {
            throw new InvalidOperationException("There are still books in this category.");
        }

        bool anyChildCategories = uow.GetRepository<Category>().GetQueryable().Any(c => c.ParentCategoryId == category.Id);
        if(anyChildCategories)
        {
            throw new InvalidOperationException("There are still child categories under this category.");
        }

        await uow.GetRepository<Category>().DeleteAsync(category.Id);
        await uow.SaveChangesAsync();
    }
    public Task<IEnumerable<Category>> SearchCategoriesAsync(
        string? searchTerm = null, 
        string? parentCategoryId = null,
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

        var searchCategoriesQuery = uow.GetRepository<Category>().GetQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            if(searchTerm.Length < 3)
            {
                throw new ArgumentException($"The searching term need to have at least three letters.");
            }
            searchCategoriesQuery = searchCategoriesQuery.Where(c =>
                c.Name   .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || (c.ParentCategory != null && c.ParentCategory.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase))
            );
        }

        if(parentCategoryId is not null)
        {
            var parentCategoryGuid = ValidateGuid(parentCategoryId);
            searchCategoriesQuery = searchCategoriesQuery.Where(c => c.ParentCategoryId == parentCategoryGuid);
        }
        
        var count = searchCategoriesQuery.Count();
        if (count > 0  && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException("Invalid page. Not so many categories.");
        }

        searchCategoriesQuery = searchCategoriesQuery.Skip((page - 1) * pageSize).Take(pageSize);

        return Task.FromResult(searchCategoriesQuery.AsEnumerable());
    }

    private static Guid ValidateGuid(string id, string entity = "category")
    {
        if(!Guid.TryParse(id, out var categoryGuid))
        {
            throw new FormatException($"Invalid {entity} ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return categoryGuid;
    }

    private static string ValidateAndFormatTags(IEnumerable<string> tags)
    {
        string result = tags.Count() > 1 
            ? string.Join(',', tags.Select(t => t.ToLower())) 
            : tags.First() ?? "";

        return result;
    }

    private static string ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name of a new category must not be blank.");
        }
        return name;
    }
}