using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.TodoList;

// COMMAND
public record UpdateCheckListColor([property: Identity] Guid Id, string NewColor);

// COMMAND HANDLER/DECIDER
public static class UpdateCheckListColorHandler
{
    [WolverinePatch("/api/checklist/color")]
    [AggregateHandler]
    public static (IResult, Events) Handle(UpdateCheckListColor cmd, CheckList entity)
    {
        return cmd.NewColor != entity.Color
            ? (Results.Accepted(), [CheckListColorUpdated.From(cmd)])
            : (Results.Ok(), []);
    }
}

// EVENTS
public record CheckListColorUpdated(Guid Id, string Color)
{
    public static CheckListColorUpdated From(UpdateCheckListColor cmd) => new (cmd.Id, cmd.NewColor);
}