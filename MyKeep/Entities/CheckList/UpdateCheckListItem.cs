using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record UpdateCheckListItem([property: Identity] Guid Id, string Key, string Text, bool IsCompleted);

public static class UpdateCheckListItemHandler
{
    [WolverinePatch("/api/checklist")]
    [AggregateHandler]
    public static (IResult, Events) Handle(UpdateCheckListItem cmd, CheckList entity)
    {
        var items = cmd.IsCompleted ? entity.CompletedItems : entity.PendingItems;

        var item = items.SingleOrDefault(i => i.Key == cmd.Key);
        if (item is null) return (Results.Problem("Did not find key", statusCode: StatusCodes.Status404NotFound), []);

        if (item.Text == cmd.Text) return (Results.Ok(), []);

        return (Results.Accepted(),
            [cmd.IsCompleted ? CompletedItemTextUpdated.From(cmd) : PendingItemTextUpdated.From(cmd)]);
    }
}

public record PendingItemTextUpdated(Guid Id, string Text, string Key)
{
    public static PendingItemTextUpdated From(UpdateCheckListItem cmd) =>
        cmd.IsCompleted
            ? throw new InvalidOperationException("Cannot create pending item text update event for completed item")
            : new PendingItemTextUpdated(cmd.Id, cmd.Text, cmd.Key);
}

public record CompletedItemTextUpdated(Guid Id, string Text, string Key)
{
    public static CompletedItemTextUpdated From(UpdateCheckListItem cmd) =>
        cmd.IsCompleted
            ? new CompletedItemTextUpdated(cmd.Id, cmd.Text, cmd.Key)
            : throw new InvalidOperationException("Cannot create completed item text update event for incomplete item");
}