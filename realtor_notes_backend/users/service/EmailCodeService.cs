using System.Runtime.InteropServices.JavaScript;
using realtor_notes_backend.users.repository;
using ErrorOr;
using realtor_notes_backend.users.consts;

namespace realtor_notes_backend.users.service;

public interface IEmailCodeService
{
    public Task<ErrorOr<DateTimeOffset>> SendEmailConfirmCode(int userId, AuthActions[] userActions);
    public Task<ErrorOr<DateTimeOffset>> SendLoginCode(int userId, AuthActions[] userActions);
    public Task<ErrorOr<bool>> CheckEmailConfirmCode(int userId, AuthActions[] userActions, string code);
    public Task<ErrorOr<bool>> CheckLoginCode(int userId, AuthActions[] userActions, string code);
}

public class EmailCodeService(
    ICodeRepository codeRepository,
    IUserService userService,
    ILogger<EmailCodeService> logger) : IEmailCodeService
{
    private const int DURATION = 60 * 30;
    private const int COOL_DOWN = 60;

    public async Task<ErrorOr<DateTimeOffset>> SendEmailConfirmCode(int userId, AuthActions[] userActions)
    {
        if (!userActions.Contains(AuthActions.EmailConfirm))
            return Error.Validation(description: "В рамках сессии нет необходимости подтверждать почту");

        return await SendCode(
            userId,
            nameof(SendEmailConfirmCode),
            "Подтверждение почты",
            "Вы зарегистрировались на сервисе Realtor Notes, введите этот код {0} для подтверждения адреса электронной почты"
        );
    }

    public async Task<ErrorOr<DateTimeOffset>> SendLoginCode(int userId, AuthActions[] userActions)
    {
        if (!userActions.Contains(AuthActions.EmailLogin))
            return Error.Validation(description: "В рамках сессии нет необходимости подтверждать вход через почту");

        return await SendCode(
            userId,
            nameof(SendEmailConfirmCode),
            "Подтверждение входа в аккаунт",
            "Вы пытаетесь войти в Realtor Notes, введите этот код {0} для подтверждения входа"
        );
    }

    public async Task<ErrorOr<bool>> CheckEmailConfirmCode(int userId, AuthActions[] userActions, string code)
    {
        if (!userActions.Contains(AuthActions.EmailConfirm))
            return Error.Validation(description: "В рамках сессии нет необходимости подтверждать почту");

        var res = await CheckCode(userId, nameof(SendEmailConfirmCode), code);
        if(!res)
            return Error.Validation(description: "Неверный код");
        
        await userService.SetEmailChecked(userId);
        return true;
    }

    public async Task<ErrorOr<bool>> CheckLoginCode(int userId, AuthActions[] userActions, string code)
    {
        if (!userActions.Contains(AuthActions.EmailLogin))
            return Error.Validation(description: "В рамках сессии нет необходимости подтверждать вход через почту");

        var res = await CheckCode(userId, nameof(SendEmailConfirmCode), code);
        if(!res)
            return Error.Validation(description: "Неверный код");
        
        return true;
    }

    private async Task<ErrorOr<DateTimeOffset>> SendCode(int userId, string label, string header, string message)
    {
        var codeDto = await codeRepository.GetCode(userId, label);
        if (codeDto != null)
        {
            int offset = (int)(COOL_DOWN - DateTimeOffset.Now.Subtract(codeDto.CreateAt).TotalSeconds);
            if (offset > 0)
                return Error.Validation(description: $"До отправки кода ещё {offset} с.");
        }


        string code = codeDto == null ? GetRandomCode() : codeDto.Code;
        await codeRepository.SaveCode(userId, label, code, DURATION);
        await SendMessage(userId, string.Format(header, code),
            string.Format(message, code));
        return DateTimeOffset.Now.AddSeconds(COOL_DOWN);
    }

    public Task<bool> CheckLoginCode(int userId, string code)
    {
        return CheckCode(userId, nameof(SendEmailConfirmCode), code);
    }

    private async Task<bool> CheckCode(int userId, string label, string code)
    {
        var codeDto = await codeRepository.GetCode(userId, label);
        if (codeDto == null)
            return false;
        return codeDto.Code == code;
    }

    private string GetRandomCode() => Random.Shared.Next(100000, 999999).ToString();

    private async Task SendMessage(int userId, string header, string message)
    {
        string? email = (await userService.GetUserById(userId))?.Email ?? null;
        if (email == null)
            return;

        logger.Log(LogLevel.Warning, $"Sending email: to {email}, header: {header}, message: {message}");
    }
}