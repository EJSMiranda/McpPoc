using McpServer.Clients.TodoApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Add logging
builder.Services.AddLogging(configure => configure.AddConsole());

// Configure HttpClient for TodoApiClient
builder.Services.AddHttpClient<ITodoApiClient, TodoApiClient>(
    (serviceProvider, client) =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var baseUrl = configuration["TodoApi:BaseUrl"] ?? "http://localhost:5266";
        client.BaseAddress = new Uri(baseUrl);
    }
);

// Add MCP server
builder.Services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();

var app = builder.Build();
await app.RunAsync();
