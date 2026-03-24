# Agent Guidelines for MCP Proof of Concept

This document provides guidelines for AI agents working on the MCP Proof of Concept repository. It includes build commands, testing procedures, and code style conventions.

## Project Overview

This is a .NET 9 solution implementing a Model Context Protocol (MCP) server that interacts with a Todo API. The solution consists of:
- **TodoApi**: REST API for todo items (ASP.NET Core Minimal API)
- **McpServer**: MCP server exposing tools to interact with the Todo API
- **TodoApi.Test**: Unit tests for TodoApi
- **McpServer.Test**: Unit tests for McpServer

## Build Commands

### Full Build
```powershell
# Build all projects in Release configuration
.\build.ps1

# Alternative: Build using dotnet
dotnet build --configuration Release
```

### Individual Project Builds
```powershell
# Build Todo API
dotnet build TodoApi/TodoApi.csproj --configuration Release

# Build MCP Server
dotnet build McpServer/McpServer.csproj --configuration Release

# Build specific configuration (Debug/Release)
dotnet build --configuration Debug
```

### Clean Build
```powershell
# Clean all projects
dotnet clean

# Clean specific project
dotnet clean TodoApi/TodoApi.csproj
```

## Testing Commands

### Run All Tests
```powershell
# Run all tests in the solution
dotnet test

# Run tests with specific configuration
dotnet test --configuration Release
```

### Run Tests for Specific Project
```powershell
# Run TodoApi tests
dotnet test TodoApi.Test/TodoApi.Test.csproj

# Run McpServer tests
dotnet test McpServer.Test/McpServer.Test.csproj
```

### Run Single Test
```powershell
# Run specific test by name
dotnet test --filter "FullyQualifiedName~Given_EmptyDatabase_When_GetAllAsync_Then_ReturnsEmptyList"

# Run tests matching pattern
dotnet test --filter "FullyQualifiedName~GetAllAsync"
```

### List Available Tests
```powershell
# List all tests without running them
dotnet test --list-tests
```

### Test Output Options
```powershell
# Verbose output
dotnet test --verbosity normal

# Detailed test results
dotnet test --logger "console;verbosity=detailed"
```

## Running Applications

### Todo API
```powershell
cd TodoApi
dotnet run

# Run with specific environment
dotnet run --environment Development
```

### MCP Server
```powershell
cd McpServer
dotnet run

# Run published version
dotnet publish -c Release -o publish
dotnet publish/McpServer.dll
```

## Code Style Guidelines

### General Conventions
- **Target Framework**: .NET 9.0
- **Language Version**: Latest C# features enabled
- **Nullable Reference Types**: Enabled (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **File Encoding**: UTF-8 with BOM for .cs files

### Naming Conventions
- **Classes**: PascalCase (e.g., `TodoItem`, `TodoRepository`)
- **Methods**: PascalCase (e.g., `GetAllAsync`, `CreateAsync`)
- **Parameters**: camelCase (e.g., `todoId`, `createRequest`)
- **Local Variables**: camelCase (e.g., `var todoList = await repository.GetAllAsync()`)
- **Interfaces**: Prefix with 'I', PascalCase (e.g., `ITodoRepository`)
- **Test Methods**: Use Given_When_Then pattern (e.g., `Given_EmptyDatabase_When_GetAllAsync_Then_ReturnsEmptyList`)

### File Organization
- **Models**: Place in `Models/` directory
- **Data Access**: Place in `Data/` directory (DbContext)
- **Repositories**: Place in `Repositories/` directory
- **Clients**: Place in `Clients/` directory (HTTP clients)
- **Tools**: Place in `Tools/` directory (MCP tools)

### Imports and Usings
- Use `global using` statements in .csproj where appropriate
- Group imports by namespace:
  1. System namespaces
  2. Microsoft namespaces
  3. Third-party libraries
  4. Project namespaces
- Remove unused imports

### Error Handling
- Use try-catch blocks for expected exceptions
- Log exceptions using ILogger
- Return appropriate HTTP status codes in API (200, 400, 404, 500)
- Use `null` return values for "not found" scenarios in repositories
- Validate input parameters at API boundaries

### Async/Await Patterns
- Use `async`/`await` for all I/O operations
- Suffix async methods with `Async` (e.g., `GetAllAsync`)
- Use `ValueTask` for hot paths where appropriate
- ConfigureAwait(false) not needed in .NET Core/ASP.NET Core

### Testing Conventions
- Use MSTest framework
- Use Moq for mocking dependencies
- Use InMemory database for repository tests
- Follow Arrange-Act-Assert pattern
- Use `[TestInitialize]` and `[TestCleanup]` for setup/teardown
- Test names should describe scenario: `Given_[State]_When_[Action]_Then_[Result]`
- **Testing Integrity**: Never modify production code to make tests pass. If tests reveal bugs or issues, inform the user to decide the appropriate course of action.

### API Design (TodoApi)
- Use Minimal API pattern
- Use repository pattern for data access
- Use DTOs for request/response models
- Enable Swagger/OpenAPI documentation
- Configure CORS for development
- Use Entity Framework Core with SQLite

### MCP Server Design
- Use official ModelContextProtocol SDK (v1.1.0)
- Use `[McpServerTool]` attribute for tool methods
- Let SDK handle protocol, serialization, and error handling
- Use dependency injection for HttpClient
- Tools should return JSON-serializable results

## Project Structure Reference

```
McpPoc/
├── TodoApi/                    # REST API
│   ├── Models/                 # Data models (TodoItem, CreateTodoRequest, etc.)
│   ├── Data/                   # DbContext (TodoContext)
│   ├── Repositories/           # Repository pattern implementations
│   ├── Program.cs              # Minimal API configuration
│   └── TodoApi.csproj
├── McpServer/                  # MCP Server
│   ├── Clients/                # HTTP clients for API communication
│   ├── Tools/                  # MCP tool implementations
│   ├── Program.cs              # HostBuilder configuration
│   └── McpServer.csproj
├── TodoApi.Test/               # Unit tests for TodoApi
├── McpServer.Test/             # Unit tests for McpServer
├── build.ps1                   # Build script
├── McpPoc.sln                  # Solution file
└── README.md                   # Project documentation
```

## Common Tasks for Agents

### Adding New API Endpoints
1. Add model in `TodoApi/Models/`
2. Add repository method in `TodoApi/Repositories/`
3. Add endpoint in `TodoApi/Program.cs`
4. Add tests in `TodoApi.Test/`

### Adding New MCP Tools
1. Add tool method in `McpServer/Tools/` with `[McpServerTool]` attribute
2. Ensure method returns JSON-serializable result
3. Add tests in `McpServer.Test/`

### Database Changes
1. Update `TodoItem` model if needed
2. Update `TodoContext` if adding new DbSets
3. Run migrations (currently using InMemory/SQLite file)

### Running Code Quality Checks
```powershell
# Check for compilation errors
dotnet build --no-restore

# Run code analysis (if configured)
dotnet format --verify-no-changes
```

## Important Notes

- The project uses SQLite with a file-based database (`todo_temp.db`)
- CORS is configured to allow all origins in development
- MCP tools automatically convert between camelCase and snake_case
- Test database uses InMemory provider to avoid dependencies
- Always run tests after making changes
- Check build script (`build.ps1`) for standard build process
- **Code Language Policy**: Regardless of the user's language or agent responses, all code implementation must be in English (class names, method names, comments, etc.)

## Troubleshooting

### Build Issues
- Ensure .NET 9 SDK is installed (`dotnet --version`)
- Run `dotnet restore` if packages are missing
- Check for conflicting package versions

### Test Issues
- Tests use InMemory database - no external dependencies needed
- Ensure test methods follow MSTest conventions
- Check test output for specific failure details

### Runtime Issues
- Todo API runs on http://localhost:5000 by default
- MCP Server communicates with Todo API via HTTP
- Check `appsettings.json` for configuration
- Ensure both services are running for end-to-end testing