using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.repository;

public interface INoteDictionaryRepository
{
    Task<ICollection<NoteDictionary>> GetNoteDictionary(IEnumerable<int> ids);
    Task Save(NoteDictionary noteDic);
}