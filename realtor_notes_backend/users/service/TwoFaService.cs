using ErrorOr;
using OtpNet;
using realtor_notes_backend.users.consts;
using realtor_notes_backend.users.dtos;
using realtor_notes_backend.users.model;
using realtor_notes_backend.users.repository;

namespace realtor_notes_backend.users.service;

public interface ITwoFaService
{
    public Task<TwoFaDto> CreateSecretKey(int userId, string login);
    public Task<TwoFaDto?> GetSecretKey(int userId);
    public Task CancelTwoFaSetup(int userId);
    public Task<ErrorOr<bool>> CheckConfirmKeyCode(int userId, string code, AuthActions[] authActions);
    public Task<ErrorOr<bool>> CheckLoginCode(int userId, string code, AuthActions[] authActions);
}

public class TwoFaService (
    ITwoFaRepository twoFaRepository,
    IUserService userService
    ) : ITwoFaService
{
    public const int DURATION = 60 * 15;
    
    public async Task<TwoFaDto> CreateSecretKey(int userId, string login)
    {
        // 1. Генерация случайного секретного ключа (20 байт = 160 бит по стандарту)
        byte[] secretBytes = KeyGeneration.GenerateRandomKey(20);
        string secretKey = Base32Encoding.ToString(secretBytes);

        // 2. Формирование URI для приложения-аутентификатора
        string issuer = "Realtor Notes";
        string otpauthUrl = $"otpauth://totp/{issuer}:{login}?secret={secretKey}&issuer={issuer}&digits=6";

        var dto = new TwoFaDto(secretKey, otpauthUrl);
        await twoFaRepository.SaveTwoFaKey(userId, dto, DURATION);
        
        return dto;
    }

    public Task<TwoFaDto?> GetSecretKey(int userId)
    {
        return twoFaRepository.GetTwoFaKey(userId);
    }

    public Task CancelTwoFaSetup(int userId)
    {
        return twoFaRepository.DeleteTwoFaKey(userId);
    }

    public async Task<ErrorOr<bool>> CheckConfirmKeyCode(int userId, string code, AuthActions[] authActions)
    {
        if (!authActions.Contains(AuthActions.TwoFaConfirm))
            return Error.Validation(description: "В рамках сессии нет необходимости подтверждать двухфакторную аутентификацию");
        
        string? key = (await twoFaRepository.GetTwoFaKey(userId))?.SecretKey;
        if(key == null)
            return Error.Validation(description: "Вышло время подтверждения");
        if(!CheckCode(key, code))
            return Error.Validation(description: "Неверный код");
        
        await userService.SetTwoFaSecret(userId, key);
        await twoFaRepository.DeleteTwoFaKey(userId); 
        return true;    
    }

    public async Task<ErrorOr<bool>> CheckLoginCode(int userId, string code, AuthActions[] authActions)
    {
        if (!authActions.Contains(AuthActions.TwoFaLogin))
            return Error.Validation(description: "В рамках сессии нет необходимости подтверждать вход двухфакторной аутентификацией");
        
        User? user = await userService.GetUserById(userId);
        if (user == null || user.TwoFaSecret == null)
            return Error.Validation(description: "Не установлена двухфакторная аутентификация");
        
        var res = CheckCode(user.TwoFaSecret, code);
        if(!res)
            return Error.Validation(description: "Неверный код");
        return true;
    }

    private bool CheckCode(string key, string code)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(code) || code.Length != 6)
            return false;

        try
        {
            byte[] secretBytes = Base32Encoding.ToBytes(key);
            var totp = new Totp(secretBytes);

            bool isValid = totp.VerifyTotp(
                code, 
                out long timeStepMatched, 
                new VerificationWindow(previous: 1, future: 1)
            );

            return isValid;
        }
        catch
        {
            return false;
        }
    }
}