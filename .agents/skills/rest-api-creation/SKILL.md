---
name: rest-api-creation
description: Create new REST APIs following repository and entity patterns with Minimal API
license: MIT
compatibility: opencode
metadata:
  audience: developers
  workflow: api-development
---

## What I do

- Create REST API implementations following repository and entity patterns
- Generate Minimal API endpoints with proper HTTP methods and routing
- Implement entity models with proper data annotations
- Create repository interfaces and implementations (database-agnostic)
- Add request/response DTO models
- Configure Swagger/OpenAPI documentation
- Set up dependency injection for repositories
- Add health check endpoints
- Follow consistent naming and structure conventions

## When to use me

Use this skill when:
- Creating new REST API projects in the solution
- Adding new resource endpoints to existing APIs
- Implementing CRUD operations for new entities
- Following the repository's established API patterns
- Ensuring consistent API design across the solution
- Need database-agnostic repository implementations

## API Patterns in this Repository

### Project Structure
- **Models/**: Request/response DTOs and entity models
- **Repositories/**: Repository interfaces and implementations
- **Program.cs**: Minimal API configuration and endpoints

### Entity Model Structure
```csharp
namespace {ProjectName}.Models;

public class {EntityName}
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Additional properties as needed
}
```

### Request/Response DTOs
```csharp
// Create request (no Id, CreatedAt)
namespace {ProjectName}.Models;

public class Create{EntityName}Request
{
    public string Name { get; set; } = string.Empty;
    // Additional properties as needed
}

// Update request (no Id, CreatedAt)
namespace {ProjectName}.Models;

public class Update{EntityName}Request
{
    public string Name { get; set; } = string.Empty;
    // Additional properties as needed
}
```

### Repository Interface Pattern (Database-agnostic)
```csharp
using {ProjectName}.Models;

namespace {ProjectName}.Repositories;

public interface I{EntityName}Repository
{
    Task<IEnumerable<{EntityName}>> GetAllAsync();
    Task<{EntityName}?> GetByIdAsync(int id);
    Task<{EntityName}> CreateAsync(Create{EntityName}Request request);
    Task<{EntityName}?> UpdateAsync(int id, Update{EntityName}Request request);
    Task<bool> DeleteAsync(int id);
    // Optional: SearchAsync, GetBy{Property}Async, etc.
}
```

### Repository Implementation Pattern (In-memory example)
```csharp
using {ProjectName}.Models;

namespace {ProjectName}.Repositories;

public class {EntityName}Repository : I{EntityName}Repository
{
    private readonly List<{EntityName}> _items = new();
    private int _nextId = 1;

    public {EntityName}Repository()
    {
        // Optional: Seed initial data
        _items.Add(new {EntityName}
        {
            Id = _nextId++,
            Name = "Sample Item",
            CreatedAt = DateTime.UtcNow,
        });
    }

    public Task<IEnumerable<{EntityName}>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<{EntityName}>>(_items);
    }

    public Task<{EntityName}?> GetByIdAsync(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        return Task.FromResult(item);
    }

    public Task<{EntityName}> CreateAsync(Create{EntityName}Request request)
    {
        var entity = new {EntityName}
        {
            Id = _nextId++,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            // Map additional properties
        };

        _items.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<{EntityName}?> UpdateAsync(int id, Update{EntityName}Request request)
    {
        var entity = _items.FirstOrDefault(i => i.Id == id);
        if (entity == null)
            return Task.FromResult<{EntityName}?>(null);

        entity.Name = request.Name;
        // Update additional properties

        return Task.FromResult<{EntityName}?>(entity);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var entity = _items.FirstOrDefault(i => i.Id == id);
        if (entity == null)
            return Task.FromResult(false);

        _items.Remove(entity);
        return Task.FromResult(true);
    }
}
```

### Repository Implementation Pattern (Entity Framework example)
```csharp
using Microsoft.EntityFrameworkCore;
using {ProjectName}.Data;
using {ProjectName}.Models;

namespace {ProjectName}.Repositories;

public class {EntityName}Repository : I{EntityName}Repository
{
    private readonly {DbContextName} _context;

    public {EntityName}Repository({DbContextName} context)
    {
        _context = context;
    }

    public async Task<IEnumerable<{EntityName}>> GetAllAsync()
    {
        return await _context.{EntityName}s.ToListAsync();
    }

    public async Task<{EntityName}?> GetByIdAsync(int id)
    {
        return await _context.{EntityName}s.FindAsync(id);
    }

    public async Task<{EntityName}> CreateAsync(Create{EntityName}Request request)
    {
        var entity = new {EntityName}
        {
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            // Map additional properties
        };

        _context.{EntityName}s.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<{EntityName}?> UpdateAsync(int id, Update{EntityName}Request request)
    {
        var entity = await _context.{EntityName}s.FindAsync(id);
        if (entity == null)
            return null;

        entity.Name = request.Name;
        // Update additional properties

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.{EntityName}s.FindAsync(id);
        if (entity == null)
            return false;

        _context.{EntityName}s.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
```

### Minimal API Endpoint Pattern
```csharp
// In Program.cs
app.MapGet(
        "/api/{entity-name}s",
        async (I{EntityName}Repository repository) =>
        {
            var items = await repository.GetAllAsync();
            return Results.Ok(items);
        }
    )
    .WithName("GetAll{EntityName}s")
    .WithOpenApi();

app.MapGet(
        "/api/{entity-name}s/{id}",
        async (int id, I{EntityName}Repository repository) =>
        {
            var item = await repository.GetByIdAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        }
    )
    .WithName("Get{EntityName}ById")
    .WithOpenApi();

app.MapPost(
        "/api/{entity-name}s",
        async (Create{EntityName}Request request, I{EntityName}Repository repository) =>
        {
            var item = await repository.CreateAsync(request);
            return Results.Created($"/api/{entity-name}s/{item.Id}", item);
        }
    )
    .WithName("Create{EntityName}")
    .WithOpenApi();

app.MapPut(
        "/api/{entity-name}s/{id}",
        async (int id, Update{EntityName}Request request, I{EntityName}Repository repository) =>
        {
            var item = await repository.UpdateAsync(id, request);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        }
    )
    .WithName("Update{EntityName}")
    .WithOpenApi();

app.MapDelete(
        "/api/{entity-name}s/{id}",
        async (int id, I{EntityName}Repository repository) =>
        {
            var deleted = await repository.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        }
    )
    .WithName("Delete{EntityName}")
    .WithOpenApi();
```

### Program.cs Configuration Pattern
```csharp
using {ProjectName}.Models;
using {ProjectName}.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories (choose implementation based on needs)
builder.Services.AddScoped<I{EntityName}Repository, {EntityName}Repository>();

// For Entity Framework, add DbContext configuration:
// builder.Services.AddDbContext<{DbContextName}>(options => 
//     options.UseSqlite("Data Source={entity-name}_temp.db"));

// Add CORS for local development
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// API Endpoints (as shown above)

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();
```

### Naming Conventions
- **Entity classes**: PascalCase singular (e.g., `Product`, `Customer`)
- **API endpoints**: `/api/{entity-name}s` (lowercase, plural)
- **Repository interfaces**: `I{EntityName}Repository`
- **Repository implementations**: `{EntityName}Repository`
- **Request DTOs**: `Create{EntityName}Request`, `Update{EntityName}Request`

### Error Handling Patterns
- Return `Results.NotFound()` for non-existent resources
- Return `Results.BadRequest()` for invalid input
- Use `null` return values for "not found" scenarios in repositories
- Validate input parameters at API boundaries
- Use appropriate HTTP status codes (200, 201, 400, 404, 500)

### Async/Await Patterns
- All repository methods use `async`/`await`
- Suffix async methods with `Async`
- Use `Task` return types for async operations
- ConfigureAwait(false) not needed in .NET Core/ASP.NET Core

### API Documentation
- Enable Swagger/OpenAPI with `AddSwaggerGen()`
- Use `.WithName()` and `.WithOpenApi()` for endpoint documentation
- Include health check endpoint at `/health`

## How to use this skill

1. **Define the entity**: Determine the entity name and properties
2. **Create models**: Generate entity model and request/response DTOs
3. **Choose repository implementation**: Select in-memory or database-backed
4. **Implement repository**: Create interface and chosen implementation
5. **Configure API**: Set up Program.cs with services and middleware
6. **Add endpoints**: Implement Minimal API endpoints for CRUD operations
7. **Test integration**: Verify API works and follows patterns

## Repository Implementation Options

### In-memory Repository
- Simple, no database dependencies
- Good for prototyping and testing
- Data lost on application restart
- Use `List<T>` or `Dictionary<T>` for storage

### Entity Framework Repository
- Persistent storage with database
- Use existing DbContext or create new one
- Supports complex queries and relationships
- Requires database configuration

### Custom Data Source
- Connect to external APIs, files, or services
- Implement same repository interface
- Abstract data source details from API

## Example Output

When asked to create a new `Product` API, the skill will produce:

1. **Models/Product.cs**: Entity model with Id, Name, Price, CreatedAt
2. **Models/CreateProductRequest.cs**: DTO for creating products
3. **Models/UpdateProductRequest.cs**: DTO for updating products
4. **Repositories/IProductRepository.cs**: Interface with CRUD methods
5. **Repositories/ProductRepository.cs**: Implementation (in-memory or EF)
6. **Program.cs**: Minimal API with endpoints for `/api/products`

## Testing Integrity

When creating APIs, ensure:
- Follow existing patterns and conventions
- Use consistent naming and structure
- Implement proper error handling
- Add appropriate HTTP status codes
- Include Swagger documentation
- Set up CORS for development
- Add health check endpoint
- **Testing Integrity**: Never modify production code to make tests pass. If tests reveal bugs or issues, inform the user to decide the appropriate course of action.