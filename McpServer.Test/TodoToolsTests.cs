using System.Text.Json;
using McpServer.Clients.TodoApiClient;
using McpServer.Tools;
using Moq;

namespace McpServer.Test;

/// <summary>
/// Contains unit tests for the <see cref="TodoTools"/> class.
/// </summary>
[TestClass]
public sealed class TodoToolsTests
{
    private Mock<ITodoApiClient> _mockApiClient = null!;

    /// <summary>
    /// Sets up the test environment before each test method execution.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _mockApiClient = new Mock<ITodoApiClient>();
    }

    /// <summary>
    /// Tests that ListTodos returns serialized JSON when the API client returns todo items.
    /// </summary>
    [TestMethod]
    public async Task Given_ApiClientReturnsTodos_When_ListTodos_Then_ReturnsSerializedJson()
    {
        // Arrange
        var todos = new List<TodoItem>
        {
            new()
            {
                Id = 1,
                Title = "Test 1",
                Description = "Desc 1",
            },
            new()
            {
                Id = 2,
                Title = "Test 2",
                Description = "Desc 2",
            },
        };

        _mockApiClient.Setup(c => c.GetAllTodosAsync()).ReturnsAsync(todos);

        // Act
        var result = await TodoTools.ListTodos(_mockApiClient.Object);

        // Assert
        Assert.IsNotNull(result);
        var deserialized = JsonSerializer.Deserialize<List<TodoItem>>(result);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized.Count);
        _mockApiClient.Verify(c => c.GetAllTodosAsync(), Times.Once);
    }

    /// <summary>
    /// Tests that GetTodo returns serialized JSON when the API client returns a todo item.
    /// </summary>
    [TestMethod]
    public async Task Given_ApiClientReturnsTodo_When_GetTodo_Then_ReturnsSerializedJson()
    {
        // Arrange
        var todo = new TodoItem
        {
            Id = 1,
            Title = "Test Todo",
            Description = "Test Description",
        };

        _mockApiClient.Setup(c => c.GetTodoByIdAsync(1)).ReturnsAsync(todo);

        // Act
        var result = await TodoTools.GetTodo(_mockApiClient.Object, 1);

        // Assert
        Assert.IsNotNull(result);
        var deserialized = JsonSerializer.Deserialize<TodoItem>(result);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(1, deserialized.Id);
        Assert.AreEqual("Test Todo", deserialized.Title);
        _mockApiClient.Verify(c => c.GetTodoByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that GetTodo returns a not found message when the API client returns null.
    /// </summary>
    [TestMethod]
    public async Task Given_ApiClientReturnsNull_When_GetTodo_Then_ReturnsNotFoundMessage()
    {
        // Arrange
        _mockApiClient.Setup(c => c.GetTodoByIdAsync(999)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await TodoTools.GetTodo(_mockApiClient.Object, 999);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Todo with ID 999 not found", result);
        _mockApiClient.Verify(c => c.GetTodoByIdAsync(999), Times.Once);
    }

    [DataRow("Task 1", "Description 1")]
    [DataRow("Another Task", "Another Description")]
    [TestMethod]
    public async Task Given_ValidTodoInput_When_CreateTodo_Then_ReturnsSerializedTodo(
        string title,
        string description
    )
    {
        // Arrange
        var createdTodo = new TodoItem
        {
            Id = 1,
            Title = title,
            Description = description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
        };

        _mockApiClient
            .Setup(c => c.CreateTodoAsync(It.IsAny<CreateTodoRequest>()))
            .ReturnsAsync(createdTodo);

        // Act
        var result = await TodoTools.CreateTodo(_mockApiClient.Object, title, description);

        // Assert
        Assert.IsNotNull(result);
        var deserialized = JsonSerializer.Deserialize<TodoItem>(result);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(1, deserialized.Id);
        Assert.AreEqual(title, deserialized.Title);
        Assert.AreEqual(description, deserialized.Description);
        _mockApiClient.Verify(
            c =>
                c.CreateTodoAsync(
                    It.Is<CreateTodoRequest>(r => r.Title == title && r.Description == description)
                ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Given_ExistingTodoId_When_UpdateTodo_Then_ReturnsSerializedTodo()
    {
        // Arrange
        var updatedTodo = new TodoItem
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            IsCompleted = true,
        };

        _mockApiClient
            .Setup(c => c.UpdateTodoAsync(1, It.IsAny<UpdateTodoRequest>()))
            .ReturnsAsync(updatedTodo);

        // Act
        var result = await TodoTools.UpdateTodo(
            _mockApiClient.Object,
            1,
            "Updated Title",
            "Updated Description",
            true
        );

        // Assert
        Assert.IsNotNull(result);
        var deserialized = JsonSerializer.Deserialize<TodoItem>(result);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(1, deserialized.Id);
        Assert.AreEqual("Updated Title", deserialized.Title);
        Assert.IsTrue(deserialized.IsCompleted);
        _mockApiClient.Verify(
            c =>
                c.UpdateTodoAsync(
                    1,
                    It.Is<UpdateTodoRequest>(r =>
                        r.Title == "Updated Title"
                        && r.Description == "Updated Description"
                        && r.IsCompleted == true
                    )
                ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Given_NonExistentTodoId_When_UpdateTodo_Then_ReturnsNotFoundMessage()
    {
        // Arrange
        _mockApiClient
            .Setup(c => c.UpdateTodoAsync(999, It.IsAny<UpdateTodoRequest>()))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await TodoTools.UpdateTodo(
            _mockApiClient.Object,
            999,
            "Title",
            "Description",
            false
        );

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Todo with ID 999 not found", result);
        _mockApiClient.Verify(
            c => c.UpdateTodoAsync(999, It.IsAny<UpdateTodoRequest>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Given_ExistingTodoId_When_DeleteTodo_Then_ReturnsSuccessMessage()
    {
        // Arrange
        _mockApiClient.Setup(c => c.DeleteTodoAsync(1)).ReturnsAsync(true);

        // Act
        var result = await TodoTools.DeleteTodo(_mockApiClient.Object, 1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Todo with ID 1 deleted successfully", result);
        _mockApiClient.Verify(c => c.DeleteTodoAsync(1), Times.Once);
    }

    [TestMethod]
    public async Task Given_NonExistentTodoId_When_DeleteTodo_Then_ReturnsNotFoundMessage()
    {
        // Arrange
        _mockApiClient.Setup(c => c.DeleteTodoAsync(999)).ReturnsAsync(false);

        // Act
        var result = await TodoTools.DeleteTodo(_mockApiClient.Object, 999);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Todo with ID 999 not found", result);
        _mockApiClient.Verify(c => c.DeleteTodoAsync(999), Times.Once);
    }

    [TestMethod]
    public async Task Given_QueryMatchingTodo_When_SearchTodos_Then_ReturnsSerializedResults()
    {
        // Arrange
        var todos = new List<TodoItem>
        {
            new()
            {
                Id = 1,
                Title = "Test",
                Description = "Search result",
            },
        };

        _mockApiClient.Setup(c => c.SearchTodosAsync("test")).ReturnsAsync(todos);

        // Act
        var result = await TodoTools.SearchTodos(_mockApiClient.Object, "test");

        // Assert
        Assert.IsNotNull(result);
        var deserialized = JsonSerializer.Deserialize<List<TodoItem>>(result);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(1, deserialized.Count);
        _mockApiClient.Verify(c => c.SearchTodosAsync("test"), Times.Once);
    }

    [TestMethod]
    public async Task Given_ApiHealthy_When_HealthCheck_Then_ReturnsHealthyMessage()
    {
        // Arrange
        _mockApiClient.Setup(c => c.HealthCheckAsync()).ReturnsAsync(true);

        // Act
        var result = await TodoTools.HealthCheck(_mockApiClient.Object);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Todo API is healthy and responding", result);
        _mockApiClient.Verify(c => c.HealthCheckAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Given_ApiUnhealthy_When_HealthCheck_Then_ReturnsUnhealthyMessage()
    {
        // Arrange
        _mockApiClient.Setup(c => c.HealthCheckAsync()).ReturnsAsync(false);

        // Act
        var result = await TodoTools.HealthCheck(_mockApiClient.Object);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Todo API is not responding", result);
        _mockApiClient.Verify(c => c.HealthCheckAsync(), Times.Once);
    }
}
