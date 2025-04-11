using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Application.Services;
using SimpleLibrary.Domain.Repositories;

namespace SimpleLibrary.Tests.Application.Services;

public class TestReaderService
{
    private readonly Dictionary<string, Guid> guids = DataInitializer.InitializeGuids();
    private readonly IUnitOfWork unitOfWork;
    private readonly Mock<IRepository<Reader>> mockReaderRepository;
    private readonly ReaderService readerService;

    public TestReaderService()
    {
        mockReaderRepository = DataInitializer.InitializeReaders(guids);
        unitOfWork = DataInitializer.InitializeUnitOfWorkAsync(guids, mockReaderRepository: mockReaderRepository).GetAwaiter().GetResult().Object;
        readerService = new ReaderService(unitOfWork);
    }
    #region GetAllReadersAsync
    [Fact]
    public async Task GetAllReadersAsync_ReturnsAllReaders()
    {
        var response = await readerService.GetAllReadersAsync();

        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Reader>>(response);
        Assert.Equal(4, response.Count());
    }
    #endregion
    #region GetReaderByIdAsync
    [Fact]
    public async Task GetReaderByIdAsync_ExistingId_ReturnsReader()
    {
        // Act
        var result = await readerService.GetReaderByIdAsync(guids["r1"].ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jan", result.FirstName);
        Assert.Equal("Kowalski", result.LastName);
        Assert.Equal("012", result.CardNumber);
        Assert.Equal("jan.kowalski@mail.com", result.Email);
        Assert.Equal("+48789456123", result.Phone);
    }

    [Fact]
    public async Task GetReaderByIdAsync_InvalidFormatOfId_ThrowsFormatException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            readerService.GetReaderByIdAsync("test")
        );
        Assert.Equal($"Invalid reader ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }

    [Fact]
    public async Task GetReaderByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        //Arange
        Guid readerGuid = Guid.NewGuid();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            readerService.GetReaderByIdAsync(readerGuid)
        );
        Assert.Equal($"A reader with the specified ID ({readerGuid}) was not found in the system.", exception.Message);
    }
    #endregion
    #region CreateReaderAsync 
    [Theory]
    [InlineData("William", "Shakespeare", "william.shakespeare@mail.com", "+48123123123", "William", "Shakespeare", "william.shakespeare@mail.com", "+48123123123")]
    [InlineData("William", "Shakespeare", null,                           "+48123123123", "William", "Shakespeare", null,                           "+48123123123")]
    [InlineData("William", "Shakespeare", "william.shakespeare@mail.com", null,           "William", "Shakespeare", "william.shakespeare@mail.com", null          )]
    public async Task CreateReaderAsync_ValidInput_ReturnsExpectedResult(
        string firstName, string lastName, string? email, string? phone,
        string expectedFirstName, string expectedLastName, string? expectedEmail, string? expectedPhone)
    {
        // Arrange
        var newReader = new ReaderPostDTO(firstName, lastName, email, phone);

        // Act
        var result = await readerService.CreateReaderAsync(newReader);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedFirstName, result.FirstName);
        Assert.Equal(expectedLastName, result.LastName);
        Assert.Equal(expectedEmail, result.Email);
        Assert.Equal(expectedPhone, result.Phone);
    }

    [Fact]
    public async Task CreateReaderAsync_EmptyFirstName_ThrowsArgumentException()
    {
        var newReader = new ReaderPostDTO("", "Shakespeare", "william.shakespeare@mail.com", "+48123123123");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => readerService.CreateReaderAsync(newReader));

        Assert.Equal("A reader's first name must not be left blank.", exception.Message);
    }

    [Fact]
    public async Task CreateReaderAsync_EmptyLastName_ThrowsArgumentException()
    {
        var newReader = new ReaderPostDTO("William", "", "william.shakespeare@mail.com", "+48123123123");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => readerService.CreateReaderAsync(newReader));

        Assert.Equal("A reader's last name must not be left blank.", exception.Message);
    }

    [Fact]
    public async Task CreateReaderAsync_InvalidEmailFormat_ThrowsFormatException()
    {
        var newReader = new ReaderPostDTO("William", "Shakespeare", "william.shakespeare.mail.com", "+48123123123");

        var exception = await Assert.ThrowsAsync<FormatException>(() => readerService.CreateReaderAsync(newReader));

        Assert.Equal("Invalid email format.", exception.Message);
    }

    [Fact]
    public async Task CreateReaderAsync_InvalidPhoneFormat_ThrowsFormatException()
    {
        var newReader = new ReaderPostDTO("William", "Shakespeare", null, "invalid-phone-number");

        var exception = await Assert.ThrowsAsync<FormatException>(() => readerService.CreateReaderAsync(newReader));

        Assert.Equal("Invalid phone number format. Please use E.164 format. Example: +11222333444", exception.Message);
    }
    #endregion
    #region UpdateReaderAsync 
    [Theory]
    [InlineData("Updated Firstname", "Updated Lastname", "updated.email@mail.com", "+48123123123", true, "Updated Firstname", "Updated Lastname", "updated.email@mail.com", "+48123123123", true)]
    [InlineData("Updated FirstName", null,               null,                     null,           null, "Updated FirstName", "Kowalski",         "jan.kowalski@mail.com",  "+48789456123", false)]
    [InlineData(null,                "Updated Lastname", null,                     null,           null, "Jan",               "Updated Lastname", "jan.kowalski@mail.com",  "+48789456123", false)]
    [InlineData(null,                null,               "updated.email@mail.com", null,           null, "Jan",               "Kowalski",         "updated.email@mail.com", "+48789456123", false)]
    [InlineData(null,                null,               null,                     "+48123123123", null, "Jan",               "Kowalski",         "jan.kowalski@mail.com",  "+48123123123", false)]
    [InlineData(null,                null,               null,                     null,           true, "Jan",               "Kowalski",         "jan.kowalski@mail.com",  "+48789456123", true)]
    [InlineData(null,                null,               null,                     null,           null, "Jan",               "Kowalski",         "jan.kowalski@mail.com",  "+48789456123", false)]
    public async Task UpdateReaderAsync_ValidInput_UpdatesReaderCorrectly(
        string? firstName, string? lastName, string? email, string? phone, bool? isBanned,
        string expectedFirstName, string expectedLastName, string expectedEmail, string expectedPhone, bool expectedIsBanned)
    {
        // Arrange
        var readerId = guids["r1"].ToString();
        var updatedReader = new ReaderPutDTO(readerId, firstName, lastName, email, phone, isBanned);

        // Act
        var result = await readerService.UpdateReaderAsync(updatedReader);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(guids["r1"],       result.Id);
        Assert.Equal(expectedFirstName, result.FirstName);
        Assert.Equal(expectedLastName,  result.LastName);
        Assert.Equal(expectedEmail,     result.Email);
        Assert.Equal(expectedPhone,     result.Phone);
        Assert.Equal(expectedIsBanned,  result.IsBanned);
    }

    [Fact]
    public async Task UpdateReaderAsync_InvalidIdFormat_ThrowsFormatException()
    {
        var invalidReaderId = "invalid-id-format";
        var updatedReader = new ReaderPutDTO(invalidReaderId, FirstName: "Updated Name");

        var exception = await Assert.ThrowsAsync<FormatException>(() => readerService.UpdateReaderAsync(updatedReader));

        Assert.Equal("Invalid reader ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }

    [Fact]
    public async Task UpdateReaderAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        var nonExistingReaderId = Guid.NewGuid().ToString(); // Losowy, nieistniejący ID
        var updatedReader = new ReaderPutDTO(nonExistingReaderId, FirstName: "Updated Name");

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => readerService.UpdateReaderAsync(updatedReader));

        Assert.Equal($"A reader with the specified ID ({nonExistingReaderId}) was not found in the system.", exception.Message);
    }

    [Fact]
    public async Task UpdateReaderAsync_EmptyFirstName_ThrowsArgumentException()
    {
        var readerId = guids["r2"].ToString();
        var updatedReader = new ReaderPutDTO(readerId, FirstName: " ");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => readerService.UpdateReaderAsync(updatedReader));

        Assert.Equal("A reader's first name must not be left blank.", exception.Message);
    }

    [Fact]
    public async Task UpdateReaderAsync_EmptyLastName_ThrowsArgumentException()
    {
        var readerId = guids["r2"].ToString();
        var updatedReader = new ReaderPutDTO(readerId, LastName: " ");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => readerService.UpdateReaderAsync(updatedReader));

        Assert.Equal("A reader's last name must not be left blank.", exception.Message);
    }

    [Fact]
    public async Task UpdateReaderAsync_InvalidEmailFormat_ThrowsFormatException()
    {
        var readerId = guids["r2"].ToString();
        var updatedReader = new ReaderPutDTO(readerId, Email: "invalid-email");

        var exception = await Assert.ThrowsAsync<FormatException>(() => readerService.UpdateReaderAsync(updatedReader));

        Assert.Equal("Invalid email format.", exception.Message);
    }

    [Fact]
    public async Task UpdateReaderAsync_InvalidPhone_ThrowsFormatException()
    {
        var readerId = guids["r2"].ToString();
        var updatedReader = new ReaderPutDTO(readerId, Phone: "invalid-phone");

        var exception = await Assert.ThrowsAsync<FormatException>(() => readerService.UpdateReaderAsync(updatedReader));

        Assert.Equal("Invalid phone number format. Please use E.164 format. Example: +11222333444", exception.Message);
    }    
    #endregion
    #region DeleteReaderAsync
    [Fact]
    public async Task DeleteReaderAsync_ValidReaderId_DeletesReaderSuccessfully()
    {
        var readersGuid = guids["r2"];

        await readerService.DeleteReaderAsync(readersGuid.ToString());

        mockReaderRepository.Verify(repo => repo.DeleteAsync(readersGuid), Times.Once);
    }
    
    [Fact]
    public async Task DeleteReaderAsync_NonExistentReader_ThrowsKeyNotFoundException()
    {
        var readersGuid = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            readerService.DeleteReaderAsync(readersGuid.ToString())
        );

        Assert.Equal($"A reader with the specified ID ({readersGuid}) was not found in the system.", exception.Message);
    }

    [Fact]
    public async Task DeleteReaderAsync_InvalidIdFormat_ThrowsFormatException()
    {
        var readersGuid = "invalid-id-format";

        var exception = await Assert.ThrowsAsync<FormatException>(() => 
            readerService.DeleteReaderAsync(readersGuid.ToString())
        );

        Assert.Equal($"Invalid reader ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }

    [Fact]
    public async Task DeleteReaderAsync_ReaderWithThreeBorrowedBooks_ThrowsInvalidOperationException()
    {
        var readersGuid = guids["r1"].ToString();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            readerService.DeleteReaderAsync(readersGuid)
        );

        Assert.Equal($"The reader cannot be deleted, because she/he has still borrowed 2 books: \"Some old book\" #1, \"Some old German book\" #1.", exception.Message);
    }

    [Fact]
    public async Task DeleteReaderAsync_ReaderWithOneBorrowedBook_ThrowsInvalidOperationException()
    {
        var readersGuid = guids["r4"].ToString();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            readerService.DeleteReaderAsync(readersGuid)
        );

        Assert.Equal($"The reader cannot be deleted, because she/he has still borrowed 1 book: \"Dziady część II\" #1.", exception.Message);
    }
    #endregion
    #region SearchReadersAsync
    [Theory]
    [InlineData("", new[] { "Kowalski", "Stones", "Konarek", "Nowak" })]
    [InlineData("Jan", new[] { "Kowalski" })] 
    [InlineData("Stone", new[] { "Stones" })] 
    [InlineData("example", new[] { "Nowak" })] 
    [InlineData("012", new string[] { "Kowalski" })]
    [InlineData("879456", new string[] { "Konarek" })]
    [InlineData("unknown_term", new string[] { })]
    public async Task SearchReadersAsync_SearchTerm_ReturnsExpectedResults(string searchTerm, string[] expectedLastNames)
    {
        // Act
        var response = await readerService.SearchReadersAsync(searchTerm: searchTerm);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Reader>>(response);
        Assert.Equal(expectedLastNames.Length, response.Count());
        foreach (var expectedLastName in expectedLastNames)
        {
            Assert.Contains(response, a => a.LastName == expectedLastName);
        }
    }

    [Fact]
    public async Task SearchReadersAsync_FilteringByCopy_ReturnsExpectedResults()
    {
        var copyId = guids["b1_c1"];
        // Act
        var response = await readerService.SearchReadersAsync(copyId: copyId.ToString());

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Reader>>(response);
        Assert.Single(response);
        Assert.Equal(guids["r1"], response.First().Id);
    }

    [Fact]
    public async Task SearchReadersAsync_FilteringByIsBannedFlag_ReturnsExpectedResults()
    {
        // Act
        var response = await readerService.SearchReadersAsync(isBanned: true);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Reader>>(response);
        Assert.Single(response);
        Assert.Equal(guids["r4"], response.First().Id);
    }

    [Theory]
    [InlineData(1, 1, new[] { "012" })]
    [InlineData(1, 2, new[] { "012", "123" })]
    [InlineData(2, 1, new[] { "123" })]
    [InlineData(2, 2, new[] { "234", "345" })]
    [InlineData(2, 3, new string[] { "345" })]
    public async Task SearchReadersAsync_Pagination_ReturnsExpectedResults(int page, int pageSize, string[] expectedCardNumbers)
    {
        // Act
        var response = await readerService.SearchReadersAsync(page: page, pageSize: pageSize);

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<Reader>>(response);
        Assert.Equal(expectedCardNumbers.Length, response.Count());
        foreach (var expectedCardNumber in expectedCardNumbers)
        {
            Assert.Contains(response, a => a.CardNumber == expectedCardNumber);
        }
    }

    [Fact]
    public async Task SearchReadersAsync_SearchingTwoLetters_ThrowsArgumentException()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await readerService.SearchReadersAsync(searchTerm: "ts"));

        Assert.Equal($"The searching term need to have at least three letters.", exception.Message);
    }

    [Fact]
    public async Task SearchReadersAsync_PageLessThanOne_ThrowsArgumentException()
    {
        var page = 0;

        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await readerService.SearchReadersAsync(page: page));

        Assert.Equal($"Page ({page}) must be greater than zero.", exception.Message);
    }

    [Fact]
    public async Task SearchReadersAsync_PageSizeLessThanOne_ThrowsArgumentException()
    {
        var pageSize = 0;

        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await readerService.SearchReadersAsync(pageSize: pageSize));

        Assert.Equal($"Size of a page ({pageSize}) must be greater than zero.", exception.Message);
    }

    [Fact]
    public async Task SearchReadersAsync_InvalidCopyIdFormat_ThrowsFormatException()
    {
        var exception = await Assert.ThrowsAsync<FormatException>(async () =>
            await readerService.SearchReadersAsync(copyId: "invalid-id")
        );

        Assert.Equal("Invalid copy ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.", exception.Message);
    }

    [Fact]
    public async Task SearchReadersAsync_NonExistantCopyId_ThrowsKeyNotFoundException()
    {
        var copyId = Guid.NewGuid().ToString();
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await readerService.SearchReadersAsync(copyId: copyId)
        );

        Assert.Equal($"A copy with the specified ID ({copyId}) was not found in the system.", exception.Message);
    }

    [Fact]
    public async Task SearchReadersAsync_PageOutOfRange_ThrowsInvalidOperationException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await readerService.SearchReadersAsync(page: 100, pageSize: 1));
    }
    #endregion
}