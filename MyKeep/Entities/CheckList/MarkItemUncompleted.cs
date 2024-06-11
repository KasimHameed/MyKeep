using System.Net;
using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record MarkItemUncompleted([property: Identity] Guid Id, string Key);

public static class MarkItemUncompletedHandler
{
    [WolverinePut("/api/checklist/uncomplete")]
    [AggregateHandler]
    public static (IResult, Events) Handle(MarkItemUncompleted cmd, CheckList entity)
    {
        var item = entity.CompletedItems.SingleOrDefault(i => i.Key == cmd.Key);

        if (item is null)
            return (Results.Problem("Could not find key", statusCode: StatusCodes.Status404NotFound), []);

        return (Results.Accepted(), [ItemReactivated.From(cmd)]);
    }
}

public record ItemReactivated(Guid Id, string Key)
{
    public static ItemReactivated From(MarkItemUncompleted cmd) => new(cmd.Id, cmd.Key);
}