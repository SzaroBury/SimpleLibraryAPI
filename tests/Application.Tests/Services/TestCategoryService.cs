using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services;
using SimpleLibrary.Application.Commands.Categories;

namespace SimpleLibrary.Tests.Application.Services;

public class TestCategoryService
{
    private readonly Dictionary<string, Guid> guids = DataInitializer.InitializeGuids();
    private readonly Mock<IRepository<Category>> mockCategoryRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly CategoryService categoryService;

    public TestCategoryService()
    {
        mockCategoryRepository = DataInitializer.InitializeCategories(guids);
        unitOfWork = DataInitializer.InitializeUnitOfWorkAsync(guids, mockCategoryRepository: mockCategoryRepository).GetAwaiter().GetResult().Object;

        categoryService = new CategoryService(unitOfWork);
    }

    #region GetAllCategoriesAsync
    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        var result = await categoryService.GetAllCategoriesAsync();

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Category>>(result);
        Assert.Equal(4, result.Count());
    }
    #endregion

    #region GetCategoryByIdAsync
    [Fact]
    public async Task GetCategoryByIdAsync_ValidId_ReturnsCategory()
    {
        var result = await categoryService.GetCategoryByIdAsync(guids["c1"].ToString());

        Assert.NotNull(result);
        Assert.Equal("Novel", result.Name);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_InvalidFormat_ThrowsFormatException()
    {
        var ex = await Assert.ThrowsAsync<FormatException>(() => categoryService.GetCategoryByIdAsync("invalid"));
        Assert.Contains("Invalid category ID format", ex.Message);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_NonExistent_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => categoryService.GetCategoryByIdAsync(id));
        Assert.Contains($"A category with the specified ID ({id}) was not found", ex.Message);
    }
    #endregion

    #region CreateCategoryAsync
    [Fact]
    public async Task CreateCategoryAsync_ValidInput_CreatesCategory()
    {
        var dto = new PostCategoryCommand("Science", "Science books", new List<string> { "physics", "chemistry" }, null);

        var result = await categoryService.CreateCategoryAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Science", result.Name);
        Assert.Contains("physics", result.Tags);
    }

    [Fact]
    public async Task CreateCategoryAsync_InvalidTags_ThrowsFormatException()
    {
        var dto = new PostCategoryCommand("Science", "desc", new List<string> { "tag,withcomma" }, null);

        var ex = await Assert.ThrowsAsync<FormatException>(() => categoryService.CreateCategoryAsync(dto));
        Assert.Equal("Invalid format of tags. Please do not use commas.", ex.Message);
    }
    #endregion

    #region UpdateCategoryAsync
    [Fact]
    public async Task UpdateCategoryAsync_ValidUpdate_UpdatesCategory()
    {
        var dto = new PatchCategoryCommand(guids["c1"].ToString(), "UpdatedName", "UpdatedDescription", new List<string> { "newtag" }, null);
        var result = await categoryService.UpdateCategoryAsync(dto);

        Assert.Equal("UpdatedName", result.Name);
        Assert.Equal("UpdatedDescription", result.Description);
        Assert.Contains("newtag", result.Tags);
    }

    [Fact]
    public async Task UpdateCategoryAsync_SetItselfAsParent_ThrowsInvalidOperationException()
    {
        var dto = new PatchCategoryCommand(guids["c1"].ToString(), null, null, null, guids["c1"].ToString());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => categoryService.UpdateCategoryAsync(dto));
        Assert.Equal("A category cannot be its own parent.", ex.Message);
    }
    #endregion

    #region DeleteCategoryAsync
    [Fact]
    public async Task DeleteCategoryAsync_WithBooks_ThrowsInvalidOperationException()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => categoryService.DeleteCategoryAsync(guids["c1"].ToString()));
        Assert.Equal("There are still books in this category.", ex.Message);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithChildren_ThrowsInvalidOperationException()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => categoryService.DeleteCategoryAsync(guids["c3"].ToString()));
        Assert.Equal("There are still child categories under this category.", ex.Message);
    }

    [Fact]
    public async Task DeleteCategoryAsync_Valid_DeletesCategory()
    {
        await categoryService.DeleteCategoryAsync(guids["c4"].ToString());
        mockCategoryRepository.Verify(r => r.DeleteAsync(guids["c4"]), Times.Once);
    }
    #endregion

    #region SearchCategoriesAsync
    [Fact]
    public async Task SearchCategoriesAsync_ByTerm_ReturnsMatching()
    {
        var result = await categoryService.SearchCategoriesAsync("Fan");

        Assert.NotEmpty(result);
        Assert.All(result, c => Assert.Contains("Fan", c.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchCategoriesAsync_InvalidTerm_ThrowsException()
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => categoryService.SearchCategoriesAsync("xy"));
        Assert.Equal("The searching term need to have at least three letters.", ex.Message);
    }

    [Fact]
    public async Task SearchCategoriesAsync_InvalidPage_ThrowsException()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => categoryService.SearchCategoriesAsync(page: 99));
        Assert.Equal("Invalid page. Not so many categories.", ex.Message);
    }
    #endregion
}