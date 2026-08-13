using realtor_notes_backend.base_note.service;
using realtor_notes_backend.client.dto;
using realtor_notes_backend.client.model;

namespace realtor_notes_backend.client.service;

public class ClientService : NoteService<ClientNote>
{
    public Task<ClientNote> CreateClientNote(int userId, CreateClientNoteDTO clientNoteDto)
    {
        
    }
}