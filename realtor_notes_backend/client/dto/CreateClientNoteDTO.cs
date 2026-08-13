using realtor_notes_backend.base_note.dto;

namespace realtor_notes_backend.client.dto;

public record CreateClientNoteDTO(
    List<byte> Tags,
    byte Status,
    string? GlobalComment,
    bool? IsClosed,
    IList<CreateNoteEventDTO> Events,
    string FullName,
    IList<string> Phones)
    : CreateNoteDTO(Tags, Status, GlobalComment, IsClosed, Events)
{
}