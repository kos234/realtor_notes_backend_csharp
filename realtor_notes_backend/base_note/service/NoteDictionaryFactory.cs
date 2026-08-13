using realtor_notes_backend.base_note.dto;
using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.service;

public static class NoteDictionaryFactory
{
    public delegate Func<CreateNoteDictionary, NDFactory, NoteDictionary> NDFactory(CreateNoteDictionary createNoteDictionary);

    public static IList<NDFactory> factories = new List<NDFactory>();

    public static void RegisterFactory(NDFactory factory)
    {
        factories.Add(factory);
    }

    public static NoteDictionary CreateNoteDictionary(CreateNoteDictionary createDTO)
    {
        
    }
}