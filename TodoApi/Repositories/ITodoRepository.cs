using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(int id);
    Task<TodoItem> CreateAsync(CreateTodoRequest request);
    Task<TodoItem?> UpdateAsync(int id, UpdateTodoRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<TodoItem>> SearchAsync(string query);
}
