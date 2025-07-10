# Blog API - Clean Architecture

This is a .NET 8 Web API project implementing Clean Architecture principles for a blog application.

## Architecture Overview

The project follows Clean Architecture with the following layers:

### Core Layer
- **Entities**: Domain models (BlogPost)
- **Interfaces**: Repository and service contracts
- **DTOs**: Data Transfer Objects for API communication

### Application Layer
- **Services**: Business logic implementation
- **Mappers**: Entity to DTO mapping logic

### Infrastructure Layer
- **Data**: Entity Framework DbContext and configurations
- **Repositories**: Data access implementations
- **Seeders**: Database seeding logic

### Presentation Layer
- **Controllers**: API endpoints
- **Middleware**: Request/response handling

## Project Structure

```
BlogApi/
├── Core/
│   ├── Entities/
│   │   └── BlogPost.cs
│   ├── Interfaces/
│   │   ├── IBlogPostRepository.cs
│   │   └── IBlogPostService.cs
│   └── DTOs/
│       └── BlogPostDto.cs
├── Application/
│   └── Services/
│       └── BlogPostService.cs
├── Infrastructure/
│   ├── Data/
│   │   ├── BlogDbContext.cs
│   │   └── DbSeeder.cs
│   └── Repositories/
│       └── BlogPostRepository.cs
├── Presentation/
│   └── Controllers/
│       └── BlogPostsController.cs
├── Program.cs
├── appsettings.json
└── BlogApi.csproj
```

## Features

- **Blog Posts**: CRUD operations for blog posts
- **Pagination**: Support for paginated results
- **Search**: Full-text search across title, content, and author
- **Categories**: Post categorization and filtering
- **Authors**: Author-based filtering
- **Featured Posts**: Support for featured post highlighting
- **SQL Server**: Entity Framework Core with SQL Server

## API Endpoints

- `GET /api/blogposts` - Get paginated posts
- `GET /api/blogposts/{id}` - Get post by ID
- `POST /api/blogposts` - Create new post
- `PUT /api/blogposts/{id}` - Update existing post
- `DELETE /api/blogposts/{id}` - Delete post
- `GET /api/blogposts/categories` - Get all categories
- `GET /api/blogposts/categories/{category}` - Get posts by category
- `GET /api/blogposts/authors` - Get all authors
- `GET /api/blogposts/authors/{author}` - Get posts by author

## Setup Instructions

1. **Database Connection**: Update the connection string in `appsettings.json`
2. **Dependencies**: Ensure all NuGet packages are restored
3. **Database**: The application will create and seed the database on startup
4. **Run**: Use `dotnet run` to start the application

## Dependencies

- .NET 8.0
- Entity Framework Core 8.0
- SQL Server
- Swagger/OpenAPI

## Benefits of Clean Architecture

- **Separation of Concerns**: Clear boundaries between layers
- **Testability**: Easy to unit test business logic
- **Maintainability**: Changes in one layer don't affect others
- **Scalability**: Easy to add new features and modify existing ones
- **Dependency Inversion**: High-level modules don't depend on low-level modules 