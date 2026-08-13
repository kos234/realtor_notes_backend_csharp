using realtor_notes_backend.base_note.consts;
using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.dto;

public record CreateNoteDictionary(byte NoteLabel, byte NoteType, string Value, StateMood? StateMoodForState, bool? IsCloseNoteForStatus)
{
    
}