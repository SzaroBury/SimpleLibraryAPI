using ApiServer.Controllers;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace ApiServer.Tests
{
    public class TestBookController
    {
        Mock<ILogger<BookController>> mockBookLogger = new();
        Mock<IBookRepository> mockBookRepository = new();
        Mock<ICopyRepository> mockCopyRepository = new();
        Mock<IBorrowingRepository> mockBorrowingRepository = new();
        ControllerContext mockControllerContext = new();

        public TestBookController()
        {
            Author a1 = new Author { Id = 1, FirstName = @"N/A", LastName = @"N/A", BornDate = null };
            Author a2 = new Author { Id = 2, FirstName = @"Adam", LastName = @"Mickiewicz", BornDate = new DateTime(1798, 12, 24) };
            Author a3 = new Author { Id = 3, FirstName = @"Jan", LastName = @"Kowalski", BornDate = new DateTime(1968, 12, 09) };

            Category c1 = new Category { Id = 1, Name = "Novel" };
            Category c2 = new Category { Id = 2, Name = "Other" };

            Reader r1 = new Reader { Id = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@mail.com", Phone = "+48 789 456 123" };

            Book b1 = new Book { Id = 1, Title = "Some old book", Author = a1, AuthorId = 1, Category = c1, CategoryId = 1, Language = Entities.Enumerations.Language.English, ReleaseDate = new DateTime(1900, 1, 1) };
            Copy b1_c1 = new Copy { Id = 1, Book = b1, BookId = 1 };
            Borrowing bor1 = new Borrowing { Id = 1, Copy = b1_c1, CopyId = 1, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5) };
            Copy b1_c2 = new Copy { Id = 2, Book = b1, BookId = 1 };
            Borrowing bor2 = new Borrowing { Id = 2, Copy = b1_c2, CopyId = 2, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5) };

            Book b2 = new Book { Id = 2, Title = "Some old German book", Author = a1, AuthorId = 1, Category = c1, CategoryId = 1, Language = Entities.Enumerations.Language.German, ReleaseDate = new DateTime(1800, 1, 1) };
            Copy b2_c3 = new Copy { Id = 3, Book = b2, BookId = 2 };
            Borrowing bor3 = new Borrowing { Id = 3, Copy = b2_c3, CopyId = 3, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5) };
            Copy b2_c4 = new Copy { Id = 4, Book = b2, BookId = 2 };

            Book b3 = new Book { Id = 3, Title = "Some new French book", Author = a1, AuthorId = 1, Category = c2, CategoryId = 2, Language = Entities.Enumerations.Language.French, ReleaseDate = new DateTime(2010, 5, 7) };
            Copy b3_c5 = new Copy { Id = 5, Book = b3, BookId = 3 };
            Borrowing bor4 = new Borrowing { Id = 4, Copy = b3_c5, CopyId = 5, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5), ActualReturnDate = DateTime.Now.AddDays(-2) };

            Book b4 = new Book { Id = 4, Title = "Dziady czêœæ II", Author = a2, AuthorId = 2, Category = c2, CategoryId = 2, Language = Entities.Enumerations.Language.Polish, ReleaseDate = new DateTime(1823, 1, 1) };
            Copy b4_c6 = new Copy { Id = 6, Book = b4, BookId = 4 };
            Borrowing bor5 = new Borrowing { Id = 5, Copy = b4_c6, CopyId = 6, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-15), ActualReturnDate = DateTime.Now.AddDays(-10) };
            Borrowing bor6 = new Borrowing { Id = 6, Copy = b4_c6, CopyId = 6, Reader = r1, ReaderId = 1, StartedDate = DateTime.Now.AddDays(-5) };

            Book b5 = new Book { Id = 5, Title = "Dziady czêœæ III", Author = a2, AuthorId = 2, Category = c2, CategoryId = 2, Language = Entities.Enumerations.Language.Polish, ReleaseDate = new DateTime(1832, 1, 1) };

            Book b6 = new Book { Id = 6, Title = "Another book", Author = a3, AuthorId = 3, Category = c1, CategoryId = 1, Language = Entities.Enumerations.Language.English, ReleaseDate = new DateTime(1985, 1, 9), Tags = "Some" };

            //Arrange 
            mockBookRepository.Setup(repo => repo.GetBooks()).Returns(new List<Book>() 
            {
                b1, b2, b3, b4, b5, b6
            });
            mockCopyRepository.Setup(repo => repo.GetCopies()).Returns(new List<Copy>()
            {
                b1_c1, b1_c2, b2_c3, b2_c4, b3_c5, b4_c6
            });
            mockBorrowingRepository.Setup(repo => repo.GetBorrowings()).Returns(new List<Borrowing>()
            {
                bor1, bor2, bor3, bor4, bor5, bor6
            });

            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.QueryString).Returns(new QueryString(""));
            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            var mockRouteData = new Microsoft.AspNetCore.Routing.RouteData();
            var mockActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor();
            var mockActionContext = new ActionContext(context.Object, mockRouteData, mockActionDescriptor);
            mockControllerContext = new ControllerContext(mockActionContext);
        }

        #region GetAll
        [Fact]
        public void GetAll_NoFilters_ReturnsAllBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll() as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(6, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 1);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 2);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 3);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 4);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 5);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 6);
        }

        [Fact]
        public void GetAll_SearchSome_ReturnsCertainBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll(search: "Some") as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(4, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 1);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 2);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 3);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 6);
        }

        [Fact]
        public void GetAll_SearchMickiewicz_ReturnsCertainBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll(search: "Mickiewicz") as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(2, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 4);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 5);
        }

        [Fact]
        public void GetAll_SearchNovel_ReturnsCertainBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll(search: "Novel") as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(3, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 1);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 2);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 6);
        }

        [Fact]
        public void GetAll_OnlyAvailable_ReturnsCertainBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object, 
                                                                mockBookRepository.Object, 
                                                                mockCopyRepository.Object, 
                                                                mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll(isAvailable: 1) as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(2, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 2);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 3);
        }

        [Fact]
        public void GetAll_OnlyNotAvailable_ReturnsCertainBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll(isAvailable: 0) as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(4, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 1);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 4);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 5);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 6);
        }

        [Fact]
        public void GetAll_OlderThan_ReturnsCertainBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll(olderThan: "1900-01-01") as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(4, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 1);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 2);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 4);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 5);
        }

        [Fact]
        public void GetAll_NewerThan_ReturnsCertainBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object,
                                                               mockBookRepository.Object,
                                                               mockCopyRepository.Object,
                                                               mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll(newerThan: "1900-01-01") as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(3, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 1);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 3);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 6);
        }

        [Fact]
        public void GetAll_ByAuthor_ReturnsCertainBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll(author: 2) as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(2, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 4);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 5);
        }

        [Fact]
        public void GetAll_InCategory_ReturnsCertainBooks()
        {
            //Arrange
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = mockControllerContext;

            //Act
            var response = bookController.GetAll(category: 1) as JsonResult;
            var responseList = response!.Value as List<Book>;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<List<Book>>(response!.Value);
            Assert.Equal(3, responseList!.Count);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 1);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 2);
            Assert.Contains(response.Value as IEnumerable<Book>, b => b.Id == 6);
        }

        [Fact]
        public void GetAll_OnFailure_Returns500()
        {
            //Arrange 
            mockBookRepository.Setup(service => service.GetBooks()).Throws<InvalidOperationException>();
            var context = new Mock<HttpContext>();
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            context.SetupGet(x => x.Connection.RemoteIpAddress).Returns(ip);
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = new ControllerContext(new ActionContext(context.Object,
                                                                                       new Microsoft.AspNetCore.Routing.RouteData(),
                                                                                       new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()));

            //Act
            var response = bookController.GetAll() as ObjectResult;

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response!.Value);
            Assert.Equal(500, response!.StatusCode);
            Assert.StartsWith("Error: ", response!.Value!.ToString());
        }
        #endregion

        #region Get
        [Fact]
        public void Get_OnSuccess_ReturnsBookInfo()
        {
            //Arrange 
            mockBookRepository.Setup(service => service.GetBook(It.IsAny<int>())).Returns(new Book() { Id = 0 });
            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);

            //Act
            var response = bookController.Get(1) as JsonResult;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<Book>(response!.Value);
            Assert.Equal(200, response!.StatusCode);
            //Assert.Equals(response.Value );
        }

        [Fact]
        public void Get_IdNotFound_Returns500()
        {
            //Arrange 
            mockBookRepository.Setup(service => service.GetBook(It.IsAny<int>())).Throws<KeyNotFoundException>();

            var context = new Mock<HttpContext>();
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            context.SetupGet(x => x.Connection.RemoteIpAddress).Returns(ip);

            BookController bookController = new BookController(mockBookLogger.Object, mockBookRepository.Object, mockCopyRepository.Object, mockBorrowingRepository.Object);
            bookController.ControllerContext = new ControllerContext(new ActionContext(context.Object,
                                                                                       new Microsoft.AspNetCore.Routing.RouteData(),
                                                                                       new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()));

            //Act
            var response = bookController.Get(1) as ObjectResult;

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response!.Value);
            Assert.StartsWith("Error:", response!.Value!.ToString() );
            Assert.Equal(500, response!.StatusCode);
        }
        #endregion
    }
}