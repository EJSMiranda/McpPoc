using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TodoApi.Models;

namespace TodoApi.Data;

public class TodoContext : DbContext, IUnitOfWork
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public virtual DbSet<TodoItem> TodoItems { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.RollbackAsync(cancellationToken);
    }

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
