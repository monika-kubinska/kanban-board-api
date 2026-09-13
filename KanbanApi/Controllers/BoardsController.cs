using KanbanApi.Data;
using KanbanApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KanbanApi.Controllers;

[ApiController]
[Route("api/boards")]
[Authorize]
public class BoardsController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    [HttpGet("{teamId:guid}")]
    public async Task<IActionResult> GetBoard(Guid teamId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole(UserRole.Admin.ToString());

        var team = await _db.Teams
            .Where(team => team.Id == teamId &&
                (isAdmin ||
                 team.Members.Any(member => member.UserId == userId)))
            .Select(team => new
            {
                TeamId = team.Id,
                team.Name,
                BoardId = team.Board.Id,
                Items = _db.Items
                    .Where(item => item.TeamId == team.Id && item.State != ItemState.Ready)
                    .ToList(),
                WipLimits = _db.WipLimits
                    .Where(limit => limit.TeamId == team.Id)
                    .ToList()
            })
            .SingleOrDefaultAsync();

        return team == null ? NotFound() : Ok(team);
    }
}