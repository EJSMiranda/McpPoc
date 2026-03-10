# MCP Proof of Concept - Todo List (Simplificado)

Esta es una prueba de concepto que implementa un servidor MCP (Model Context Protocol) en .NET 9 que interactúa con una API REST de lista de tareas (Todo List). **Versión simplificada** usando el SDK oficial de MCP.

## Arquitectura

El proyecto consiste en dos componentes principales:

1. **Todo API** - API REST con operaciones CRUD para gestionar tareas
2. **MCP Server** - Servidor MCP simplificado que expone herramientas para interactuar con la API

### Tecnologías Utilizadas
- .NET 9
- ASP.NET Core Minimal API
- Entity Framework Core con SQLite (archivo temporal)
- ModelContextProtocol SDK oficial
- Microsoft.Extensions.Hosting para HostBuilder
- System.Text.Json para serialización

## Estructura del Proyecto (Simplificada)

```
McpPoc/
├── TodoApi/                    # API REST
│   ├── Models/                 # Modelos de datos
│   ├── Data/                   # Contexto de base de datos
│   ├── Repositories/           # Patrón Repository
│   ├── Program.cs              # Configuración Minimal API
│   └── TodoApi.csproj
├── McpServer/                  # Servidor MCP Simplificado
│   ├── Clients/                # Cliente HTTP para API
│   ├── Tools/                  # Herramientas MCP con atributos
│   │   └── TodoTools.cs        # Definición de herramientas con [McpServerTool]
│   ├── Program.cs              # HostBuilder con AddMcpServer()
│   └── McpServer.csproj
├── build.ps1                   # Script de build
├── test_mcp.py                 # Script de prueba MCP
├── test_tool.py                # Script de prueba de herramientas
└── McpPoc.sln                  # Solución .NET
```

## Instalación y Ejecución

### Prerrequisitos
- .NET 9 SDK
- PowerShell (para script de build)

### Pasos para ejecutar

1. **Clonar y restaurar dependencias:**
   ```powershell
   dotnet restore
   ```

2. **Construir proyectos:**
   ```powershell
   .\build.ps1
   ```

3. **Ejecutar la API REST:**
   ```powershell
   cd TodoApi
   dotnet run
   ```
   La API estará disponible en: http://localhost:5000
   Swagger UI: http://localhost:5000/swagger

4. **Ejecutar el servidor MCP:**
   ```powershell
   cd McpServer
   dotnet run
   ```

## Endpoints de la API REST

### Operaciones CRUD
- `GET /api/todos` - Listar todas las tareas
- `GET /api/todos/{id}` - Obtener tarea por ID
- `POST /api/todos` - Crear nueva tarea
- `PUT /api/todos/{id}` - Actualizar tarea
- `DELETE /api/todos/{id}` - Eliminar tarea
- `GET /api/todos/search?q={query}` - Buscar tareas por contenido

### Modelo de datos
```json
{
  "id": 1,
  "title": "Comprar leche",
  "description": "Ir al supermercado",
  "isCompleted": false,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

## Herramientas MCP Disponibles

El servidor MCP expone las siguientes herramientas:

### `list_todos`
Lista todas las tareas.
```json
{
  "name": "list_todos",
  "description": "List all todo items"
}
```

### `get_todo`
Obtiene una tarea específica por ID.
```json
{
  "name": "get_todo",
  "description": "Get a specific todo item by ID",
  "inputSchema": {
    "type": "object",
    "properties": {
      "id": {
        "type": "number",
        "description": "Todo item ID"
      }
    },
    "required": ["id"]
  }
}
```

### `create_todo`
Crea una nueva tarea.
```json
{
  "name": "create_todo",
  "description": "Create a new todo item",
  "inputSchema": {
    "type": "object",
    "properties": {
      "title": {
        "type": "string",
        "description": "Todo title"
      },
      "description": {
        "type": "string",
        "description": "Todo description"
      }
    },
    "required": ["title", "description"]
  }
}
```

### `update_todo`
Actualiza una tarea existente.
```json
{
  "name": "update_todo",
  "description": "Update an existing todo item",
  "inputSchema": {
    "type": "object",
    "properties": {
      "id": {
        "type": "number",
        "description": "Todo item ID"
      },
      "title": {
        "type": "string",
        "description": "Todo title"
      },
      "description": {
        "type": "string",
        "description": "Todo description"
      },
      "isCompleted": {
        "type": "boolean",
        "description": "Whether the todo is completed"
      }
    },
    "required": ["id", "title", "description", "isCompleted"]
  }
}
```

### `delete_todo`
Elimina una tarea por ID.
```json
{
  "name": "delete_todo",
  "description": "Delete a todo item by ID",
  "inputSchema": {
    "type": "object",
    "properties": {
      "id": {
        "type": "number",
        "description": "Todo item ID"
      }
    },
    "required": ["id"]
  }
}
```

### `search_todos`
Busca tareas por contenido.
```json
{
  "name": "search_todos",
  "description": "Search todo items by content",
  "inputSchema": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "Search query"
      }
    },
    "required": ["query"]
  }
}
```

## Configuración MCP

Para integrar este servidor MCP con Claude Desktop o VS Code:

1. **Construir el servidor:**
   ```powershell
   cd McpServer
   dotnet publish -c Release -o publish
   ```

2. **Configurar en Claude Desktop:**
   Editar `~/AppData/Roaming/Claude/claude_desktop_config.json` (Windows) o `~/Library/Application Support/Claude/claude_desktop_config.json` (macOS):
   ```json
   {
     "mcpServers": {
       "todo-server": {
         "command": "dotnet",
         "args": ["C:\\ruta\\completa\\a\\McpServer\\publish\\McpServer.dll"],
         "env": {}
       }
     }
   }
   ```

3. **Configurar en VS Code con extensión Claude:**
   Editar la configuración de MCP servers en la extensión.

## Datos de Ejemplo

La base de datos incluye tareas de ejemplo:
1. "Comprar leche" - Pendiente
2. "Llamar al médico" - Completada
3. "Terminar proyecto MCP" - Pendiente

## Características Implementadas (Versión Simplificada)

### Todo API
- ✅ API REST completa con Minimal API
- ✅ Patrón Repository para acceso a datos
- ✅ Entity Framework Core con SQLite (archivo temporal)
- ✅ 6 endpoints CRUD + búsqueda + health check
- ✅ Documentación Swagger/OpenAPI automática
- ✅ CORS configurado para desarrollo local
- ✅ Datos de ejemplo pre-cargados

### MCP Server (Simplificado)
- ✅ SDK oficial ModelContextProtocol (v1.1.0)
- ✅ HostBuilder con `AddMcpServer()` y `WithToolsFromAssembly()`
- ✅ 7 herramientas definidas con atributos `[McpServerTool]`
- ✅ Generación automática de esquemas JSON desde parámetros
- ✅ Conversión automática de nombres (camelCase → snake_case)
- ✅ Inyección de dependencias automática (TodoApiClient)
- ✅ Protocolo stdio manejado por el SDK
- ✅ Manejo de errores automático

### Ventajas de la Simplificación
- **90% menos código**: De ~400 líneas a ~40 líneas en Program.cs
- **Mantenibilidad**: Código declarativo con atributos
- **Extensibilidad**: Agregar nuevas herramientas es tan simple como agregar métodos
- **Robustez**: SDK maneja protocolo, errores y serialización
- **Estándares**: Compatible con especificaciones MCP actuales

## Próximas Mejoras

1. **Persistencia real**: Reemplazar SQLite en memoria con PostgreSQL
2. **Autenticación**: Agregar JWT para seguridad
3. **Métricas**: Agregar logging y métricas de rendimiento
4. **Docker**: Crear contenedores Docker para cada componente
5. **Tests**: Agregar pruebas unitarias y de integración

## Licencia

Este proyecto es una prueba de concepto para demostrar la integración de MCP con .NET.