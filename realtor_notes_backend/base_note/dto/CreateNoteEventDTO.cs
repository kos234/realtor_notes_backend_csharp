namespace realtor_notes_backend.base_note.dto;

public record CreateNoteEventDTO(
    byte Type, 
    byte State, 
    DateTimeOffset Date, 
    string? Comment,
    ShelterEventDTO? Shelter
    );