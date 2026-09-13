namespace KanbanApi.Models;

public class Board
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Team Team { get; set; }
}