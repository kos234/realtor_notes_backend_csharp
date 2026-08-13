using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.consts;

public static class SystemNoteDictionary
{
    public static readonly NoteDictionary CREATED_EVENT_TYPE =
        new NoteDictionary()
        {
            Id = 1,
            UserId = -2,
            NoteLabel = BaseNoteLabel.EVENT_TYPE, 
            NoteType = BaseNoteType.ALL,
            Value = "Создано"
        };
    
    public static readonly NoteDictionary CLOSED_EVENT_TYPE =
        new NoteDictionary()
        {
            Id = 2,
            UserId = -2,
            NoteLabel = BaseNoteLabel.EVENT_TYPE, 
            NoteType = BaseNoteType.ALL,
            Value = "Закрыто"
        };
    
    public static readonly NoteDictionary DELETED_EVENT_TYPE =
        new NoteDictionary()
        {
            Id = 3,
            UserId = -2,
            NoteLabel = BaseNoteLabel.EVENT_TYPE, 
            NoteType = BaseNoteType.ALL,
            Value = "Удалено"
        };

    public static readonly StateNoteDictionary COMPLETED_STATE = new StateNoteDictionary()
    {
        Id = 1,
        UserId = -2,
        NoteLabel = BaseNoteLabel.EVENT_STATE,
        NoteType = BaseNoteType.ALL,
        Value = "Выполнено",
        Mood = StateMood.POSITIVE,
    };

    public static readonly StateNoteDictionary WAIT_STATE = new StateNoteDictionary()
    {
        Id = 2,
        UserId = -2,
        NoteLabel = BaseNoteLabel.EVENT_STATE,
        NoteType = BaseNoteType.ALL,
        Value = "В процессе",
        Mood = StateMood.NEUTRAL,
    };

    public static readonly StateNoteDictionary FAIL_STATE = new StateNoteDictionary()
    {
        Id = 3,
        UserId = -2,
        NoteLabel = BaseNoteLabel.EVENT_STATE,
        NoteType = BaseNoteType.ALL,
        Value = "Не выполнено",
        Mood = StateMood.NEGATIVE,
    };
}