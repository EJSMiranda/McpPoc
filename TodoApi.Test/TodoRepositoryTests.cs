using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Test;

/// <summary>
/// Contains unit tests for the <see cref="TodoRepository"/> class.
/// </summary>
[TestClass]
public sealed class TodoRepositoryTests
{
    private TodoContext _context = null!;
    private TodoRepository _repository = null!;
    private Mock<ILogger<TodoRepository>> _loggerMock = null!;

    /// <summary>
    /// Sets up the test environment before each test method execution.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: $"TodoTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new TodoContext(options);
        _loggerMock = new Mock<ILogger<TodoRepository>>();
        _repository = new TodoRepository(_context, _loggerMock.Object);
    }

    /// <summary>
    /// Cleans up the test environment after each test method execution.
    /// </summary>
    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Tests that GetAllAsync returns an empty list when the database is empty.
    /// </summary>
    [TestMethod]
    public async Task Given_EmptyDatabase_When_GetAllAsync_Then_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    /// <summary>
    /// Tests that CreateAsync successfully creates a todo item and returns it.
    /// </summary>
    [TestMethod]
    public async Task Given_ValidCreateTodoRequest_When_CreateAsync_Then_ReturnsCreatedTodo()
    {
        // Arrange
        var request = new CreateTodoRequest
        {
            Title = "Test Task",
            Description = "Test Description",
        };

        // Act
        var result = await _repository.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Id > 0);
        Assert.AreEqual("Test Task", result.Title);
        Assert.AreEqual("Test Description", result.Description);
        Assert.IsFalse(result.IsCompleted);
        Assert.IsTrue((DateTime.UtcNow - result.CreatedAt).TotalSeconds < 5);
    }

    /// <summary>
    /// Tests that GetByIdAsync returns the correct todo item when given an existing ID.
    /// </summary>
    [TestMethod]
    public async Task Given_ExistingTodoId_When_GetByIdAsync_Then_ReturnsTodo()
    {
        // Arrange
        var request = new CreateTodoRequest { Title = "Test", Description = "Desc" };
        var created = await _repository.CreateAsync(request, CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(created.Id, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(created.Id, result.Id);
        Assert.AreEqual("Test", result.Title);
    }

    /// <summary>
    /// Tests that GetByIdAsync returns null when given a non-existent ID.
    /// </summary>
    [TestMethod]
    public async Task Given_NonExistentTodoId_When_GetByIdAsync_Then_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests that UpdateAsync successfully updates a todo item and returns the updated version.
    /// </summary>
    [TestMethod]
    public async Task Given_ValidUpdateRequest_When_UpdateAsync_Then_ReturnsUpdatedTodo()
    {
        // Arrange
        var createRequest = new CreateTodoRequest { Title = "Original", Description = "Original" };
        var created = await _repository.CreateAsync(createRequest, CancellationToken.None);

        var updateRequest = new UpdateTodoRequest
        {
            Title = "Updated",
            Description = "Updated Description",
            IsCompleted = true,
        };

        // Act
        var result = await _repository.UpdateAsync(
            created.Id,
            updateRequest,
            CancellationToken.None
        );

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(created.Id, result.Id);
        Assert.AreEqual("Updated", result.Title);
        Assert.AreEqual("Updated Description", result.Description);
        Assert.IsTrue(result.IsCompleted);
    }

    /// <summary>
    /// Tests that UpdateAsync returns null when given a non-existent ID.
    /// </summary>
    [TestMethod]
    public async Task Given_NonExistentTodoId_When_UpdateAsync_Then_ReturnsNull()
    {
        // Arrange
        var request = new UpdateTodoRequest
        {
            Title = "New",
            Description = "New Description",
            IsCompleted = true,
        };

        // Act
        var result = await _repository.UpdateAsync(999, request, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests that DeleteAsync returns true and successfully deletes an existing todo item.
    /// </summary>
    [TestMethod]
    public async Task Given_ExistingTodoId_When_DeleteAsync_Then_ReturnsTrue()
    {
        // Arrange
        var request = new CreateTodoRequest { Title = "To Delete", Description = "Desc" };
        var created = await _repository.CreateAsync(request, CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(created.Id, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);

        // Verify deletion
        var deleted = await _repository.GetByIdAsync(created.Id, CancellationToken.None);
        Assert.IsNull(deleted);
    }

    /// <summary>
    /// Tests that DeleteAsync returns false when given a non-existent ID.
    /// </summary>
    [TestMethod]
    public async Task Given_NonExistentTodoId_When_DeleteAsync_Then_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that SearchAsync returns only todo items matching the search query.
    /// </summary>
    /// <summary>
    /// Tests that SearchAsync returns all todo items when given an empty search query.
    /// </summary>
    /// <summary>
    /// Tests that SearchAsync returns todo items matching the search query in descriptions.
    /// </summary>
    [TestMethod]
    public async Task Given_SearchQueryMatchingDescription_When_SearchAsync_Then_ReturnsMatchingTodos()
    {
        // Arrange
        await _repository.CreateAsync(
            new CreateTodoRequest { Title = "Task A", Description = "Important meeting" },
            CancellationToken.None
        );
        await _repository.CreateAsync(
            new CreateTodoRequest { Title = "Task B", Description = "Regular checkup" },
            CancellationToken.None
        );

        // Act
        var result = await _repository.SearchAsync("meeting", CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Task A", result.First().Title);
    }
}
