using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;


// COMMAND
public record StartCheckList([property: Identity]Guid Id);

// COMMAND HANDLER/DECIDER
public static class StartCheckListHandler
{
    [WolverinePost("/api/checklist")]
    [AggregateHandler(AggregateType = typeof(CheckList))]
    public static (IResult, Events) Handle(StartCheckList cmd, LinkGenerator linkGenerator)
        => (Results.Accepted(linkGenerator.GetPathByPage("/Todo/Index", values: new{cmd.Id})), [CheckListStarted.From(cmd)]);
}

public record CheckListStarted(Guid Id)
{
    public static CheckListStarted From(StartCheckList cmd) => new(cmd.Id);
}