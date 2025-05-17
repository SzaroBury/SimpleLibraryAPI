using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services;
using SimpleLibrary.Application.Commands.Borrowings;

namespace SimpleLibrary.Tests.Application.Services;

public class TestBorrowingService
{
    private readonly Dictionary<string, Guid> guids = DataInitializer.InitializeGuids();
    private readonly Mock<IRepository<Borrowing>> mockBorrowingRepository;
    private readonly Mock<IUnitOfWork> mockUnitOfWork;
    private readonly BorrowingService borrowingService;

    public TestBorrowingService()
    {
        mockBorrowingRepository = DataInitializer.InitializeBorrowingsAsync(guids).GetAwaiter().GetResult();
        mockUnitOfWork = DataInitializer.InitializeUnitOfWorkAsync(guids, null, null, null, null, null, mockBorrowingRepository).GetAwaiter().GetResult();
        borrowingService = new BorrowingService(mockUnitOfWork.Object);
    }
    #region Get
    [Fact]
    public async Task GetAllBorrowingsAsync_ReturnsBorrowings()
    {
        var result = await borrowingService.GetAllBorrowingsAsync();

        Assert.IsAssignableFrom<IEnumerable<Borrowing>>(result);
        Assert.Equal(6, result.Count());
    }

    [Fact]
    public async Task GetBorrowingByIdAsync_WithValidId_ReturnsBorrowing()
    {
        var id = guids["bor1"];
        var targetStartedDate = DateTime.Now.AddDays(-5);

        var result = await borrowingService.GetBorrowingByIdAsync(id.ToString());

        Assert.Equal(guids["bor1"], result.Id);
        Assert.Equal(guids["b1_c1"], result.CopyId);
        Assert.Equal(guids["r1"], result.ReaderId);
        Assert.Equal(targetStartedDate.Hour, result.StartedDate.Hour);
        Assert.Equal(targetStartedDate.Month, result.StartedDate.Month);
        Assert.Equal(targetStartedDate.Day, result.StartedDate.Day);
        Assert.Null(result.ActualReturnDate);
    }

    [Fact]
    public async Task GetBorrowingByIdAsync_WithInvalidGuidFormat_ThrowsFormatException()
    {
        var invalidId = "not-a-guid";

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.GetBorrowingByIdAsync(invalidId));

        Assert.Contains("Invalid borrowing ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task GetBorrowingByIdAsync_WithNonExistantId_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            borrowingService.GetBorrowingByIdAsync(id.ToString()));

        Assert.Contains($"A borrowing with the specified ID ({id}) was not found in the system.", ex.Message);
    }
    #endregion
    #region CreateBorrowingAsync
    [Fact]
    public async Task CreateBorrowingAsync_WithValidData_CreatesBorrowing()
    {
        var copyId = guids["b1_c2"];
        var readerId = guids["r2"];
        var startedDate = DateTime.Today.AddDays(-10);
        var actualReturnDate = DateTime.Today.AddDays(-5);
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId.ToString(), startedDate.ToString("yyyy-MM-dd HH:mm"), actualReturnDate.ToString("yyyy-MM-dd HH:mm"));

        var result = await borrowingService.CreateBorrowingAsync(dto);

        Assert.Equal(guids["b1_c2"], result.CopyId);
        Assert.Equal(guids["r2"], result.ReaderId);
        Assert.Equal(startedDate, result.StartedDate);
        Assert.Equal(actualReturnDate, result.ActualReturnDate);

        mockBorrowingRepository.Verify(b => b.AddAsync(It.IsAny<Borrowing>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithValidDataWithoutDates_CreatesBorrowingWithDefaultsDates()
    {
        var copyId = guids["b1_c2"];
        var readerId = guids["r2"];
        var targetStartedDate = DateTime.Now;
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId.ToString());

        var result = await borrowingService.CreateBorrowingAsync(dto);

        Assert.Equal(guids["b1_c2"], result.CopyId);
        Assert.Equal(guids["r2"], result.ReaderId);
        Assert.Equal(targetStartedDate.Year, result.StartedDate.Year);
        Assert.Equal(targetStartedDate.Month, result.StartedDate.Month);
        Assert.Equal(targetStartedDate.Day, result.StartedDate.Day);
        Assert.Equal(targetStartedDate.Hour, result.StartedDate.Hour);
        Assert.Equal(targetStartedDate.Minute, result.StartedDate.Minute);
        Assert.Null(result.ActualReturnDate);

        mockBorrowingRepository.Verify(r => r.AddAsync(It.IsAny<Borrowing>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithInvalidReaderGuidFormat_ThrowsFormatException()
    {
        var copyId = guids["b1_c2"];
        var dto = new PostBorrowingCommand(copyId.ToString(), "invalid-reader-guid");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains("Invalid reader ID format", ex.Message);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithNonExistingCopyId_ThrowsKeyNotFoundException()
    {
        var copyId = Guid.NewGuid().ToString(); // nie istnieje w bazie mocków
        var readerId = guids["r2"];
        var dto = new PostBorrowingCommand(copyId, readerId.ToString());

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains($"A copy with the specified ID ({copyId}) was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithNonExistingReaderId_ThrowsKeyNotFoundException()
    {
        var readerId = Guid.NewGuid().ToString(); // nie istnieje
        var copyId = guids["b1_c2"];
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains($"A reader with the specified ID ({readerId}) was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithCopyMarkedAsLost_ThrowsInvalidOperationException()
    {
        var copyId = guids["b2_c7"]; // ustaw kopię jako IsLost=true w DataInitializer
        var readerId = guids["r2"];
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId.ToString());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains("is marked as lost and cannot be borrowed", ex.Message);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithCopyAlreadyBorrowed_ThrowsInvalidOperationException()
    {
        var copyId = guids["b1_c1"]; // ma aktywne wypożyczenie (ActualReturnDate == null)
        var readerId = guids["r2"];
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId.ToString());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains("is currently borrowed and cannot be borrowed again", ex.Message);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithReaderBanned_ThrowsInvalidOperationException()
    {
        var copyId = guids["b1_c2"];
        var readerId = guids["r4"]; // z IsBanned=true
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId.ToString());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains("is banned and cannot borrow more books", ex.Message);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithInvalidStartedDate_ThrowsFormatException()
    {
        var copyId = guids["b1_c2"];
        var readerId = guids["r2"];
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId.ToString(), "not-a-date");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains("is invalid started date format", ex.Message);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithInvalidActualReturnDate_ThrowsFormatException()
    {
        var copyId = guids["b1_c2"];
        var readerId = guids["r2"];
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId.ToString(), DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd"), "not-a-date");

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains("is invalid actual return date format", ex.Message);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithStartedDateInFuture_ThrowsInvalidOperationException()
    {
        var copyId = guids["b1_c2"];
        var readerId = guids["r2"];
        var futureDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd");
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId.ToString(), futureDate);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains("cannot be set to a future date", ex.Message);
    }

    [Fact]
    public async Task CreateBorrowingAsync_WithActualReturnDateBeforeStartedDate_ThrowsInvalidOperationException()
    {
        var copyId = guids["b1_c2"];
        var readerId = guids["r2"];
        var started = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
        var returned = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd");
        var dto = new PostBorrowingCommand(copyId.ToString(), readerId.ToString(), started, returned);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.CreateBorrowingAsync(dto));

        Assert.Contains("must not be set after the actual return date", ex.Message);
    }
    #endregion
    #region UpdateBorrowingAsync
    [Fact]
    public async Task UpdateBorrowingAsync_WithValidStartedDate_UpdatesStartedDate()
    {
        var borrowingId = guids["bor1"];
        var newStartedDate = DateTime.Today.AddDays(-10);
        var dto = new PatchBorrowingCommand(borrowingId.ToString(), StartedDate: newStartedDate.ToString("yyyy-MM-dd HH:mm"));

        // Act
        var result = await borrowingService.UpdateBorrowingAsync(dto);

        // Assert
        Assert.Equal(newStartedDate, result.StartedDate);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithValidActualReturnDate_UpdatesReturnDate()
    {
        // Arrange
        var borrowingId = guids["bor1"];
        var newReturnDate = DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd");

        var dto = new PatchBorrowingCommand(
            borrowingId.ToString(),
            ActualReturnDate: newReturnDate
        );

        // Act
        var result = await borrowingService.UpdateBorrowingAsync(dto);

        // Assert
        Assert.Equal(DateTime.Parse(newReturnDate), result.ActualReturnDate);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithEmptyActualReturnDate_SetsNull()
    {
        var borrowingId = guids["bor1"];
        var dto = new PatchBorrowingCommand(
            borrowingId.ToString(),
            ActualReturnDate: ""
        );

        var result = await borrowingService.UpdateBorrowingAsync(dto);

        Assert.Null(result.ActualReturnDate);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithValidReaderId_UpdatesReader()
    {
        var borrowingId = guids["bor1"];
        var newReaderId = guids["r2"];

        var dto = new PatchBorrowingCommand(
            borrowingId.ToString(),
            ReaderId: newReaderId.ToString()
        );

        var result = await borrowingService.UpdateBorrowingAsync(dto);

        Assert.Equal(newReaderId, result.ReaderId);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithValidCopyId_UpdatesCopy()
    {
        var borrowingId = guids["bor1"];
        var newCopyId = guids["b1_c2"];

        var dto = new PatchBorrowingCommand(
            borrowingId.ToString(),
            CopyId: newCopyId.ToString()
        );

        var result = await borrowingService.UpdateBorrowingAsync(dto);

        Assert.Equal(newCopyId, result.CopyId);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithAllFields_UpdatesAllFields()
    {
        var borrowingId = guids["bor1"];
        var newReaderId = guids["r2"];
        var newCopyId = guids["b1_c2"];
        var startedDate = DateTime.Now.AddDays(-14).ToString("yyyy-MM-dd");
        var returnDate = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");

        var dto = new PatchBorrowingCommand(
            borrowingId.ToString(),
            newCopyId.ToString(),
            newReaderId.ToString(),
            startedDate,
            returnDate
        );

        var result = await borrowingService.UpdateBorrowingAsync(dto);

        Assert.Equal(newReaderId, result.ReaderId);
        Assert.Equal(newCopyId, result.CopyId);
        Assert.Equal(DateTime.Parse(startedDate), result.StartedDate);
        Assert.Equal(DateTime.Parse(returnDate), result.ActualReturnDate);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithEmptyStartedDate_ThrowsArgumentException()
    {
        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            StartedDate: ""
        );

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Equal("The start date cannot be empty.", ex.Message);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithInvalidStartedDateFormat_ThrowsFormatException()
    {
        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            StartedDate: "32-13-2023" // błąd
        );

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Contains("is invalid started date format", ex.Message);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithFutureStartedDate_ThrowsInvalidOperationException()
    {
        var futureDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");

        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            StartedDate: futureDate
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Equal("The start date cannot be set to a future date.", ex.Message);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithInvalidActualReturnDateFormat_ThrowsFormatException()
    {
        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            ActualReturnDate: "not-a-date"
        );

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Contains("is invalid actual return date format", ex.Message);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithFutureActualReturnDate_ThrowsInvalidOperationException()
    {
        var futureReturnDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd");

        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            ActualReturnDate: futureReturnDate
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Equal("The actual return date cannot be set to a future date.", ex.Message);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithActualReturnDateBeforeStartedDate_ThrowsInvalidOperationException()
    {
        var invalidReturnDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            ActualReturnDate: invalidReturnDate
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Contains("must not be set after the actual return date", ex.Message);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithInvalidCopyIdFormat_ThrowsFormatException()
    {
        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            CopyId: "not-a-guid"
        );

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Contains("Invalid copy ID format", ex.Message);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithInvalidReaderIdFormat_ThrowsFormatException()
    {
        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            ReaderId: "bad-guid"
        );

        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Contains("Invalid reader ID format", ex.Message);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithBannedReader_ThrowsInvalidOperationException()
    {
        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            ReaderId: guids["r4"].ToString()
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Contains("is banned and cannot borrow", ex.Message);
    }
    [Fact]
    public async Task UpdateBorrowingAsync_WithLostCopy_ThrowsInvalidOperationException()
    {
        var dto = new PatchBorrowingCommand(
            guids["bor1"].ToString(),
            CopyId: guids["b2_c7"].ToString()
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.UpdateBorrowingAsync(dto));

        Assert.Contains("is marked as lost and cannot be borrowed", ex.Message);
    }
    #endregion
    #region DeleteBorrowingAsync
    [Fact]
    public async Task DeleteBorrowingAsync_WithValidId_DeletesBorrowing()
    {
        // Arrange
        var id = guids["bor1"];

        // Act
        await borrowingService.DeleteBorrowingAsync(id.ToString());

        // Assert
        mockBorrowingRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task DeleteBorrowingAsync_WithInvalidGuidFormat_ThrowsFormatException()
    {
        // Arrange
        var invalidId = "not-a-guid";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.DeleteBorrowingAsync(invalidId));

        Assert.Contains("Invalid borrowing ID format", ex.Message);
    }
    [Fact]
    public async Task DeleteBorrowingAsync_WithNonExistantId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid(); // ID nie istnieje w mocku

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            borrowingService.DeleteBorrowingAsync(id.ToString()));

        Assert.Contains($"A borrowing with the specified ID ({id}) was not found", ex.Message);
    }
    #endregion
    #region SearchCopiesAsync
    [Fact]
    public async Task SearchBorrowingsAsync_WithoutFilters_ReturnsAllBorrowings()
    {
        // Act
        var result = await borrowingService.SearchBorrowingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.Count()); // bor1–bor6
    }
    [Fact]
    public async Task SearchBorrowingsAsync_WithSearchTermMatchingBookTitle_ReturnsExpected()
    {
        // Act
        var result = await borrowingService.SearchBorrowingsAsync(searchTerm: "dziady");

        // Assert
        Assert.All(result, b =>
            Assert.Contains("dziady", b.Copy.Book.Title, StringComparison.OrdinalIgnoreCase)
        );
        Assert.Equal(2, result.Count()); // bor5, bor6 (Dziady cz. II)
    }
    [Fact]
    public async Task SearchBorrowingsAsync_WithSearchTermMatchingReaderName_ReturnsExpected()
    {
        // Act
        var result = await borrowingService.SearchBorrowingsAsync(searchTerm: "kowalski");

        // Assert
        Assert.All(result, b =>
            Assert.Contains("kowalski", b.Reader.FullName, StringComparison.OrdinalIgnoreCase)
        );
        Assert.Equal(5, result.Count()); // bor1–bor5 (ReaderId == r1)
    }
    [Fact]
    public async Task SearchBorrowingsAsync_WithOlderThanDate_ReturnsExpected()
    {
        // Arrange
        var olderThan = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd");

        // Act
        var result = await borrowingService.SearchBorrowingsAsync(olderThan: olderThan);

        // Assert
        Assert.All(result, b => Assert.True(b.StartedDate < DateTime.Now.AddDays(-10)));
        Assert.Single(result); // tylko bor5
    }
    [Fact]
    public async Task SearchBorrowingsAsync_WithNewerThanDate_ReturnsExpected()
    {
        // Arrange
        var newerThan = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd");

        // Act
        var result = await borrowingService.SearchBorrowingsAsync(newerThan: newerThan);

        // Assert
        Assert.All(result, b => Assert.True(b.StartedDate > DateTime.Now.AddDays(-10)));
        Assert.Equal(5, result.Count()); // bor1–bor4, bor6
    }
    [Fact]
    public async Task SearchBorrowingsAsync_WithCopyId_ReturnsExpectedBorrowing()
    {
        // Arrange
        var copyId = guids["b1_c1"].ToString();

        // Act
        var result = await borrowingService.SearchBorrowingsAsync(copyId: copyId);

        // Assert
        Assert.Single(result);
        Assert.Equal(guids["b1_c1"], result.First().CopyId);
    }
    [Fact]
    public async Task SearchBorrowingsAsync_WithReaderId_ReturnsExpectedBorrowings()
    {
        // Arrange
        var readerId = guids["r1"].ToString();

        // Act
        var result = await borrowingService.SearchBorrowingsAsync(readerId: readerId);

        // Assert
        Assert.All(result, b => Assert.Equal(guids["r1"], b.ReaderId));
        Assert.Equal(5, result.Count());
    }
    [Theory]
    [InlineData(1, 1, new[] { "bor1" })]
    [InlineData(1, 2, new[] { "bor1", "bor2" })]
    [InlineData(2, 2, new[] { "bor3", "bor4" })]
    [InlineData(3, 2, new[] { "bor5", "bor6" })]
    [InlineData(2, 3, new[] { "bor4", "bor5", "bor6" })]
    public async Task SearchBorrowingsAsync_Pagination_ReturnsExpectedResults(int page, int pageSize, string[] expectedBorrowingIds)
    {
        // Act
        var response = await borrowingService.SearchBorrowingsAsync(page: page, pageSize: pageSize);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Borrowing>>(response);
        Assert.Equal(expectedBorrowingIds.Length, response.Count());

        foreach (var expectedId in expectedBorrowingIds)
        {
            Assert.Contains(response, b => b.Id == guids[expectedId]);
        }
    }
    [Fact]
    public async Task SearchBorrowingsAsync_InvalidPage_ThrowsArgumentException()
    {
        // Arrange
        int invalidPage = 0;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            borrowingService.SearchBorrowingsAsync(page: invalidPage, pageSize: 10));
        
        Assert.Equal($"Page ({invalidPage}) must be greater than zero.", ex.Message);
    }

    [Fact]
    public async Task SearchBorrowingsAsync_InvalidPageSize_ThrowsArgumentException()
    {
        // Arrange
        int invalidSize = 0;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            borrowingService.SearchBorrowingsAsync(page: 1, pageSize: invalidSize));
        
        Assert.Equal($"Size of a page ({invalidSize}) must be greater than zero.", ex.Message);
    }

    [Fact]
    public async Task SearchBorrowingsAsync_SearchTermTooShort_ThrowsArgumentException()
    {
        // Arrange
        string shortTerm = "ab";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            borrowingService.SearchBorrowingsAsync(searchTerm: shortTerm));
        
        Assert.Equal("The searching term need to have at least three letters.", ex.Message);
    }

    [Fact]
    public async Task SearchBorrowingsAsync_InvalidOlderThanFormat_ThrowsFormatException()
    {
        // Arrange
        string badDate = "31/02/2023";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.SearchBorrowingsAsync(olderThan: badDate));
        
        Assert.Contains("invalid olderThan date format", ex.Message);
    }

    [Fact]
    public async Task SearchBorrowingsAsync_InvalidNewerThanFormat_ThrowsFormatException()
    {
        // Arrange
        string badDate = "not-a-date";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.SearchBorrowingsAsync(newerThan: badDate));
        
        Assert.Contains("invalid newerThan date format", ex.Message);
    }

    [Fact]
    public async Task SearchBorrowingsAsync_InvalidCopyId_ThrowsFormatException()
    {
        // Arrange
        string invalidGuid = "not-a-guid";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() =>
            borrowingService.SearchBorrowingsAsync(copyId: invalidGuid));

        Assert.Equal("Invalid copy ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", ex.Message);
    }

    [Fact]
    public async Task SearchBorrowingsAsync_CopyIdNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        string guid = Guid.NewGuid().ToString(); // nieistniejący

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            borrowingService.SearchBorrowingsAsync(copyId: guid));

        Assert.Equal($"A copy with the specified ID ({guid}) was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task SearchBorrowingsAsync_ReaderIdNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        string guid = Guid.NewGuid().ToString(); // nieistniejący

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            borrowingService.SearchBorrowingsAsync(readerId: guid));

        Assert.Equal($"A reader with the specified ID ({guid}) was not found in the system.", ex.Message);
    }

    [Fact]
    public async Task SearchBorrowingsAsync_PageTooHigh_ThrowsInvalidOperationException()
    {
        // Arrange – ustawiamy page tak, by przekroczyć ilość rekordów
        int highPage = 10;
        int pageSize = 2;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            borrowingService.SearchBorrowingsAsync(page: highPage, pageSize: pageSize));

        Assert.Equal("Invalid page. Not so many borrowings.", ex.Message);
    }

    #endregion
}