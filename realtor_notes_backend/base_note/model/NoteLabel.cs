namespace realtor_notes_backend.base_note.model;

public abstract class NoteLabel
{
    public byte Id { get; }
    public string Name { get; }

    protected NoteLabel(byte id, string name)
    {
        Id = id;
        Name = name;
        Register(this);
    }

    private static readonly Dictionary<byte, NoteLabel> _registry = new();

    public static void Register(NoteLabel label)
    {
        if (!_registry.TryAdd(label.Id, label))
            throw new InvalidOperationException($"Ярлык с ID {label.Id} уже зарегистрирован.");
    }

    public static NoteLabel FromId(byte id) => _registry[id];
    public static bool TryFromId(byte id, out NoteLabel? var) => _registry.TryGetValue(id, out var);
}