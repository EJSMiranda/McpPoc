using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TodoApi.Models;

namespace TodoApi.Data;

/// <summary>
/// Represents the database context for todo items, implementing the unit of work pattern.
/// </summary>
public class TodoContext : DbContext, IUnitOfWork
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the database context.</param>
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    /// <summary>
    /// Gets or sets the collection of todo items in the database.
    /// </summary>
    /// <value>The database set of todo items.</value>
    public virtual DbSet<TodoItem> TodoItems { get; set; }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task CommitTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
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
