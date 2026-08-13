namespace realtor_notes_backend.base_note.dto;

public record ShelterEventDTO(
    DateTimeOffset StartDate, 
    DateTimeOffset? EndDate,
    bool IsRepeatAsDay,
    int RepeatCount
    )
{
    
}