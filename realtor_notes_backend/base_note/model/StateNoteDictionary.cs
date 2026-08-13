using realtor_notes_backend.base_note.consts;

namespace realtor_notes_backend.base_note.model;

public class StateNoteDictionary : NoteDictionary
{
    public StateMood Mood { get; set; }
}