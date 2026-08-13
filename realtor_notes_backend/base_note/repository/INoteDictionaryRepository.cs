using ErrorOr;
using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.repository;

public interface INoteDictionaryRepository
{
    Task<ICollection<NoteDictionary>> GetNoteDictionary(int userId, IEnumerable<int> ids);
    Task Save(NoteDictionary noteDic);
    Task<NoteDictionary> GetById(long id);
    Task DeleteById(long id, int userId);
}