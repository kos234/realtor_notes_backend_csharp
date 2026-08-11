using System.Text.RegularExpressions;
using FluentValidation;
using realtor_notes_backend.users.dtos;

namespace realtor_notes_backend.users.validators;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Вы не указали логин или почту")
            .Length(3, 32).WithMessage("Длина логина или почты должна быть от 3 до 32 символов")
            .Matches(@"^[a-zA-Za-яА-Я0-9_\-+=]+$", RegexOptions.IgnoreCase).WithMessage("Логин или почта не прошли валидацию");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Вы не указали пароль")
            .Length(3, 32).WithMessage("Длина пароля должна быть от 3 до 32 символов");

        RuleFor(x => x.Recaptcha)
            .NotEmpty().WithMessage("Вы не указали код капчи");
    }
}