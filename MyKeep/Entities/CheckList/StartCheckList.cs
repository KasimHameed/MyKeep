using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;


// COMMAND
public record StartCheckList([property: Identity]Guid Id);

public record StartCheckListData(string? Link, string Key);

// COMMAND HANDLER/DECIDER
public static class StartCheckListHandler
{
    public static StartCheckListData Load(StartCheckList cmd, LinkGenerator linkGenerator) => 
        new StartCheckListData(linkGenerator.GetPathByPage("/Todo/Index", values: new{cmd.Id}), KeyGenerator.GetUniqueKey(8));
    
    [WolverinePost("/api/checklist")]
    [AggregateHandler(AggregateType = typeof(CheckList))]
    public static (IResult, Events) Handle(StartCheckList cmd, StartCheckListData data)
        => (Results.Accepted(data.Link), 
        [
            CheckListStarted.From(cmd),
            new CheckListItemAdded(cmd.Id, data.Key)
        ]);
}

public record CheckListStarted(Guid Id)
{
    public static CheckListStarted From(StartCheckList cmd) => new(cmd.Id);
}