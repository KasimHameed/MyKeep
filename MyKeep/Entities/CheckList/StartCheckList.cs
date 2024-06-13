using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;


// COMMAND
public record StartCheckList([property: Identity]Guid Id);

public record StartCheckListKey(string Key);

// COMMAND HANDLER/DECIDER
public static class StartCheckListHandler
{
    public static StartCheckListKey Load(StartCheckList _) => new StartCheckListKey(KeyGenerator.GetUniqueKey(8));
    
    [WolverinePost("/api/checklist")]
    [AggregateHandler(AggregateType = typeof(CheckList))]
    public static (IResult, Events) Handle(StartCheckList cmd, LinkGenerator linkGenerator, StartCheckListKey key)
        => (Results.Accepted(linkGenerator.GetPathByPage("/Todo/Index", values: new{cmd.Id})), 
        [
            CheckListStarted.From(cmd),
            new CheckListItemAdded(cmd.Id, key.Key)
        ]);
}

public record CheckListStarted(Guid Id)
{
    public static CheckListStarted From(StartCheckList cmd) => new(cmd.Id);
}