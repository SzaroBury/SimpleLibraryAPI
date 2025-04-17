# Simple API for library staff

### Features:
- CRUD operations for all entities
- Diffrent endpoints available for three diffrent API keys.
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
- API - Controllers, Attributes, DI
- Application - Services
- Domain - Model, DTOs, Enumerations
- Infrastructure - Repository, UOW, DBContext
- Application.Tests - unit tests of the Application project

### Entities in the system:
- Author
- Book
- Borrowing
- Category
- Copy (of a book)
- Reader

![alt text](https://github.com/SzaroBury/SimpleLibraryAPI/blob/HEAD/SimpleLibraryLogicalERD.png?raw=true)

### Todo:
- JWT Authentication
- Integragion tests