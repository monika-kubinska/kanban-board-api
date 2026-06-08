using KanbanApi.Data;
using KanbanApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace KanbanApi.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ItemsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Get([FromQuery] Guid? teamId, [FromQuery] string? state)
    {
        var query = _db.Items.AsQueryable();

        if (teamId.HasValue)
            query = query.Where(i => i.TeamId == teamId);

        if (!string.IsNullOrEmpty(state))
            query = query.Where(i => i.State == state);

        return Ok(query.ToList());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Item item)
    {
        item.Id = Guid.NewGuid();
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        return Ok(item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Item updated)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null) return NotFound();

        item.Title = updated.Title;
        item.State = updated.State;
        item.Type = updated.Type;

        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null) return NotFound();

        _db.Items.Remove(item);
        await _db.SaveChangesAsync();

        return Ok();
    }
}