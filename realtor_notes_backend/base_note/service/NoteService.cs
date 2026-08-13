using realtor_notes_backend.base_note.consts;
using realtor_notes_backend.base_note.dto;
using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.service;

public abstract class NoteService<T> where T : Note
{
    public IEnumerable<byte> GetBaseDictionary(CreateNoteDTO createNoteDto)
    {
        return createNoteDto.Tags
            .Concat([createNoteDto.Status])
            .Concat(
                createNoteDto.Events.Select(vl => vl.Type)
            ).Concat(
                createNoteDto.Events.Select(vl => vl.State)
            );
    }

    public void SetUpBaseFromCreateDTO(T instanse, int userId, CreateNoteDTO createNoteDto, IDictionary<byte, NoteDictionary> dictionary)
    {
        instanse.Id = Guid.CreateVersion7();
        instanse.UserId = userId;
        instanse.IsDeleted = false;
        instanse.IsClosed = createNoteDto.IsClosed ?? false;
        instanse.Tags = createNoteDto.Tags.Select(vl => dictionary[vl]).ToList();
        instanse.Status = (StatusNoteDictionary) dictionary[createNoteDto.Status];
        instanse.GlobalComment = createNoteDto.GlobalComment;
        instanse.CreatedAt = DateTimeOffset.Now;
        
        List<NoteEvent> events = new List<NoteEvent>();
        events.Add(new NoteEvent()
        {
            Id = Guid.CreateVersion7(),
            NoteId = instanse.Id,
            Date = DateTimeOffset.Now,
            Type = SystemNoteDictionary.CREATED_EVENT_TYPE,
            State = SystemNoteDictionary.COMPLETED_STATE,
        });
        foreach (var dtoEvent in createNoteDto.Events)
        {
            events.Add(new NoteEvent()
            {
                Id = Guid.CreateVersion7(),
                NoteId = instanse.Id,
                Date = dtoEvent.Date,
                Type = dictionary[dtoEvent.Type],
                State = (StateNoteDictionary) dictionary[dtoEvent.State],
                Comment = dtoEvent.Comment,
            });
        }
        events.Sort();
        instanse.Events = events;
        instanse.LastEventDate = events.Last().Date;
    }
}