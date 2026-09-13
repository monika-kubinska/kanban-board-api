namespace KanbanApi.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; } = UserRole.TeamMember;

    public List<TeamMember> Teams { get; set; }
}
