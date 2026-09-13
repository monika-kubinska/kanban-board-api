using KanbanApi.Data;
using KanbanApi.DTO;
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
            .Where(team => User.IsInRole(UserRole.Admin.ToString()) || team.Members.Any(member => member.UserId == userId))
            .Select(team => new
            {
                team.Id,
                team.Name,
                Members = team.Members.Select(member => new
                {
                    member.UserId,
                    member.User.Name,
                    member.User.Email
                }).ToList()
            })
            .ToListAsync();

        return Ok(teams);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Team name is required.");

        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Board = new Board { Id = Guid.NewGuid() },
            Members = new List<TeamMember>
            {
                new() { UserId = userId.Value }
            }
        };

        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            team.Id,
            team.Name
        });
    }

    [HttpPost("{teamId:guid}/join")]
    public async Task<IActionResult> JoinTeam(Guid teamId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        if (!await _db.Teams.AnyAsync(team => team.Id == teamId))
            return NotFound();

        if (await _db.TeamMembers.AnyAsync(member =>
            member.TeamId == teamId && member.UserId == userId.Value))
            return Conflict("User is already a member of this team.");

        _db.TeamMembers.Add(new TeamMember
        {
            TeamId = teamId,
            UserId = userId.Value
        });
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{teamId:guid}/leave")]
    public async Task<IActionResult> LeaveTeam(Guid teamId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var member = await _db.TeamMembers.FindAsync(userId.Value, teamId);
        if (member == null)
            return NotFound();

        _db.TeamMembers.Remove(member);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{teamId:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid teamId)
    {
        if (!await IsTeamMember(teamId))
            return NotFound();

        var members = await _db.TeamMembers
            .Where(member => member.TeamId == teamId)
            .Select(member => new
            {
                member.UserId,
                member.User.Name,
                member.User.Email
            })
            .ToListAsync();

        return Ok(members);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users
            .Select(user => new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Role
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("{teamId:guid}/members")]
    public async Task<IActionResult> AddMember(Guid teamId, TeamMemberRequest request)
    {
        if (!User.IsInRole(UserRole.Admin.ToString()))
            return Forbid();

        if (!await _db.Teams.AnyAsync(team => team.Id == teamId) ||
            !await _db.Users.AnyAsync(user => user.Id == request.UserId))
            return NotFound();

        if (await _db.TeamMembers.AnyAsync(member => member.TeamId == teamId && member.UserId == request.UserId))
            return Conflict("User is already a member of this team.");

        var member = new TeamMember
        {
            TeamId = teamId,
            UserId = request.UserId
        };
        _db.TeamMembers.Add(member);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            member.UserId,
            member.TeamId
        });
    }

    [HttpDelete("{teamId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId)
    {
        if (!User.IsInRole(UserRole.Admin.ToString()))
            return Forbid();

        var member = await _db.TeamMembers.FindAsync(userId, teamId);
        if (member == null)
            return NotFound();

        _db.TeamMembers.Remove(member);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;
    }

    private async Task<bool> IsTeamMember(Guid teamId)
    {
        var userId = GetCurrentUserId();
        return userId.HasValue && await _db.TeamMembers.AnyAsync(
            member => member.TeamId == teamId && member.UserId == userId.Value);
    }

}