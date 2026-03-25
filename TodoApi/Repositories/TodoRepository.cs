using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TodoContext _context;
    private readonly ILogger<TodoRepository> _logger;

    public TodoRepository(TodoContext context, ILogger<TodoRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _unitOfWork = context;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            _logger.LogDebug("Getting all todo items");
            return await _context.TodoItems.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all todo items");
            throw;
        }
    }

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting todo item with ID {TodoId}", id);
            return await _context.TodoItems.FindAsync([id], cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todo item with ID {TodoId}", id);
            throw;
        }
    }

    public async Task<TodoItem> CreateAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            _logger.LogDebug("Creating new todo item with title: {Title}", request.Title);

            var todoItem = new TodoItem
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
            };

            _context.TodoItems.Add(todoItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created todo item with ID {TodoId}", todoItem.Id);
            return todoItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo item with title: {Title}", request.Title);
            throw;
        }
    }

    public async Task<TodoItem?> UpdateAsync(
        int id,
        UpdateTodoRequest request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            _logger.LogDebug("Updating todo item with ID {TodoId}", id);

            var todoItem = await _context.TodoItems.FindAsync([id], cancellationToken);
            if (todoItem == null)
            {
                _logger.LogWarning("Todo item with ID {TodoId} not found for update", id);
                return null;
            }

            todoItem.Title = request.Title;
            todoItem.Description = request.Description;
            todoItem.IsCompleted = request.IsCompleted;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated todo item with ID {TodoId}", id);
            return todoItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo item with ID {TodoId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting todo item with ID {TodoId}", id);

            var todoItem = await _context.TodoItems.FindAsync([id], cancellationToken);
            if (todoItem == null)
            {
                _logger.LogWarning("Todo item with ID {TodoId} not found for deletion", id);
                return false;
            }

            _context.TodoItems.Remove(todoItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted todo item with ID {TodoId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo item with ID {TodoId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TodoItem>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            _logger.LogDebug("Searching todo items with query: {Query}", query);

            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAllAsync(cancellationToken);
            }

            return await _context
                .TodoItems.Where(t =>
                    t.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || t.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
                )
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching todo items with query: {Query}", query);
            throw;
        }
    }
}
