using System.Net;
using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record MarkItemCompleted([property: Identity] Guid Id, string Key);

public static class MarkItemCompletedHandler
{
    [WolverinePut("/api/checklist/complete")]
    [AggregateHandler]
    public static (IResult, Events) Handle(MarkItemCompleted cmd, CheckList entity)
    {
        var item = entity.PendingItems.SingleOrDefault(i => i.Key == cmd.Key);
        if (item is null)
            return (Results.Problem("The key could not be found", statusCode: StatusCodes.Status404NotFound), []);

        return (Results.Accepted(), [ItemMarkedCompleted.From(cmd)]);
    }
}

public record ItemMarkedCompleted(Guid Id, string Key)
{
    public static ItemMarkedCompleted From(MarkItemCompleted cmd) => new(cmd.Id, cmd.Key);
}