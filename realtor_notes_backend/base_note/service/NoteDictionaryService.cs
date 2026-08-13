using ErrorOr;
using FluentValidation;
using realtor_notes_backend.base_note.dto;
using realtor_notes_backend.base_note.model;
using realtor_notes_backend.base_note.repository;
using realtor_notes_backend.utils;

namespace realtor_notes_backend.base_note.service;

public class NoteDictionaryService
{
    private readonly IValidator<CreateNoteDictionary> _validator;
    private readonly IValidator<UpdateNoteDictionary> _updateValidator;
    private readonly INoteDictionaryRepository _noteDictionaryRepository;

    public NoteDictionaryService(IValidator<CreateNoteDictionary> validator, IValidator<UpdateNoteDictionary> updateValidator, INoteDictionaryRepository noteDictionaryRepository)
    {
        _validator = validator;
        _updateValidator = updateValidator;
        this._noteDictionaryRepository = noteDictionaryRepository;
    }


    public async Task<IDictionary<long, NoteDictionary>> GetNoteDictionary(int userId, IEnumerable<int> ids)
    {
        return (await _noteDictionaryRepository.GetNoteDictionary(userId, ids)).ToDictionary(vl => vl.Id, vl => vl);
    }

    //DTO и использователь валидатор
    public async Task<ErrorOr<NoteDictionary>> CreateNoteDictionary(int userId, CreateNoteDictionary createNoteDictionary)
    {
        var res = await _validator.ValidateAsync(createNoteDictionary);
        if (!res.IsValid)
            return res.ToErrorOr<NoteDictionary>();
        
        var noteDic = new NoteDictionary()
        {
            UserId = userId,
            NoteLabel = NoteLabel.FromId(createNoteDictionary.NoteLabel),
            NoteType = NoteType.FromId(createNoteDictionary.NoteType),
            Value = createNoteDictionary.Value,
        };
        await _noteDictionaryRepository.Save(noteDic);
        return noteDic;
    }

    public async Task<ErrorOr<NoteDictionary>> UpdateNoteDictionary(int userId, UpdateNoteDictionary updateNoteDictionary)
    {
        var res = await _updateValidator.ValidateAsync(updateNoteDictionary);
        if (!res.IsValid)
            return res.ToErrorOr<NoteDictionary>();

        var noteDic = await _noteDictionaryRepository.GetById(updateNoteDictionary.Id);
        if (noteDic is null)
            return Error.NotFound(description: "Справочник не найден");
        if (noteDic.UserId != userId)
            return Error.Forbidden(description: "Нет прав на редактирование");

        noteDic.NoteLabel = NoteLabel.FromId(updateNoteDictionary.NoteLabel);
        noteDic.NoteType = NoteType.FromId(updateNoteDictionary.NoteType);
        noteDic.Value = updateNoteDictionary.Value;

        await _noteDictionaryRepository.Save(noteDic);
        return noteDic;
    }

    public async Task DeleteNoteDictionary(int userId, long id)
    {
        await _noteDictionaryRepository.DeleteById(id, userId);
    }
}