namespace KanbanApi.Models;

public class WipLimit
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string State { get; set; }
    public int Limit { get; set; }
}