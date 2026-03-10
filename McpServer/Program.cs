using McpServer.Clients.TodoApiClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();
builder.Services.AddSingleton<TodoApiClient>();

var app = builder.Build();
await app.RunAsync();
