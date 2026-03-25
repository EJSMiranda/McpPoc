using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TodoItem> CreateAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TodoItem?> UpdateAsync(
        int id,
        UpdateTodoRequest request,
        CancellationToken cancellationToken = default
    );
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItem>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default
    );
}
