# 📚 Simple Library API

A simple REST API for managing a library system. Designed for library staff use.

## 🚀 Features

- **📚 Library Resource Management**
  - Full CRUD operations for books, authors, categories, and copies  
  - Support for multiple copies of a single book (inventory tracking)  
  - Book categorization and author assignment  

- **🙋‍♂️ Reader and Borrowing System**
  - Create and manage reader profiles  
  - Record borrowings and returns of book copies  
  - Built-in validation for copy availability  

- **🔐 Access Control**
  - Three distinct API access levels based on provided API keys  
  - Each key grants access to a specific set of endpoints and actions  

- **📊 API Documentation and Testing**
  - Interactive Swagger UI for exploring and testing endpoints (`/swagger`)  
  - Unit tests for core application logic using xUnit and Moq  

- **🛠️ Technical Highlights**
  - RESTful API design with consistent endpoint structure  
  - Console logging of key operations for debugging and monitoring  
  - Clean layered architecture using Repository, Unit of Work, and DTO patterns  

## 📅 Planned Features
- JWT Authentication for secure access.
- Integrating ASP.NET Identity or OAuth2
- Validation Refactoring - plan to implement separate classes for custom validation rules to improve code organization and maintainability.
- Caching for Performance – plan to implement Redis to cache frequently queried entities and improve response times.
- Integration Tests to ensure the robustness of the API.
- Expand logging with Serilog for structured logging, including integration with files, databases, or cloud providers.
- Add ASP.NET Core Health Checks to monitor service availability and integrate with Prometheus for real-time metrics.

## 🛠️ Technologies

- .NET 8.0
- Entity Framework Core
- MS SQL Server
- Swagger 7.1
- Xunit + Moq (unit testing)

## 🏗️ Project Structure

- **API**               – Controllers, attributes, dependency injection
- **Application**       – Business logic and services
- **Domain**            – Entities, DTOs, enums
- **Infrastructure**    – Repositories, Unit of Work, DbContext
- **Application.Tests** – Unit tests for Application layer

## 📘 Entities

- Author
- Book
- Borrowing
- Category
- Copy
- Reader

![ERD](https://github.com/SzaroBury/SimpleLibraryAPI/blob/master/SimpleLibraryLogicalERD.png?raw=true)

## 🔐 Security

- Lightweight role-based access system using scoped API keys
- [ ] JWT Authentication (planned)

## ✅ Testing

- Unit tests using Xunit and Moq
- [ ] Integration tests (planned)

## 📦 Installation

```bash
dotnet run --project .\src\API
API documentation available at: https://localhost:7299/swagger
```

## 📐 Design Patterns Used
- Repository Pattern – abstracting data access logic
- Unit of Work – managing transactions across repositories
- Dependency Injection (built-in DI container) – for services and repositories
- DTOs (Data Transfer Objects) – separating domain models from API contracts
- Layered Architecture – separation of concerns into API / Application / Domain / Infrastructure