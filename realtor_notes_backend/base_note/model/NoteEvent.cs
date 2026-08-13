namespace realtor_notes_backend.base_note.model;

public class NoteEvent : IComparable<NoteEvent>
{
    public Guid Id { get; set; }
    public Guid NoteId { get; set; }
    public DateTimeOffset Date { get; set; }
    
    public required NoteDictionary Type { get; set; }
    public required StateNoteDictionary State { get; set; }
    public string? Comment { get; set; }
    
    public int CompareTo(NoteEvent? other)
    {
        if(other == null)
            return 1;
        return Date.CompareTo(other.Date);
    }
}