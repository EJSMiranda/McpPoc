namespace McpServer.Clients.TodoApiClient;

/// <summary>
/// Defines the contract for communicating with the Todo API.
/// </summary>
public interface ITodoApiClient
{
    /// <summary>
    /// Retrieves all todo items from the Todo API asynchronously.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of all todo items.
    /// </returns>
    Task<List<TodoItem>> GetAllTodosAsync();

    /// <summary>
    /// Retrieves a specific todo item by its unique identifier from the Todo API asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the todo item if found;
    /// otherwise, <c>null</c>.
    /// </returns>
    Task<TodoItem?> GetTodoByIdAsync(int id);

    /// <summary>
    /// Creates a new todo item via the Todo API asynchronously.
    /// </summary>
    /// <param name="request">The todo creation request containing title and description.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the created todo item.
    /// </returns>
    Task<TodoItem> CreateTodoAsync(CreateTodoRequest request);

    /// <summary>
    /// Updates an existing todo item via the Todo API asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item to update.</param>
    /// <param name="request">The todo update request containing new values.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the updated todo item if found;
    /// otherwise, <c>null</c>.
    /// </returns>
    Task<TodoItem?> UpdateTodoAsync(int id, UpdateTodoRequest request);

    /// <summary>
    /// Deletes a todo item by its unique identifier via the Todo API asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item to delete.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing <c>true</c> if the todo item
    /// was deleted; otherwise, <c>false</c> if the todo item was not found.
    /// </returns>
    Task<bool> DeleteTodoAsync(int id);

    /// <summary>
    /// Searches for todo items containing the specified query in title or description via the Todo API asynchronously.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of matching todo items.
    /// </returns>
    Task<List<TodoItem>> SearchTodosAsync(string query);

    /// <summary>
    /// Checks the health status of the Todo API asynchronously.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, containing <c>true</c> if the API is healthy
    /// and responding; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> HealthCheckAsync();
}
