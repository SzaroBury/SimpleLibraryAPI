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

- **🔐 Security**
  - JSON Web Token (JWT) Authentication with Access and Refresh tokens stored 
  - Role-based access control implemented through claims in the JWT.
  - User roles (e.g., Admin, Librarian) are stored in JWT claims.
  - Access to API endpoints is restricted based on user roles.
  - Tokens are securely stored in HttpOnly cookies, which are inaccessible to JavaScript, protecting against XSS attacks.
  - Refresh tokens are stored separately to ensure additional security.
  - Token expiration and refresh mechanisms are implemented to ensure session security.
  - HTTPS is required to prevent token interception during transmission.

- **📊 API Documentation and Testing**
  - Interactive Swagger UI for exploring and testing endpoints (`/swagger`)  
  - Unit tests for core application logic using xUnit and Moq  

- **🛠️ Technical Highlights**
  - RESTful API design with consistent endpoint structure  
  - Console logging of key operations for debugging and monitoring  
  - Clean layered architecture using Repository, Unit of Work, and DTO patterns  

## 📅 Planned Features
- Validation Refactoring in the Application layer.
- Integrating ASP.NET Identity or OAuth2
- Caching for Performance – plan to implement Redis to cache frequently queried entities and improve response times.
- Integration Tests to ensure the robustness of the API.
- Expand logging with Serilog for structured logging, including integration with files, databases, or cloud providers.
- Add ASP.NET Core Health Checks to monitor service availability and integrate with Prometheus for real-time metrics.

## 🛠️ Technologies
- .NET 8.0
- Entity Framework Core
- MS SQL Server 2019
- Swagger
- Xunit + Moq (unit testing)
- FluentValidations
- Docker

## 🏗️ Project Structure

- **API**               – Controllers, Dependency Injection, Requests, Mappers, Validators, Attributes
- **Application**       – Services, Interfaces and Commands
- **Domain**            – Entities, Enums and Interfaces
- **Infrastructure**    – Repositories, Unit of Work, DbContext, External services and Migrations
- **Application.Tests** – Unit tests for Application layer

## 📘 Entities

- Author
- Book
- Borrowing
- Category
- Copy
- Reader

![ERD](https://github.com/SzaroBury/SimpleLibraryAPI/blob/master/SimpleLibraryLogicalERD.png?raw=true)

## ✅ Testing

- Unit tests using Xunit and Moq
- [ ] Integration tests (planned)

## 📦 Installation

### Prerequisites
- **Git**: Ensure Git is installed on your system. [Download Git](https://git-scm.com/downloads)
- **Docker**: Install Docker and Docker Compose. [Get Docker](https://www.docker.com/get-started)
- **.NET 8.0 SDK (Optional)**: Required if you want to run the application locally without Docker. [Download .NET SDK](https://dotnet.microsoft.com/download)

### Steps
1. **Clone the Repository**
   ```bash
   git clone https://github.com/SzaroBury/SimpleLibraryAPI.git
   cd SimpleLibraryAPI
   ```

2. **Run with Docker Compose**
   - Ensure Docker is running on your system.
    ```bash
    docker info
    ```
   - Start the application using `docker-compose`:
     ```bash
     docker-compose up --build
     ```

3. **Access the API**
   - The API will be available at: [https://localhost:5000/swagger](https://localhost:5000/swagger)

### Notes
- The `docker-compose.yml` file includes configurations for the API and its dependencies (e.g., database).
- Ensure ports used in the `docker-compose.yml` file are not blocked by other applications.

## 📐 Design Patterns Used
- Repository Pattern – abstracting data access logic
- Unit of Work – managing transactions across repositories
- Dependency Injection (built-in DI container) – for services and repositories
- DTOs (Data Transfer Objects) – separating domain models from API contracts
- Layered Architecture – separation of concerns into API / Application / Domain / Infrastructure