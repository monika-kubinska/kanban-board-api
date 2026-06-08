using KanbanApi.Data;
using KanbanApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace KanbanApi.Controllers;

[ApiController]
[Route("teams")]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TeamsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetTeams()
    {
        return Ok(_db.Teams.ToList());
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] Team team)
    {
        team.Id = Guid.NewGuid();

        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        return Ok(team);
    }
}