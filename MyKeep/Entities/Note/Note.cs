namespace MyKeep.Entities.Note;

public record Note(Guid Id, string? Title = null, string? Text = null)
{
    public static Note Create(Guid id) => new(id);
}