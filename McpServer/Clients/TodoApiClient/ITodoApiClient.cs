namespace McpServer.Clients.TodoApiClient;

public interface ITodoApiClient
{
    Task<List<TodoItem>> GetAllTodosAsync();
    Task<TodoItem?> GetTodoByIdAsync(int id);
    Task<TodoItem> CreateTodoAsync(CreateTodoRequest request);
    Task<TodoItem?> UpdateTodoAsync(int id, UpdateTodoRequest request);
    Task<bool> DeleteTodoAsync(int id);
    Task<List<TodoItem>> SearchTodosAsync(string query);
    Task<bool> HealthCheckAsync();
}
