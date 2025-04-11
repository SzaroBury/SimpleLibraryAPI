using SimpleLibrary.Application.Services;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;

namespace SimpleLibrary.Tests.Application.Services;

public class TestCopyService
{
    private readonly Dictionary<string, Guid> guids = DataInitializer.InitializeGuids();
    private readonly Mock<IRepository<Copy>> mockCopyRepository;
    private readonly Mock<IRepository<Book>> mockBookRepository;
    private readonly Mock<IRepository<Borrowing>> mockBorrowingRepository;
    private readonly Mock<IUnitOfWork> mockUnitOfWork;
    private readonly CopyService copyService;

    public TestCopyService()
    {
        mockBookRepository = DataInitializer.InitializeBookRepositoryAsync(guids).GetAwaiter().GetResult();
        mockCopyRepository = DataInitializer.InitializeCopiesAsync(guids, mockBookRepository).GetAwaiter().GetResult();
        mockBorrowingRepository = DataInitializer.InitializeBorrowingsAsync(guids, mockCopyRepository).GetAwaiter().GetResult();
        mockUnitOfWork = DataInitializer.InitializeUnitOfWorkAsync(guids, null, null, mockBookRepository, mockCopyRepository, null, mockBorrowingRepository).GetAwaiter().GetResult();
        copyService = new CopyService(mockUnitOfWork.Object);
    }
    #region Get
    [Fact]
    public async Task GetAllCopiesAsync_ReturnsCopies()
    {
        var result = await copyService.GetAllCopiesAsync();

        Assert.IsAssignableFrom<IEnumerable<Copy>>(result);
        Assert.Equal(8, result.Count());
    }

    [Fact]
    public async Task GetCopyByIdAsync_WithValidId_ReturnsCopy()
    {
        var id = guids["b1_c1"];

        var result = await copyService.GetCopyByIdAsync(id.ToString());

        Assert.Equal(guids["b1_c1"], result.Id);
        Assert.Equal(guids["b1"], result.BookId);
        Assert.Equal(1, result.CopyNumber);
        Assert.Equal(1, result.ShelfNumber);
        Assert.Equal(CopyCondition.Good, result.Condition);
        Assert.NotNull(result.LastInspectionDate);
    }

    [Fact]
    public async Task GetCopyByIdAsync_WithInvalidGuidFormat_ThrowsFormatException()
    {
        var invalidId = "not-a-guid";

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.GetCopyByIdAsync(invalidId));

        Assert.Contains("Invalid copy ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task GetCopyByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            copyService.GetCopyByIdAsync(id.ToString()));

        Assert.Contains($"A copy with the specified ID ({id}) was not found in the system.", ex.Message);
    }
    #endregion
    #region CreateCopyAsync
    [Fact]
    public async Task CreateCopyAsync_WithValidData_CreatesCopy()
    {
        var dto = new CopyPostDTO(guids["b4"].ToString(), 1, "New");

        var result = await copyService.CreateCopyAsync(dto);

        Assert.Equal(2, result.CopyNumber);
        Assert.Equal(1, result.ShelfNumber);
        Assert.Equal(CopyCondition.New, result.Condition);
        Assert.Equal(guids["b4"], result.BookId);

        mockCopyRepository.Verify(r => r.AddAsync(It.IsAny<Copy>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCopyAsync_CreateForthCopy_CreatesCopyWithCorrectCopyNumber()
    {
        var dto = new CopyPostDTO(guids["b2"].ToString(), 1);

        var result = await copyService.CreateCopyAsync(dto);

        Assert.Equal(5, result.CopyNumber);
        Assert.Equal(1, result.ShelfNumber);
        Assert.Equal(CopyCondition.New, result.Condition);
        Assert.Equal(guids["b2"], result.BookId);

        mockCopyRepository.Verify(r => r.AddAsync(It.IsAny<Copy>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCopyAsync_WithInvalidBookGuidFormat_ThrowsFormatException()
    {
        var dto = new CopyPostDTO("invalid-book-guid", 1);

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.CreateCopyAsync(dto));

        Assert.Contains("Invalid book ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task CreateCopyAsync_WithNonExistentBookId_ThrowsKeyNotFoundException()
    {
        var bookId = Guid.NewGuid().ToString();
        var dto = new CopyPostDTO(bookId, 1, null, null, null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            copyService.CreateCopyAsync(dto));

        Assert.Contains($"A book with  id '{bookId}' was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task CreateCopyAsync_WithInvalidShelfNumber_ThrowsArgumentException()
    {
        var dto = new CopyPostDTO(guids["b4"].ToString(), 0);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            copyService.CreateCopyAsync(dto));

        Assert.Contains("Shelf number must be greater than zero.", ex.Message);
    }

    [Fact]
    public async Task CreateCopyAsync_WithInvalidConditionValue_ThrowsFormatException()
    {
        var dto = new CopyPostDTO(guids["b4"].ToString(), 1, "BrandNew");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.CreateCopyAsync(dto));

        Assert.Contains($"Invalid copy condition format. Please use: New, Good, Bad.", ex.Message);
    }

    [Fact]
    public async Task CreateCopyAsync_WithInvalidAcquisitionDate_ThrowsFormatException()
    {
        var dto = new CopyPostDTO(guids["b4"].ToString(), 1, AcquisitionDate: "invalida-date-format");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.CreateCopyAsync(dto));

        Assert.Contains($"Invalid acquisition date format. Please use DD-MM-YYYY format.", ex.Message);
    }

    [Fact]
    public async Task CreateCopyAsync_WithInvalidLastInspectionDate_ThrowsFormatException()
    {
        var dto = new CopyPostDTO(guids["b4"].ToString(), 1, LastInspectionDate: "invalida-date-format");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.CreateCopyAsync(dto));

        Assert.Contains($"Invalid last inspection date format. Please use DD-MM-YYYY format.", ex.Message);
    }
    #endregion
    #region UpdateCopyAsync
    [Theory]
    [InlineData("b2", 2,    true, "Bad", 3,    "b2", 2, true,  CopyCondition.Bad, 3)]
    [InlineData("b2", null, null, null,  null, "b2", 1, false, CopyCondition.Good, 1)]
    [InlineData(null, 2,    null, null,  null, "b1", 2, false, CopyCondition.Good, 1)]
    [InlineData(null, null, true, null,  null, "b1", 1, true,  CopyCondition.Good, 1)]
    [InlineData(null, null, null, "Bad", null, "b1", 1, false, CopyCondition.Bad, 1)]
    [InlineData(null, null, null, null,  3,    "b1", 1, false, CopyCondition.Good, 3)]
    public async Task UpdateCopyAsync_UpdateSpecificProperties_UpdatesCopy(
        string? bookId, int? shelfNumber, bool? isLost, string? condition, int? copyNumber,
        string expectedBookId, int expectedShelfNumber, bool expectedIsLost, CopyCondition expectedCondition, int expectedCopyNumber)
    {
        var book = !string.IsNullOrEmpty(bookId) ? guids[bookId].ToString() : null;
        var dto = new CopyPutDTO(guids["b1_c1"].ToString(), book, shelfNumber, isLost, condition, CopyNumber: copyNumber);

        var result = await copyService.UpdateCopyAsync(dto);
        Assert.Equal(guids[expectedBookId], result.BookId);
        Assert.Equal(expectedShelfNumber, result.ShelfNumber);
        Assert.Equal(expectedIsLost, result.IsLost);
        Assert.Equal(expectedCondition, result.Condition);
        Assert.Equal(expectedCopyNumber, result.CopyNumber);

        mockCopyRepository.Verify(r => r.Update(It.Is<Copy>(
            c => c.Id == guids["b1_c1"] 
            && c.BookId == guids[expectedBookId] 
            && c.ShelfNumber == expectedShelfNumber 
            && c.IsLost == expectedIsLost 
            && c.Condition == expectedCondition
            && c.CopyNumber == expectedCopyNumber
        )), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCopyAsync_ValidAcquisitionDate_UpdatesCopy()
    {
        var dto = new CopyPutDTO(guids["b1_c1"].ToString(), AcquisitionDate: "01-01-2020");
        var expectedAcquisitionDate = new DateTime(2020, 1, 1);

        var result = await copyService.UpdateCopyAsync(dto);

        Assert.Equal(expectedAcquisitionDate, result.AcquisitionDate);
        Assert.Equal(1, result.ShelfNumber);
        Assert.False(false);
        Assert.Equal(CopyCondition.Good, result.Condition);
        Assert.Equal(1, result.CopyNumber);
        mockCopyRepository.Verify(r => r.Update(It.Is<Copy>(c => 
            c.Id == guids["b1_c1"] && c.ShelfNumber == 1 && !c.IsLost && c.Condition == CopyCondition.Good && c.CopyNumber == 1
            && c.AcquisitionDate == expectedAcquisitionDate
        )), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCopyAsync_ValidLastInspectionDate_UpdatesCopy()
    {
        var dto = new CopyPutDTO(guids["b1_c1"].ToString(), LastInspectionDate: "01-01-2025");
        var expectedLastInspectionDate = new DateTime(2025, 1, 1);

        var result = await copyService.UpdateCopyAsync(dto);

        Assert.Equal(expectedLastInspectionDate, result.LastInspectionDate);
        Assert.Equal(1, result.ShelfNumber);
        Assert.False(false);
        Assert.Equal(CopyCondition.Good, result.Condition);
        Assert.Equal(1, result.CopyNumber);
        mockCopyRepository.Verify(r => r.Update(It.Is<Copy>(c => 
            c.Id == guids["b1_c1"] && c.ShelfNumber == 1 && !c.IsLost && c.Condition == CopyCondition.Good && c.CopyNumber == 1
            && c.LastInspectionDate == expectedLastInspectionDate
        )), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithInvalidCopyGuid_ThrowsFormatException()
    {
        var dto = new CopyPutDTO("invalid-guid");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains("Invalid copy ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithNonExistentCopyId_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid().ToString();
        var dto = new CopyPutDTO(id);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains($"A copy with the specified ID ({id}) was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithInvalidBookGuid_ThrowsFormatException()
    {
        var id = guids["b1_c1"].ToString();
        var dto = new CopyPutDTO(id, "invalid-book-guid");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains("Invalid book ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithNonExistentBookId_ThrowsKeyNotFoundException()
    {
        var id = guids["b1_c1"].ToString();
        var nonExistentBook = Guid.NewGuid().ToString();
        var dto = new CopyPutDTO(id, nonExistentBook);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains($"A book with the specified ID ({nonExistentBook}) was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithInvalidShelfNumber_ThrowsArgumentException()
    {
        var id = guids["b1_c1"].ToString();
        var dto = new CopyPutDTO(id, guids["b4"].ToString(), 0);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains("Shelf number must be greater than zero.", ex.Message);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithInvalidConditionValue_ThrowsFormatException()
    {
        var id = guids["b1_c1"].ToString();
        var dto = new CopyPutDTO(id, guids["b4"].ToString(), 1, Condition: "BrandNew");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains($"Invalid copy condition format. Please use: New, Good, Bad.", ex.Message);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithInvalidAcquisitionDate_ThrowsFormatException()
    {
        var id = guids["b1_c1"].ToString();
        var dto = new CopyPutDTO(id, guids["b4"].ToString(), 1, AcquisitionDate: "invalid-date-format");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains($"Invalid acquisition date format. Please use DD-MM-YYYY format.", ex.Message);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithInvalidLastInspectionDate_ThrowsFormatException()
    {
        var id = guids["b1_c1"].ToString();
        var dto = new CopyPutDTO(id, guids["b4"].ToString(), 1, LastInspectionDate: "invalid-date-format");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains($"Invalid last inspection date format. Please use DD-MM-YYYY format.", ex.Message);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithInvalidCopyNumber_ThrowsArgumentException()
    {
        var dto = new CopyPutDTO(guids["b1_c1"].ToString(), CopyNumber: 0);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains("Copy number must be greater than zero.", ex.Message);
    }

    [Fact]
    public async Task UpdateCopyAsync_WithDuplicateCopyNumber_ThrowsArgumentException()
    {
        var dto = new CopyPutDTO(guids["b1_c1"].ToString(), CopyNumber: 2);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            copyService.UpdateCopyAsync(dto));

        Assert.Contains("The specified copy number (2) is already taken by some else copy of the book.", ex.Message);
    }
    #endregion
    #region DeleteCopyAsync
    [Fact]
    public async Task DeleteCopyAsync_WithInactiveBorrowings_DeletesCopyAndBorrowings()
    {
        var copyId = guids["b3_c5"];
        var borrowingId = guids["bor4"];
        await copyService.DeleteCopyAsync(copyId.ToString());

        mockCopyRepository.Verify(r => r.DeleteAsync(It.Is<Guid>(guid => guid == copyId)), Times.Once);
        mockBorrowingRepository.Verify(r => r.DeleteAsync(It.Is<Guid>(guid => guid == borrowingId)), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCopyAsync_WithActiveBorrowing_ThrowsInvalidOperationException()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            copyService.DeleteCopyAsync(guids["b1_c1"].ToString()));
        
        Assert.Equal("You must not delete the copy, because it is still borrowed. If it is lost, mark it with IsLost flag with Update request.", exception.Message);
    }
    #endregion
    #region SearchCopiesAsync
    [Theory]
    [InlineData("Dziady", new[] { "b4_c6"})]
    [InlineData("Adam", new[] { "b4_c6"})]
    [InlineData("Mickiewicz", new[] { "b4_c6"})]
    [InlineData("old", new[] { "b1_c1", "b1_c2", "b2_c3", "b2_c4", "b2_c7"})]
    [InlineData("new", new[] { "b3_c5"})]
    public async Task SearchCopiesAsync_WithSearchTerm_FiltersCopies(string searchTerm, string[] expectedCopies)
    {
        var result = await copyService.SearchCopiesAsync(searchTerm);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Copy>>(result);
        Assert.Equal(expectedCopies.Length, result.Count());
        foreach (var expectedCopy in expectedCopies)
        {
            Assert.Contains(result, c => c.Id == guids[expectedCopy]);
        }
    }

    [Fact]
    public async Task SearchCopiesAsync_WithBookId_FiltersCopies()
    {
        var result = await copyService.SearchCopiesAsync(bookId: guids["b1"].ToString());

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Copy>>(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == guids["b1_c1"]);
        Assert.Contains(result, c => c.Id == guids["b1_c2"]);
    }

    [Fact]
    public async Task SearchCopiesAsync_WithIsAvailableFlag_ReturnAvailableCopies()
    {
        var result = await copyService.SearchCopiesAsync(bookId: guids["b1"].ToString(), isAvailable: true);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Copy>>(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Id == guids["b1_c2"]);
    }

    [Fact]
    public async Task SearchCopiesAsync_WithIsAvailableFlag_ReturnUnavailableCopies()
    {
        var result = await copyService.SearchCopiesAsync(bookId: guids["b2"].ToString(), isAvailable: false);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Copy>>(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == guids["b2_c3"]);
        Assert.Contains(result, c => c.Id == guids["b2_c7"]);
    }

    [Fact]
    public async Task SearchCopiesAsync_WithPageOutOfRange_ThrowsArgumentException()
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            copyService.SearchCopiesAsync("book", page: 0));

        Assert.Contains("Page must be greater than zero.", ex.Message);
    }

    [Fact]
    public async Task SearchCopiesAsync_WithPageSizeOutOfTheRange_ThrowsArgumentException()
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            copyService.SearchCopiesAsync("Test", pageSize: 0));

        Assert.Contains("Size of a page must be greater than zero.", ex.Message);
    }

    [Theory]
    [InlineData(1, 1, new[] { "b1_c1" })]
    [InlineData(1, 2, new[] { "b1_c1", "b1_c2" })]
    [InlineData(2, 1, new[] { "b1_c2" })]
    [InlineData(2, 3, new[] { "b2_c4", "b3_c5", "b4_c6" })]
    [InlineData(3, 3, new string[] { "b2_c7", "b6_c8" })]
    public async Task SearchCopiesAsync_Pagination_ReturnsExpectedResults(int page, int pageSize, string[] expectedCopies)
    {
        // Act
        var response = await copyService.SearchCopiesAsync(page: page, pageSize: pageSize);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Copy>>(response);
        Assert.Equal(expectedCopies.Length, response.Count());
        foreach (var expectedCopy in expectedCopies)
        {
            Assert.Contains(response, c => c.Id == guids[expectedCopy]);
        }
    }

    [Fact]
    public async Task SearchCopiesAsync_WithShortSearchTerm_ThrowsArgumentException()
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            copyService.SearchCopiesAsync("ab"));

        Assert.Contains("The searching term need to have at least three letters.", ex.Message);
    }

    [Fact]
    public async Task SearchCopiesAsync_InvalidBookIdFormat_ThrowsFormatException()
    {
        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            copyService.SearchCopiesAsync(bookId: "invalid-book-id"));

        Assert.Contains("Invalid book ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task SearchCopiesAsync_NonExistantBook_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid().ToString();
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            copyService.SearchCopiesAsync(bookId: id));

        Assert.Contains($"A book with the specified ID ({id}) was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task SearchCopiesAsync_WithShortSearchTerm_ThrowsInvalidOperationException()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            copyService.SearchCopiesAsync("", page: 10, pageSize: 10));

        Assert.Contains("Invalid page. Not so many copies.", ex.Message);
    }
    #endregion
}