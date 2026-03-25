namespace McpServer.Clients.TodoApiClient;

/// <summary>
/// Represents a request to create a new todo item via the Todo API.
/// </summary>
public class CreateTodoRequest
{
    /// <summary>
    /// Gets or sets the title of the todo item to create.
    /// </summary>
    /// <value>The title text.</value>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the todo item to create.
    /// </summary>
    /// <value>The description text.</value>
    public string Description { get; set; } = string.Empty;
}
