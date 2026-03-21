using System.ComponentModel;
using System.Text.Json;
using McpServer.Clients.TodoApiClient;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class TodoTools
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    [McpServerTool, Description("List all todo items")]
    public static async Task<string> ListTodos(ITodoApiClient client)
    {
        var todos = await client.GetAllTodosAsync();
        return JsonSerializer.Serialize(todos, _jsonOptions);
    }

    [McpServerTool, Description("Get a specific todo item by ID")]
    public static async Task<string> GetTodo(
        ITodoApiClient client,
        [Description("Todo item ID")] int id
    )
    {
        var todo = await client.GetTodoByIdAsync(id);
        return todo != null
            ? JsonSerializer.Serialize(todo, _jsonOptions)
            : $"Todo with ID {id} not found";
    }

    [McpServerTool, Description("Create a new todo item")]
    public static async Task<string> CreateTodo(
        ITodoApiClient client,
        [Description("Todo title")] string title,
        [Description("Todo description")] string description
    )
    {
        var request = new CreateTodoRequest { Title = title, Description = description };
        var todo = await client.CreateTodoAsync(request);
        return JsonSerializer.Serialize(todo, _jsonOptions);
    }

    [McpServerTool, Description("Update an existing todo item")]
    public static async Task<string> UpdateTodo(
        ITodoApiClient client,
        [Description("Todo item ID")] int id,
        [Description("Todo title")] string title,
        [Description("Todo description")] string description,
        [Description("Whether the todo is completed")] bool isCompleted
    )
    {
        var request = new UpdateTodoRequest
        {
            Title = title,
            Description = description,
            IsCompleted = isCompleted,
        };

        var todo = await client.UpdateTodoAsync(id, request);
        return todo != null
            ? JsonSerializer.Serialize(todo, _jsonOptions)
            : $"Todo with ID {id} not found";
    }

    [McpServerTool, Description("Delete a todo item by ID")]
    public static async Task<string> DeleteTodo(
        ITodoApiClient client,
        [Description("Todo item ID")] int id
    )
    {
        var deleted = await client.DeleteTodoAsync(id);
        return deleted ? $"Todo with ID {id} deleted successfully" : $"Todo with ID {id} not found";
    }

    [McpServerTool, Description("Search todo items by content")]
    public static async Task<string> SearchTodos(
        ITodoApiClient client,
        [Description("Search query")] string query
    )
    {
        var todos = await client.SearchTodosAsync(query);
        return JsonSerializer.Serialize(todos, _jsonOptions);
    }

    [McpServerTool, Description("Check if the Todo API is healthy")]
    public static async Task<string> HealthCheck(ITodoApiClient client)
    {
        var isHealthy = await client.HealthCheckAsync();
        return isHealthy ? "Todo API is healthy and responding" : "Todo API is not responding";
    }
}
