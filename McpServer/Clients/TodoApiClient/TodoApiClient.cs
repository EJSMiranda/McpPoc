using System.Net.Http.Json;

namespace McpServer.Clients.TodoApiClient;

public class TodoApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public TodoApiClient(string baseUrl = "http://localhost:5266")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
    }

    public async Task<List<TodoItem>> GetAllTodosAsync()
    {
        var response = await _httpClient.GetAsync("/api/todos");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<TodoItem>>() ?? new List<TodoItem>();
    }

    public async Task<TodoItem?> GetTodoByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/todos/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItem>();
    }

    public async Task<TodoItem> CreateTodoAsync(CreateTodoRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/todos", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItem>()
            ?? throw new Exception("Failed to create todo");
    }

    public async Task<TodoItem?> UpdateTodoAsync(int id, UpdateTodoRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/todos/{id}", request);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItem>();
    }

    public async Task<bool> DeleteTodoAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/todos/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return false;

        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<List<TodoItem>> SearchTodosAsync(string query)
    {
        var response = await _httpClient.GetAsync(
            $"/api/todos/search?q={Uri.EscapeDataString(query)}"
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<TodoItem>>() ?? new List<TodoItem>();
    }

    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
