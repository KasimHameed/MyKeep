using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

public record RenameCheckList([property: Identity] Guid Id, string Title);

public static class RenameCheckListHandler
{
    [WolverinePatch("/api/checklist/title")]
    [AggregateHandler]
    public static (IResult, Events) Handle(RenameCheckList cmd, CheckList entity)
    {
        if (entity.Title == cmd.Title) return (Results.Ok(), []);
        return (Results.Accepted(), [CheckListRenamed.From(cmd)]);
    }
}

public record CheckListRenamed(Guid Id, string Title)
{
    public static CheckListRenamed From(RenameCheckList cmd) => new(cmd.Id, cmd.Title);
}