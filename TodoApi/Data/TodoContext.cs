using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public virtual DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<TodoItem>()
            .HasData(
                new TodoItem
                {
                    Id = 1,
                    Title = "Comprar leche",
                    Description = "Ir al supermercado",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                },
                new TodoItem
                {
                    Id = 2,
                    Title = "Llamar al médico",
                    Description = "Pedir cita para revisión",
                    IsCompleted = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                },
                new TodoItem
                {
                    Id = 3,
                    Title = "Terminar proyecto MCP",
                    Description = "Completar prueba de concepto",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                }
            );
    }
}
