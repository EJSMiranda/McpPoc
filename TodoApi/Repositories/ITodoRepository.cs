using TodoApi.Models;

namespace TodoApi.Repositories;

/// <summary>
/// Defines the contract for todo item data access operations.
/// </summary>
public interface ITodoRepository
{
    /// <summary>
    /// Retrieves all todo items asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of all todo items.
    /// </returns>
    Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a todo item by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the todo item if found;
    /// otherwise, <c>null</c>.
    /// </returns>
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new todo item asynchronously.
    /// </summary>
    /// <param name="request">The todo creation request containing title and description.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the created todo item.
    /// </returns>
    Task<TodoItem> CreateAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing todo item asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item to update.</param>
    /// <param name="request">The todo update request containing new values.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the updated todo item if found;
    /// otherwise, <c>null</c>.
    /// </returns>
    Task<TodoItem?> UpdateAsync(
        int id,
        UpdateTodoRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes a todo item by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing <c>true</c> if the todo item
    /// was deleted; otherwise, <c>false</c> if the todo item was not found.
    /// </returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for todo items containing the specified query in title or description asynchronously.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of matching todo items.
    /// </returns>
    Task<IEnumerable<TodoItem>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default
    );
}
