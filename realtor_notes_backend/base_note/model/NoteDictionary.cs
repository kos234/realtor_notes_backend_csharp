namespace realtor_notes_backend.base_note.model;

public class NoteDictionary
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public required NoteLabel NoteLabel { get; set; }
    public required NoteType NoteType { get; set; }
    public required string Value {get; set;}
}