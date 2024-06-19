using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

// COMMAND
public record StartCheckList([property: Identity]Guid Id, string StartingColor);

// COMMAND HANDLER/DECIDER
public static class StartCheckListHandler
{    
    [WolverinePost("/api/checklist")]
    [AggregateHandler(AggregateType = typeof(CheckList))]
    public static (IResult, Events) Handle(StartCheckList cmd, LinkGenerator linkGenerator)
        => (Results.Accepted(linkGenerator.GetPathByPage("/Todo/Index", values: new{cmd.Id})), [
            CheckListStartedWithColor.From(cmd),
            new CheckListItemAdded(cmd.Id, KeyGenerator.GetUniqueKey(8))
        ]);
}

public record CheckListStarted(Guid Id)
{
    public static CheckListStarted From(StartCheckList cmd) => new(cmd.Id);
}

public record CheckListStartedWithColor(Guid Id, string Color)
{
    public static CheckListStartedWithColor From(StartCheckList cmd) => new(cmd.Id, cmd.StartingColor);
}