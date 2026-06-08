namespace KanbanApi.Models;

public class Item
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string State { get; set; }
    public int? Estimation { get; set; }

    public Guid TeamId { get; set; }
    public Team Team { get; set; }

    public Guid? AssignedUserId { get; set; }
}