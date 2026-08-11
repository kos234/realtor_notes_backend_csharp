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
    private readonly INoteDictionaryRepository _noteDictionaryRepository;

    public NoteDictionaryService(IValidator<CreateNoteDictionary> validator, INoteDictionaryRepository noteDictionaryRepository)
    {
        _validator = validator;
        this._noteDictionaryRepository = noteDictionaryRepository;
    }

    public async Task<IDictionary<long, NoteDictionary>> GetNoteDictionary(IEnumerable<int> ids)
    {
        return (await _noteDictionaryRepository.GetNoteDictionary(ids)).ToDictionary(vl => vl.Id, vl => vl);
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
    
    
}