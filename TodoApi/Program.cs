using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework with SQLite file-based database
// Using file instead of :memory: to avoid connection issues
var dbPath = "todo_temp.db";
builder.Services.AddDbContext<TodoContext>(
    options => options.UseSqlite($"Data Source={dbPath}"),
    ServiceLifetime.Scoped
);

// Register repositories
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

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

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoContext>();

    // Ensure database is created and tables exist
    Console.WriteLine($"Creating database at: {dbPath}");
    var created = dbContext.Database.EnsureCreated();
    Console.WriteLine($"Database created: {created}");

    // Seed initial data if tables were created
    if (created)
    {
        Console.WriteLine("Database was created, seeding initial data...");
    }
    else
    {
        Console.WriteLine("Database already exists.");
    }
}

// API Endpoints
app.MapGet(
        "/api/todos",
        async (ITodoRepository repository, CancellationToken cancellationToken) =>
        {
            var todos = await repository.GetAllAsync(cancellationToken);
            return Results.Ok(todos);
        }
    )
    .WithName("GetAllTodos")
    .WithOpenApi();

app.MapGet(
        "/api/todos/{id}",
        async (int id, ITodoRepository repository, CancellationToken cancellationToken) =>
        {
            var todo = await repository.GetByIdAsync(id, cancellationToken);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        }
    )
    .WithName("GetTodoById")
    .WithOpenApi();

app.MapPost(
        "/api/todos",
        async (
            CreateTodoRequest request,
            ITodoRepository repository,
            CancellationToken cancellationToken
        ) =>
        {
            var todo = await repository.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/todos/{todo.Id}", todo);
        }
    )
    .WithName("CreateTodo")
    .WithOpenApi();

app.MapPut(
        "/api/todos/{id}",
        async (
            int id,
            UpdateTodoRequest request,
            ITodoRepository repository,
            CancellationToken cancellationToken
        ) =>
        {
            var todo = await repository.UpdateAsync(id, request, cancellationToken);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        }
    )
    .WithName("UpdateTodo")
    .WithOpenApi();

app.MapDelete(
        "/api/todos/{id}",
        async (int id, ITodoRepository repository, CancellationToken cancellationToken) =>
        {
            var deleted = await repository.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }
    )
    .WithName("DeleteTodo")
    .WithOpenApi();

app.MapGet(
        "/api/todos/search",
        async (string q, ITodoRepository repository, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(q))
                return Results.BadRequest("Search query is required");

            var todos = await repository.SearchAsync(q, cancellationToken);
            return Results.Ok(todos);
        }
    )
    .WithName("SearchTodos")
    .WithOpenApi();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();
