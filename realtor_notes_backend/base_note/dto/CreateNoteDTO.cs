namespace realtor_notes_backend.base_note.dto;

public abstract record CreateNoteDTO(
    List<byte> Tags, 
    byte Status, 
    string? GlobalComment, 
    bool? IsClosed,
    IList<CreateNoteEventDTO> Events)
{
    
}