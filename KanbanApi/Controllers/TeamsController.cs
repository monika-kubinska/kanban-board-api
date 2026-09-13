using KanbanApi.Data;
using KanbanApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KanbanApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TeamsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetTeams()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var teams = await _db.Teams
            .Where(team => team.Members.Any(member => member.UserId == userId))
            .ToListAsync();

        return Ok(teams);
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