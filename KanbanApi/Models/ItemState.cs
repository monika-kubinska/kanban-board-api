using System.Text.Json.Serialization;

namespace KanbanApi.Models;

public enum ItemState
{
    [JsonStringEnumMemberName("Ready")]
    Ready,

    [JsonStringEnumMemberName("In Progress")]
    InProgress,

    [JsonStringEnumMemberName("Code Review")]
    CodeReview,

    [JsonStringEnumMemberName("In Test")]
    InTest,

    [JsonStringEnumMemberName("Ready for Production")]
    ReadyForProduction,

    [JsonStringEnumMemberName("Done")]
    Done
}

public static class ItemStateExtensions
{
    public static string ToStorageValue(this ItemState state) => state switch
    {
        ItemState.Ready => "Ready",
        ItemState.InProgress => "In Progress",
        ItemState.CodeReview => "Code Review",
        ItemState.InTest => "In Test",
        ItemState.ReadyForProduction => "Ready for Production",
        ItemState.Done => "Done",
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
    };

    public static bool TryParse(string? value, out ItemState state)
    {
        state = value?.Trim() switch
        {
            "Ready" => ItemState.Ready,
            "To Do" => ItemState.Ready,
            "ToDo" => ItemState.Ready,
            "In Progress" => ItemState.InProgress,
            "InProgress" => ItemState.InProgress,
            "Code Review" => ItemState.CodeReview,
            "CodeReview" => ItemState.CodeReview,
            "In Test" => ItemState.InTest,
            "InTest" => ItemState.InTest,
            "Ready for Production" => ItemState.ReadyForProduction,
            "ReadyForProduction" => ItemState.ReadyForProduction,
            "Done" => ItemState.Done,
            _ => default
        };

        return value is "Ready" or "To Do" or "ToDo" or
            "In Progress" or "InProgress" or "Code Review" or "CodeReview" or
            "In Test" or "InTest" or "Ready for Production" or "ReadyForProduction" or "Done";
    }

    public static ItemState FromStorageValue(string value)
    {
        if (TryParse(value, out var state))
            return state;

        throw new InvalidOperationException($"Unknown item state: {value}");
    }
}