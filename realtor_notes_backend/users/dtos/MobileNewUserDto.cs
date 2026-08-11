namespace realtor_notes_backend.users.dtos;

public record MobileNewUserDto(
    string Login,
    string Email,
    string Password,
    string Recaptcha,
    int RecaptchaVersion = 3,
    bool IsTwoFa = false,
    bool TrustDevice = false,
    string? DeviceId = null
) : NewUserDto(Login, Email, Password, Recaptcha, RecaptchaVersion, IsTwoFa, TrustDevice);