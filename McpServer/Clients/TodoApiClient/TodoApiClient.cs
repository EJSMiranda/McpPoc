using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace McpServer.Clients.TodoApiClient;

public class TodoApiClient : ITodoApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TodoApiClient> _logger;

    public TodoApiClient(HttpClient httpClient, ILogger<TodoApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<TodoItem>> GetAllTodosAsync()
    {
        _logger.LogDebug("Getting all todos from API");
        var response = await _httpClient.GetAsync("/api/todos");
        response.EnsureSuccessStatusCode();
        var todos =
            await response.Content.ReadFromJsonAsync<List<TodoItem>>() ?? new List<TodoItem>();
        _logger.LogInformation("Retrieved {Count} todos from API", todos.Count);
        return todos;
    }

    public async Task<TodoItem?> GetTodoByIdAsync(int id)
    {
        _logger.LogDebug("Getting todo with ID {TodoId}", id);
        var response = await _httpClient.GetAsync($"/api/todos/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Todo with ID {TodoId} not found", id);
            return null;
        }

        response.EnsureSuccessStatusCode();
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();
        if (todo == null)
        {
            _logger.LogWarning("Todo with ID {TodoId} returned null response", id);
            return null;
        }
        _logger.LogDebug("Retrieved todo with ID {TodoId}", id);
        return todo;
    }

    public async Task<TodoItem> CreateTodoAsync(CreateTodoRequest request)
    {
        _logger.LogDebug("Creating new todo with title: {Title}", request.Title);
        var response = await _httpClient.PostAsJsonAsync("/api/todos", request);
        response.EnsureSuccessStatusCode();
        var todo =
            await response.Content.ReadFromJsonAsync<TodoItem>()
            ?? throw new InvalidOperationException("Failed to create todo: response was null");
        _logger.LogInformation("Created todo with ID {TodoId}", todo.Id);
        return todo;
    }

    public async Task<TodoItem?> UpdateTodoAsync(int id, UpdateTodoRequest request)
    {
        _logger.LogDebug("Updating todo with ID {TodoId}", id);
        var response = await _httpClient.PutAsJsonAsync($"/api/todos/{id}", request);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Todo with ID {TodoId} not found for update", id);
            return null;
        }

        response.EnsureSuccessStatusCode();
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();
        if (todo == null)
        {
            _logger.LogWarning("Updated todo with ID {TodoId} returned null response", id);
            return null;
        }
        _logger.LogInformation("Updated todo with ID {TodoId}", id);
        return todo;
    }

    public async Task<bool> DeleteTodoAsync(int id)
    {
        _logger.LogDebug("Deleting todo with ID {TodoId}", id);
        var response = await _httpClient.DeleteAsync($"/api/todos/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Todo with ID {TodoId} not found for deletion", id);
            return false;
        }

        response.EnsureSuccessStatusCode();
        _logger.LogInformation("Deleted todo with ID {TodoId}", id);
        return true;
    }

    public async Task<List<TodoItem>> SearchTodosAsync(string query)
    {
        _logger.LogDebug("Searching todos with query: {Query}", query);
        var response = await _httpClient.GetAsync(
            $"/api/todos/search?q={Uri.EscapeDataString(query)}"
        );
        response.EnsureSuccessStatusCode();
        var todos =
            await response.Content.ReadFromJsonAsync<List<TodoItem>>() ?? new List<TodoItem>();
        _logger.LogDebug("Found {Count} todos matching query: {Query}", todos.Count, query);
        return todos;
    }

    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/health");
            var isHealthy = response.IsSuccessStatusCode;
            _logger.LogDebug("Health check result: {IsHealthy}", isHealthy);
            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return false;
        }
    }
}
