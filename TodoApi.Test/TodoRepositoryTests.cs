using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Test;

[TestClass]
public sealed class TodoRepositoryTests
{
    private TodoContext _context = null!;
    private TodoRepository _repository = null!;
    private Mock<ILogger<TodoRepository>> _loggerMock = null!;

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

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public async Task Given_EmptyDatabase_When_GetAllAsync_Then_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

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

    [TestMethod]
    public async Task Given_NonExistentTodoId_When_GetByIdAsync_Then_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Given_ExistingTodoId_When_UpdateAsync_Then_ReturnsUpdatedTodo()
    {
        // Arrange
        var createRequest = new CreateTodoRequest { Title = "Old", Description = "Old Desc" };
        var created = await _repository.CreateAsync(createRequest, CancellationToken.None);

        var updateRequest = new UpdateTodoRequest
        {
            Title = "Updated",
            Description = "Updated Desc",
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
        Assert.AreEqual("Updated Desc", result.Description);
        Assert.IsTrue(result.IsCompleted);
    }

    [TestMethod]
    public async Task Given_NonExistentTodoId_When_UpdateAsync_Then_ReturnsNull()
    {
        // Arrange
        var request = new UpdateTodoRequest
        {
            Title = "New",
            Description = "New Desc",
            IsCompleted = true,
        };

        // Act
        var result = await _repository.UpdateAsync(999, request, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

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

        // Verify it was actually deleted
        var shouldBeNull = await _repository.GetByIdAsync(created.Id, CancellationToken.None);
        Assert.IsNull(shouldBeNull);
    }

    [TestMethod]
    public async Task Given_NonExistentTodoId_When_DeleteAsync_Then_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Given_TodosWithMatchingAndNonMatchingTitles_When_SearchAsync_Then_ReturnsMatchingTodos()
    {
        // Arrange
        await _repository.CreateAsync(
            new CreateTodoRequest { Title = "Buy milk", Description = "From supermarket" },
            CancellationToken.None
        );
        await _repository.CreateAsync(
            new CreateTodoRequest { Title = "Call doctor", Description = "Schedule appointment" },
            CancellationToken.None
        );
        await _repository.CreateAsync(
            new CreateTodoRequest { Title = "Milk shake", Description = "Make dessert" },
            CancellationToken.None
        );

        // Act
        var result = await _repository.SearchAsync("milk", CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        // Now case-insensitive, so both "Buy milk" and "Milk shake" match
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(
            result.All(t => t.Title.Contains("milk", StringComparison.OrdinalIgnoreCase))
        );
    }
}
