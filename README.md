# Simple API for library staff

### Features:
- CRUD operations for all entities with diffrent accesability (diffrent endpoints available for three diffrent API keys)
- Searching books by: word, availbility, relaese date, author and category.
- Logging information in the console

### Still to do:
- Check if copy of book is available
-	Check how many copies of the book are available
-	Readers can have borrowed only specific number of books in the time	
- End Borrowing

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

![alt text](https://github.com/SzaroBury/SimpleLibraryAPI/blob/master/erd.png?raw=true)

### Technials:
- .NET Core 6.0
- Entity Framework Core
- MS SQL Server
- Unit tests with Moq
