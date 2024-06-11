using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record DeleteItem([property: Identity] Guid Id, string Key, bool IsCompleted);

public static class DeleteItemHandler
{
    [WolverineDelete("/api/checklist/item")]
    [AggregateHandler]
    public static (IResult, Events) Handle(DeleteItem cmd, CheckList entity)
    {
        var item = cmd.IsCompleted
            ? entity.CompletedItems.SingleOrDefault(i => i.Key == cmd.Key)
            : entity.PendingItems.SingleOrDefault(i => i.Key == cmd.Key);

        if (item is null)
            return (Results.Problem("Could not find item with key", statusCode: StatusCodes.Status404NotFound), []);

        return (Results.Accepted(),
            cmd.IsCompleted
                ? [ new CompletedItemDeleted(cmd.Id, cmd.Key) ]
                : [ new PendingItemDeleted(cmd.Id, cmd.Key) ]);
    }
}

public record PendingItemDeleted(Guid Id, string Key);

public record CompletedItemDeleted(Guid Id, string Key);