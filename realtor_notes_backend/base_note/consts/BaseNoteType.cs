using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.consts;

public class BaseNoteType : NoteType
{
    public static BaseNoteType ALL = new BaseNoteType(1, "*");

    public BaseNoteType(byte id, string name) : base(id, name)
    {
    }
}