namespace realtor_notes_backend.base_note.model;

public class NoteEvent
{
    public Guid Id { get; set; }
    public Guid NoteId { get; set; }
    public DateTimeOffset Date { get; set; }
    
    public required NoteDictionary Type { get; set; }
    public required NoteDictionary State { get; set; }
    public string? Comment { get; set; }
}