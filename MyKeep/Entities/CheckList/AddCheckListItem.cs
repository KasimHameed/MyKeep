using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record AddCheckListItem([property: Identity] Guid Id);

public static class AddCheckListItemHandler
{

    [WolverinePost("/api/checklist/add")]
    [AggregateHandler]
    public static (IResult, Events) Handle(AddCheckListItem cmd, CheckList entity)
    {
        var key = KeyGenerator.GetUniqueKey(8);
        return (Results.Accepted(value: new { Key = key }), [CheckListItemAdded.From(cmd, key)]);
    }
}

public record CheckListItemAdded(Guid Id, string Key)
{
    public static CheckListItemAdded From(AddCheckListItem cmd, string key) => new(cmd.Id, key);
}