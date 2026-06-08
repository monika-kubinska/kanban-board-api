using BCrypt.Net;
using KanbanApi.Data;
using KanbanApi.DTO;
using KanbanApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace KanbanApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var user = new User { Id = Guid.NewGuid(), Email = req.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password), Name = req.Name };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return StatusCode(201);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest req)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == req.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized();

        return Ok(new { token = "mock-jwt-token" });
    }
}