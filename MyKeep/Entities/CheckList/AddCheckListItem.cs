using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record AddCheckListItem([property: Identity] Guid Id);

public record AddCheckListItemKey(string Value);

public static class AddCheckListItemHandler
{

    public static AddCheckListItemKey Load(AddCheckListItem _) => new AddCheckListItemKey(KeyGenerator.GetUniqueKey(8));
    
    [WolverinePost("/api/checklist/add")]
    [AggregateHandler]
    public static (IResult, Events) Handle(AddCheckListItem cmd, CheckList entity, AddCheckListItemKey key)
    {
        return (Results.Accepted(value: new { Key = key }), [CheckListItemAdded.From(cmd, key.Value)]);
    }
}

public record CheckListItemAdded(Guid Id, string Key)
{
    public static CheckListItemAdded From(AddCheckListItem cmd, string key) => new(cmd.Id, key);
}