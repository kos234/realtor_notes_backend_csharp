using System.Text.RegularExpressions;
using FluentValidation;
using realtor_notes_backend.users.dtos;

namespace realtor_notes_backend.users.validators;

public class NewUserDtoValidator : AbstractValidator<NewUserDto>
{
    public NewUserDtoValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Вы не указали логин")
            .Length(3, 32).WithMessage("Длина логина должна быть от 3 до 32 символов")
            .Matches(@"^[a-zA-Za-яА-Я0-9_\-+=]+$", RegexOptions.IgnoreCase).WithMessage("Логин не прошел валидацию");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Вы не указали почту")
            .Length(3, 32).WithMessage("Длина почты должна быть от 3 до 32 символов")
            .EmailAddress().WithMessage("Почта не прошла валидацию");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Вы не указали пароль")
            .Length(3, 32).WithMessage("Длина пароля должна быть от 3 до 32 символов");

        RuleFor(x => x.Recaptcha)
            .NotEmpty().WithMessage("Вы не указали код капчи");
    }
}