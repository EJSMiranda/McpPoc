# Build script for MCP PoC
Write-Host "Building MCP Proof of Concept..." -ForegroundColor Green

# Build Todo API
Write-Host "`nBuilding Todo API..." -ForegroundColor Cyan
dotnet build TodoApi/TodoApi.csproj --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Todo API build failed!" -ForegroundColor Red
    exit 1
}

# Build MCP Server
Write-Host "`nBuilding MCP Server..." -ForegroundColor Cyan
dotnet build McpServer/McpServer.csproj --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "MCP Server build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`nBuild completed successfully!" -ForegroundColor Green
Write-Host "`nTo run the Todo API:" -ForegroundColor Yellow
Write-Host "  cd TodoApi"
Write-Host "  dotnet run"
Write-Host "`nTo run the MCP Server:" -ForegroundColor Yellow
Write-Host "  cd McpServer"
Write-Host "  dotnet run"
Write-Host "`nAPI will be available at: http://localhost:5000" -ForegroundColor Yellow
Write-Host "Swagger UI: http://localhost:5000/swagger" -ForegroundColor Yellow