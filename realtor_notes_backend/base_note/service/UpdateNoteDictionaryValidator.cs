using FluentValidation;
using realtor_notes_backend.base_note.dto;
using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.service;

public class UpdateNoteDictionaryValidator : AbstractValidator<UpdateNoteDictionary>
{
    public UpdateNoteDictionaryValidator()
    {
        RuleFor(x => x.NoteLabel)
            .Must((dictionary, i, arg3) => NoteLabel.TryFromId(i, out _)).WithMessage("Не опознан идентификатор поля");
        RuleFor(x => x.NoteType)
            .Must((dictionary, i, arg3) => NoteType.TryFromId(i, out _)).WithMessage("Не опознан тип заметки");
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Значение не может быть пустым")
            .Length(1, 32).WithMessage("Длина должна быть до 32 символов");
    }
}
