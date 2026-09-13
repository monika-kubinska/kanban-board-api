using KanbanApi.Data;
using KanbanApi.DTO;
using KanbanApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanApi.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        {
            if (!ItemStateExtensions.TryParse(state, out var parsedState))
                return BadRequest("Unknown item state.");

            query = query.Where(i => i.State == parsedState);
        }

        return Ok(query.ToList());
    }

    [HttpGet("backlog/{teamId:guid}")]
    public async Task<IActionResult> GetBacklog(Guid teamId)
    {
        if (!await _db.Teams.AnyAsync(team => team.Id == teamId))
            return NotFound();

        var items = await _db.Items
            .Where(item => item.TeamId == teamId)
            .OrderBy(item => item.State)
            .ThenBy(item => item.Title)
            .ToListAsync();

        return Ok(items);
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

    [HttpPost("{id}/state")]
    public async Task<IActionResult> ChangeState(Guid id, [FromBody] ChangeItemStateRequest request)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null)
            return NotFound();

        item.State = request.State;
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