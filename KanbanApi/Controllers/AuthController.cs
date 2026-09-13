using BCrypt.Net;
using KanbanApi.Data;
using KanbanApi.DTO;
using KanbanApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KanbanApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Name = req.Name,
            Role = UserRole.TeamMember
        };

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

        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT signing key is not configured.");
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}