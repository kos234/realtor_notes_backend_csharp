using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.consts;

public class BaseNoteLabel : NoteLabel
{
    public static BaseNoteLabel TAGS = new BaseNoteLabel(0, nameof(Note.Tags));
    public static BaseNoteLabel STATUS = new BaseNoteLabel(0, nameof(Note.Status));
    public static BaseNoteLabel EVENT_TYPE = new BaseNoteLabel(0, nameof(Note.Events)+"."+nameof(NoteEvent.Type));
    public static BaseNoteLabel EVENT_STATE = new BaseNoteLabel(0, nameof(Note.Events)+"."+nameof(NoteEvent.State));
    
    public BaseNoteLabel(byte id, string name) : base(id, name)
    {
    }
}