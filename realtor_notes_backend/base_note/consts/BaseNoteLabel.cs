using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.consts;

public class BaseNoteLabel : NoteLabel
{
    public static BaseNoteLabel ALL = new BaseNoteLabel(1, "*");
    public static BaseNoteLabel TAGS = new BaseNoteLabel(2, nameof(Note.Tags));
    public static BaseNoteLabel STATUS = new BaseNoteLabel(3, nameof(Note.Status));
    public static BaseNoteLabel EVENT_TYPE = new BaseNoteLabel(4, nameof(Note.Events)+"."+nameof(NoteEvent.Type));
    public static BaseNoteLabel EVENT_STATE = new BaseNoteLabel(5, nameof(Note.Events)+"."+nameof(NoteEvent.State));
    
    public BaseNoteLabel(byte id, string name) : base(id, name)
    {
    }
}