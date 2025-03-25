# Simple API for library staff (work in progress)

### Target Features:
- CRUD operations for all entities with diffrent accesability.
- Diffrent endpoints available for three diffrent API keys.
- Searching books by: word, availbility, relaese date, author and category.
- Logging information in the console

### Installation:
- dotnet run --project .\src\APIServer
- To open full API documentation: 'https://localhost:7299/swagger'

### Technicalities:
- .NET Core 8.0
- Entity Framework Core
- MS SQL Server
- Swagger 7.1
- Unit tests with Xunit and Moq

### Projects in the solution:
- API - controllers
- Application - services, interfaces
- Domain - model, DTOs, enumerations
- Infrastructure - connection to the database
- Application.Tests - unit tests of the Core project

### Entities in the system:
- Author
- Book
- Borrowing
- Category
- Copy (of a book)
- Reader

![alt text](https://github.com/SzaroBury/SimpleLibraryAPI/blob/master/erd.png?raw=true)

### To do:
- Check if copy of book is available
- Check how many copies of the book are available
- Readers can have borrowed only specific number of books in the time	
- End Borrowing
