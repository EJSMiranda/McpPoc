using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly TodoContext _context;

    public TodoRepository(TodoContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await _context.TodoItems.ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        return await _context.TodoItems.FindAsync(id);
    }

    public async Task<TodoItem> CreateAsync(CreateTodoRequest request)
    {
        var todoItem = new TodoItem
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
        };

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();
        return todoItem;
    }

    public async Task<TodoItem?> UpdateAsync(int id, UpdateTodoRequest request)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
            return null;

        todoItem.Title = request.Title;
        todoItem.Description = request.Description;
        todoItem.IsCompleted = request.IsCompleted;

        await _context.SaveChangesAsync();
        return todoItem;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
            return false;

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TodoItem>> SearchAsync(string query)
    {
        return await _context
            .TodoItems.Where(t => t.Title.Contains(query) || t.Description.Contains(query))
            .ToListAsync();
    }
}
