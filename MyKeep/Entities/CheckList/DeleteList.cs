using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record DeleteList([property: Identity] Guid Id);

public static class DeleteListHandler
{
    [WolverineDelete("/api/checklist")]
    [AggregateHandler]
    public static (IResult, Events) Handle(DeleteList cmd, CheckList _)
    {
        return (Results.Accepted(), [new ListDeleted(cmd.Id)]);
    }
}

public record ListDeleted(Guid Id);