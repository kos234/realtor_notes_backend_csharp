using ErrorOr;
using FluentValidation;
using realtor_notes_backend.users.consts;
using realtor_notes_backend.users.dtos;
using realtor_notes_backend.users.model;
using realtor_notes_backend.utils;

namespace realtor_notes_backend.users.service;

public interface IUserAuthService
{
    Task<ErrorOr<RegisterResponseDTO>> CreateNewUserAsync(
        NewUserDto dto,
        ClientPlatformInfo platformInfo);

    Task<ErrorOr<LoginResponseDTO>> LoginUserAsync(UserLoginDto dto, ClientPlatformInfo platformInfo);
    
    Task<AuthActions[]> GetAuthActions(int userId, long sessionId);
}

public class UserAuthService(
    IUserService userService,
    IUserSubscriptionService userSubscriptionService,
    IRecaptchaValidator recaptchaValidator,
    ISessionService sessionService,
    ITwoFaService twoFaService,
    IValidator<NewUserDto> newUserValidator,
    IValidator<UserLoginDto> loginUserValidator) : IUserAuthService
{
    public async Task<ErrorOr<RegisterResponseDTO>> CreateNewUserAsync(
        NewUserDto dto,
        ClientPlatformInfo platformInfo)
    {
        var validationResult = await newUserValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return validationResult.ToErrorOr<RegisterResponseDTO>();
        
        if(!await recaptchaValidator.IsValidAsync(dto.Recaptcha, dto.RecaptchaVersion))
            return Error.Validation("recaptcha", "Вы не прошли проверку на человека");
        
        if (await userService.IsUnavailableLogin(dto.Login))
            return Error.Validation("login", "Логин уже используется");

        if (await userService.IsUnavailableEmail(dto.Email))
            return Error.Validation("email", "Почта уже используется");
        
        var user = await userService.CreateNewUser(dto.Login, dto.Email, BCrypt.Net.BCrypt.HashPassword(dto.Password));
        await userSubscriptionService.ActivateTrialSubscription(user.Id);
        var session = await sessionService.CreateNewSession(user.Id, dto.TrustDevice, platformInfo);

        var authActions = new List<AuthActions> { AuthActions.EmailConfirm };
        string? twoFaUrl = null;
        if (dto.IsTwoFa)
        {
            twoFaUrl = (await twoFaService.CreateSecretKey(user.Id, user.Login)).ConnectionUrl;
            authActions.Add(AuthActions.TwoFaConfirm);
        }

        return new RegisterResponseDTO(user, session, false, twoFaUrl, authActions.ToArray());
    }

    public async Task<ErrorOr<LoginResponseDTO>> LoginUserAsync(UserLoginDto dto, ClientPlatformInfo platformInfo)
    {
        var validationResult = await loginUserValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return validationResult.ToErrorOr<LoginResponseDTO>();
        
        if(!await recaptchaValidator.IsValidAsync(dto.Recaptcha, dto.RecaptchaVersion))
            return Error.Validation("recaptcha", "Вы не прошли проверку на человека");

        bool isEmail = dto.Username.Contains('@');
        User? user = await (isEmail ? userService.GetUserByEmail(dto.Username) : userService.GetUserByLogin(dto.Username));
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Error.Unauthorized(description:"Неверные данные для входа или пароль");
        
        AuthActions? nextAction = null;
        if (!user.IsConfirmEmail)
        {
            nextAction = AuthActions.EmailConfirm;
        }
        
        Session session;
        Session? existingSession = dto.TrustDevice 
            ? await sessionService.GetSessionForDevice(user.Id, platformInfo.DeviceId) 
            : null;
        if (existingSession != null)
            session = await sessionService.CreateNewSessionFromSession(existingSession, platformInfo);
        else
        {
            session = await sessionService.CreateNewSession(user.Id, dto.TrustDevice, platformInfo);
            if (nextAction == null)
            {
                if (user.TwoFaSecret != null)
                    nextAction = AuthActions.TwoFaLogin;
                else
                {
                    nextAction = AuthActions.EmailLogin;
                }
            }
        }
        
        return new LoginResponseDTO(user, session, nextAction == null, nextAction);
    }

    public async Task<AuthActions[]> GetAuthActions(int userId, long sessionId)
    {
        var user = await userService.GetUserById(userId);
        if (user == null)
            return [];
        
        List<AuthActions> actions = new List<AuthActions>();
        if (user.IsConfirmEmail)
        {
            var session = await sessionService.GetSessionById(userId, sessionId);
            if(session != null && !session.IsEntered)
                actions.Add(user.TwoFaSecret == null ? AuthActions.EmailLogin : AuthActions.TwoFaLogin);
        }else
            actions.Add(AuthActions.EmailConfirm);
        
        var key = await twoFaService.GetSecretKey(userId);
        if(key != null)
            actions.Add(AuthActions.TwoFaConfirm);
        return actions.ToArray();
    }
}