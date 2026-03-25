namespace McpServer.Clients.TodoApiClient;

/// <summary>
/// Represents a request to update an existing todo item via the Todo API.
/// </summary>
public class UpdateTodoRequest
{
    /// <summary>
    /// Gets or sets the updated title of the todo item.
    /// </summary>
    /// <value>The title text.</value>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated description of the todo item.
    /// </summary>
    /// <value>The description text.</value>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the todo item is completed.
    /// </summary>
    /// <value><c>true</c> if the todo item is completed; otherwise, <c>false</c>.</value>
    public bool IsCompleted { get; set; }
}
