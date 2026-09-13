namespace KanbanApi.DTO;

using KanbanApi.Models;

public class ChangeItemStateRequest
{
    public ItemState State { get; set; }
}