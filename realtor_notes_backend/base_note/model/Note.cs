namespace realtor_notes_backend.base_note.model;

public abstract class Note
{
    public Guid Id { get; set; }    //v7
    public int UserId { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsClosed { get; set; }

    public List<NoteDictionary> Tags { get; set; } = new();
    public required StatusNoteDictionary Status { get; set; }
    public string? GlobalComment { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastEventDate { get; set; }
    public ICollection<NoteEvent> Events { get; set; } = new List<NoteEvent>();
}