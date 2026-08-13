using FluentValidation;
using realtor_notes_backend.base_note.dto;
using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.base_note.service;

public abstract class CreateNoteDTOValidator<T> : AbstractValidator<T> where T : CreateNoteDTO
{
    protected CreateNoteDTOValidator()
    {
        RuleFor(x => x.Tags)
            .Must((model, value, context) =>
        {
            if (value == null || !value.Any()) return true;
            
            IDictionary<byte, NoteDictionary> dictionary = GetDictionaryFromContext(context);
            var missingIds = value.Where(id => !dictionary.ContainsKey(id)).ToList();
            if (missingIds.Any())
            {
                context.MessageFormatter.AppendArgument("MissingIds", string.Join(", ", missingIds));
                return false;
            }

            return true;
        }).WithMessage("Неопознаны следующие айди тегов: {MissingIds}");
        
        RuleFor(x => x.Status)
            .Must((model, value, context) => GetDictionaryFromContext(context).TryGetValue(value, out var dictionary) && dictionary is StatusNoteDictionary)
            .WithMessage("Неопознан айди статуса заметки");
        
        RuleFor(x => x.GlobalComment)
            .MaximumLength(2000)
            .WithMessage("Глобальный комментарий превышает 2000 символов");
        
        RuleForEach(x => x.Events)
            .ChildRules(events => 
            {
                events.RuleFor(e => e.Type)
                    .Must((model, value, context) => GetDictionaryFromContext(context).ContainsKey(value))
                    .WithMessage("В событии с индексом {CollectionIndex} не опознан айди типа");
                
                events.RuleFor(e => e.State)
                    .Must((model, value, context) => GetDictionaryFromContext(context).TryGetValue(value, out var dictionary) && dictionary is StateNoteDictionary)
                    .WithMessage("В событии с индексом {CollectionIndex} не опознан айди состояния");
                
                events.RuleFor(e => e.Date)
                    .NotEmpty()
                    .WithMessage("В событии с индексом {CollectionIndex} не указана дата");
                
                events.RuleFor(e => e.Comment)
                    .MaximumLength(1000)
                    .WithMessage("В событии с индексом {CollectionIndex} комментарий превышает 1000 символов");

                events.When(e => e.Shelter != null, () =>
                {
                    events.RuleFor(e => e.Shelter!)
                        .ChildRules(s =>
                        {
                            s.RuleFor(x => x.StartDate)
                                .NotEmpty()
                                .WithMessage(
                                    "В событии с индексом {CollectionIndex} у планировщика не указана дата начала");
                            s.RuleFor(x => x.RepeatCount)
                                .GreaterThanOrEqualTo(0)
                                .WithMessage(
                                    "В событии с индексом {CollectionIndex} у планировщика не может быть отрицательный период");
                        });
                });
            });
    }

    protected IDictionary<byte, NoteDictionary> GetDictionaryFromContext(IValidationContext context) =>
        (IDictionary<byte, NoteDictionary>)context.RootContextData["Dictionary"];
}