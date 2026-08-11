namespace realtor_notes_backend.base_note.model;

public class EventSchedule
{
    public long Id { get; set; }
    public Guid NoteId { get; set; }
    public required NoteDictionary EventType { get; set; } 
    
    public DateTimeOffset StartDate { get; set; }
    public bool IsRepeatAsDay { get; set; }
    public int RepeatCount { get; set; }
    
    public DateTimeOffset? EndDate { get; set; } // Если null, повторяется бесконечно
    public DateTimeOffset LastMaterializedDate { get; set; } // До какой даты мы уже сгенерировали события
}