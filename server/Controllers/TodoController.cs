using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodolistAPI.Models;
using TodolistAPI.Data; 

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context)
    {
        _context = context;
    }

    // Create a new Todo 
    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateTodo([FromBody] TodoItem todoItem)
    {
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodo), new { id = todoItem.TodoItemId }, todoItem);
    }

    // Get all Todos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAllTodos()
    {
        return await _context.TodoItems.ToListAsync();
    }

    // Get a Todo
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodo(int id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        return todoItem;
    }

    // Update a Todo (PUT)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, [FromBody] TodoItem todoItem)
    {
        if (id != todoItem.TodoItemId)
        {
            return BadRequest();
        }

        _context.Entry(todoItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // Delete a Todo 
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Delete all Todos 
    [HttpDelete]
    public async Task<IActionResult> DeleteAllTodos()
    {
        var todos = await _context.TodoItems.ToListAsync();
        _context.TodoItems.RemoveRange(todos);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoExists(int id)
    {
        return _context.TodoItems.Any(e => e.TodoItemId == id);
    }
}
