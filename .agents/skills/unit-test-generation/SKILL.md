---
name: unit-test-generation
description: Generate comprehensive unit tests for .NET projects following repository patterns
license: MIT
compatibility: opencode
metadata:
  audience: developers
  workflow: testing
---

## What I do

- Analyze existing code to understand testing patterns and conventions
- Generate MSTest unit tests with proper Arrange-Act-Assert structure
- Create test projects following repository conventions (naming, structure, dependencies)
- Implement tests for repositories, services, controllers, and MCP tools
- Use appropriate mocking (Moq) for dependencies
- Follow Given-When-Then naming convention for test methods
- Set up proper test initialization and cleanup
- Handle async/await patterns correctly
- Generate tests for edge cases and error scenarios

## When to use me

Use this skill when:
- Adding new features that require unit tests
- Creating tests for existing untested code
- Setting up test projects for new components
- Following the repository's established testing patterns
- Ensuring test coverage for critical paths

## Testing Patterns in this Repository

### Project Structure
- Test projects follow naming convention: `{ProjectName}.Test`
- Test projects reference the main project being tested
- Use MSTest framework with Moq for mocking
- Global usings configured in .csproj for common namespaces

### Test Class Structure
```csharp
[TestClass]
public sealed class {ClassName}Tests
{
    private {DependencyType} _dependency = null!;
    
    [TestInitialize]
    public void Setup()
    {
        // Initialize dependencies, mocks, or test data
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        // Clean up resources (e.g., dispose DbContext)
    }
    
    [TestMethod]
    public async Task Given_{State}_When_{Action}_Then_{Result}()
    {
        // Arrange
        // Set up test data and expectations
        
        // Act
        // Execute the method being tested
        
        // Assert
        // Verify results and behavior
    }
}
```

### Repository Tests
- Use InMemory database for Entity Framework tests
- Test CRUD operations with edge cases (null returns, not found scenarios)
- Follow pattern: `Given_EmptyDatabase_When_GetAllAsync_Then_ReturnsEmptyList`
- Verify database state changes where appropriate

### Service/Client Tests
- Mock dependencies using Moq
- Verify method calls with `Times.Once`, `Times.Never`
- Test both success and failure scenarios
- Handle JSON serialization/deserialization for MCP tools

### Test Naming Convention
- Use `Given_{Precondition}_When_{Action}_Then_{ExpectedResult}`
- Be descriptive about the scenario being tested
- Include async suffix for async methods: `GetAllAsync`
- Use camelCase for parameter names in DataRow tests

### Common Assertions
- `Assert.IsNotNull(result)` for non-null returns
- `Assert.AreEqual(expected, actual)` for value comparisons
- `Assert.IsTrue(condition)` for boolean conditions
- `Assert.IsNull(result)` for null returns
- Verify mock interactions with `_mock.Verify()`

### Async Patterns
- All async methods use `async Task` return type
- Use `await` for async operations
- Test methods are async and return `Task`
- Handle async exceptions appropriately

### Data-Driven Tests
- Use `[DataRow]` attribute for parameterized tests
- Provide multiple test cases for the same scenario
- Keep test method parameters descriptive

## How to use this skill

1. **Analyze the target code**: Examine the class/method to understand its dependencies and behavior
2. **Identify test scenarios**: Determine success paths, edge cases, and error conditions
3. **Check existing patterns**: Look at similar tests in the repository for consistency
4. **Generate test structure**: Create test class with proper setup/cleanup
5. **Implement test methods**: Write tests following Arrange-Act-Assert pattern
6. **Verify with existing tests**: Ensure new tests match repository conventions
7. **Testing Integrity**: Never modify production code to make tests pass. If tests reveal bugs or issues, inform the user to decide the appropriate course of action.

## Example Output

When asked to generate tests for a new repository method `GetByCategoryAsync`, the skill will produce:

```csharp
[TestMethod]
public async Task Given_ExistingCategory_When_GetByCategoryAsync_Then_ReturnsMatchingItems()
{
    // Arrange
    var category = "Work";
    await _repository.CreateAsync(new CreateItemRequest { Title = "Task 1", Category = "Work" });
    await _repository.CreateAsync(new CreateItemRequest { Title = "Task 2", Category = "Personal" });
    
    // Act
    var result = await _repository.GetByCategoryAsync(category);
    
    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual(1, result.Count());
    Assert.AreEqual("Task 1", result.First().Title);
}

[TestMethod]
public async Task Given_NonExistentCategory_When_GetByCategoryAsync_Then_ReturnsEmptyList()
{
    // Act
    var result = await _repository.GetByCategoryAsync("NonExistent");
    
    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual(0, result.Count());
}
```