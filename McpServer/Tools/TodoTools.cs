using System.ComponentModel;
using System.Text.Json;
using McpServer.Clients.TodoApiClient;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

/// <summary>
/// Provides MCP tools for interacting with todo items via the Todo API.
/// </summary>
[McpServerToolType]
public static class TodoTools
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Lists all todo items from the Todo API.
    /// </summary>
    /// <param name="client">The Todo API client used to communicate with the API.</param>
    /// <returns>
    /// A JSON-formatted string containing all todo items.
    /// </returns>
    /// <remarks>
    /// This tool calls the Todo API's GET /api/todos endpoint and returns the results
    /// as a formatted JSON string for display in the MCP client.
    /// </remarks>
    [McpServerTool, Description("List all todo items")]
    public static async Task<string> ListTodos(ITodoApiClient client)
    {
        var todos = await client.GetAllTodosAsync();
        return JsonSerializer.Serialize(todos, _jsonOptions);
    }

    /// <summary>
    /// Gets a specific todo item by its unique identifier.
    /// </summary>
    /// <param name="client">The Todo API client used to communicate with the API.</param>
    /// <param name="id">The unique identifier of the todo item.</param>
    /// <returns>
    /// A JSON-formatted string containing the todo item if found; otherwise, a message
    /// indicating the todo item was not found.
    /// </returns>
    /// <remarks>
    /// This tool calls the Todo API's GET /api/todos/{id} endpoint.
    /// </remarks>
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

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <param name="client">The Todo API client used to communicate with the API.</param>
    /// <param name="title">The title of the todo item to create.</param>
    /// <param name="description">The description of the todo item to create.</param>
    /// <returns>
    /// A JSON-formatted string containing the created todo item.
    /// </returns>
    /// <remarks>
    /// This tool calls the Todo API's POST /api/todos endpoint.
    /// </remarks>
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

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="client">The Todo API client used to communicate with the API.</param>
    /// <param name="id">The unique identifier of the todo item to update.</param>
    /// <param name="title">The updated title of the todo item.</param>
    /// <param name="description">The updated description of the todo item.</param>
    /// <param name="isCompleted">A value indicating whether the todo item is completed.</param>
    /// <returns>
    /// A JSON-formatted string containing the updated todo item if found; otherwise, a message
    /// indicating the todo item was not found.
    /// </returns>
    /// <remarks>
    /// This tool calls the Todo API's PUT /api/todos/{id} endpoint.
    /// </remarks>
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

    /// <summary>
    /// Deletes a todo item by its unique identifier.
    /// </summary>
    /// <param name="client">The Todo API client used to communicate with the API.</param>
    /// <param name="id">The unique identifier of the todo item to delete.</param>
    /// <returns>
    /// A message indicating whether the todo item was deleted successfully or not found.
    /// </returns>
    /// <remarks>
    /// This tool calls the Todo API's DELETE /api/todos/{id} endpoint.
    /// </remarks>
    [McpServerTool, Description("Delete a todo item by ID")]
    public static async Task<string> DeleteTodo(
        ITodoApiClient client,
        [Description("Todo item ID")] int id
    )
    {
        var deleted = await client.DeleteTodoAsync(id);
        return deleted ? $"Todo with ID {id} deleted successfully" : $"Todo with ID {id} not found";
    }

    /// <summary>
    /// Searches for todo items containing the specified query in title or description.
    /// </summary>
    /// <param name="client">The Todo API client used to communicate with the API.</param>
    /// <param name="query">The search query string.</param>
    /// <returns>
    /// A JSON-formatted string containing matching todo items.
    /// </returns>
    /// <remarks>
    /// This tool calls the Todo API's GET /api/todos/search endpoint.
    /// </remarks>
    [McpServerTool, Description("Search todo items by content")]
    public static async Task<string> SearchTodos(
        ITodoApiClient client,
        [Description("Search query")] string query
    )
    {
        var todos = await client.SearchTodosAsync(query);
        return JsonSerializer.Serialize(todos, _jsonOptions);
    }

    /// <summary>
    /// Checks the health status of the Todo API.
    /// </summary>
    /// <param name="client">The Todo API client used to communicate with the API.</param>
    /// <returns>
    /// A message indicating whether the Todo API is healthy and responding.
    /// </returns>
    /// <remarks>
    /// This tool calls the Todo API's GET /health endpoint.
    /// </remarks>
    [McpServerTool, Description("Check if the Todo API is healthy")]
    public static async Task<string> HealthCheck(ITodoApiClient client)
    {
        var isHealthy = await client.HealthCheckAsync();
        return isHealthy ? "Todo API is healthy and responding" : "Todo API is not responding";
    }
}
