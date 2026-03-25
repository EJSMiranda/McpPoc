namespace TodoApi.Models;

/// <summary>
/// Represents a todo item in the system.
/// </summary>
public class TodoItem
{
    /// <summary>
    /// Gets or sets the unique identifier of the todo item.
    /// </summary>
    /// <value>The unique identifier.</value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the todo item.
    /// </summary>
    /// <value>The title text.</value>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the todo item.
    /// </summary>
    /// <value>The description text.</value>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the todo item is completed.
    /// </summary>
    /// <value><c>true</c> if the todo item is completed; otherwise, <c>false</c>.</value>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the todo item was created.
    /// </summary>
    /// <value>The creation timestamp in UTC.</value>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
