# Simple API for library staff

### Features:
- CRUD operations for all entities with diffrent accesability (diffrent endpoints available for three diffrent API keys)
- Searching books by: word, availbility, relaese date, author and category.
- Logging information in the console

### Projects in the solution:
- ApiServer - controllers and business logic
- Entities - model, interfaces, enumerations
- RepositoryEF - connection to the database
- ApiServer.Tests - unit tests

### Entities in the system:
- Author
- Book
- Borrowing
- Category
- Copy (of a book)
- Reader

### Technials:
- .NET Core 6.0
- Entity Framework
- MS SQL Server
- Unit tests with Moq
