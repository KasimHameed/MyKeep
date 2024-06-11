using JasperFx.Core;
using Marten.Schema;
using Wolverine.Http;
using Wolverine.Marten;

namespace MyKeep.Entities.Note;


// COMMAND
public record AddNote([property: Identity]Guid Id);

// COMMAND HANDLER/DECIDER
public static class AddNoteHandler
{
    [WolverinePost("/api/note")]
    [AggregateHandler(AggregateType = typeof(Note))]
    public static (IResult, Events) Handle(AddNote cmd, LinkGenerator linkGenerator)
        => (Results.Accepted(linkGenerator.GetPathByPage("/Note/Index"), new { cmd.Id }), [NoteAdded.From(cmd)]);
}

// EVENT
public record NoteAdded(Guid Id)
{
    public static NoteAdded From(AddNote note) => new(note.Id);
}