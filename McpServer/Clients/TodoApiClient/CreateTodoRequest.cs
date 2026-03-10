namespace McpServer.Clients.TodoApiClient;

public class CreateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
