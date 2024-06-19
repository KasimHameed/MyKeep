using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record AddCheckListItem([property: Identity] Guid Id);

public record Key(string Value);

public static class AddCheckListItemHandler
{
    public static Key Load() => new (KeyGenerator.GetUniqueKey(8));

    [WolverinePost("/api/checklist/add")]
    [AggregateHandler]
    public static (IResult, Events) Handle(AddCheckListItem cmd, CheckList entity, Key key)
    {
        return (Results.Accepted(value: new { Key = key.Value }), [CheckListItemAdded.From(cmd, key.Value)]);
    }
}

public record CheckListItemAdded(Guid Id, string Key)
{
    public static CheckListItemAdded From(AddCheckListItem cmd, string key) => new(cmd.Id, key);
}