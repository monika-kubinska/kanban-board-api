namespace KanbanApi.DTO;

public class TeamMemberRequest
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = "Member";
}