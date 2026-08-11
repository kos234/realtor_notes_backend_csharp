namespace realtor_notes_backend.base_note.model;

public abstract class NoteType
{
    public byte Id { get; }
    public string Name { get; }

    protected NoteType(byte id, string name)
    {
        Id = id;
        Name = name;
        Register(this);
    }

    // Реестр для хранения типов
    private static readonly Dictionary<byte, NoteType> _registry = new();

    public static void Register(NoteType type)
    {
        if (!_registry.TryAdd(type.Id, type))
        {
            throw new InvalidOperationException($"Тип заметки с ID {type.Id} уже зарегистрирован.");
        }
    }

    public static NoteType FromId(byte id) => _registry[id];
    public static bool TryFromId(byte id, out NoteType? var) => _registry.TryGetValue(id, out var);

}