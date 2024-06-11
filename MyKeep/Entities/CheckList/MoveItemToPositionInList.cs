using JasperFx.Core;
using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record MoveItemToPositionInList([property: Identity] Guid Id, string Key, string? AfterKey);

public static class MoveItemToPositionInListHandler
{
    [WolverinePatch("/api/checklist/reorder")]
    [AggregateHandler]
    public static (IResult, Events) Handle(MoveItemToPositionInList cmd, CheckList entity)
    {
        if (cmd.Key == cmd.AfterKey) return (Results.Ok(), []);
        
        var toMoveItem = entity.PendingItems.SingleOrDefault(i => i.Key == cmd.Key);
        if (toMoveItem is null)
            return (Results.Problem("Could not find move key", statusCode: StatusCodes.Status404NotFound), []);

        if (cmd.AfterKey is null)
            return (Results.Accepted(), [ItemMovedToTopOfList.From(cmd)]);

        var afterItem = entity.PendingItems.SingleOrDefault(i => i.Key == cmd.AfterKey);
        if (afterItem is null)
            return (Results.Problem("Could not find after key", statusCode: StatusCodes.Status400BadRequest), []);

        return (Results.Accepted(), [ItemMovedAfterItemInList.From(cmd)]);
    }
}

public record ItemMovedAfterItemInList(Guid Id, string Key, string AfterKey)
{
    public static ItemMovedAfterItemInList From(MoveItemToPositionInList cmd) => new(cmd.Id, cmd.Key, cmd.AfterKey!);
}

public record ItemMovedToTopOfList(Guid Id, string Key)
{
    public static ItemMovedToTopOfList From(MoveItemToPositionInList cmd) => new(cmd.Id, cmd.Key);
}