namespace realtor_notes_backend.users.dtos;

public record MobileUserLoginDto(
    string Username,
    string Password,
    string Recaptcha,
    int RecaptchaVersion = 3,
    bool TrustDevice = false,
    string? DeviceId = null
) : UserLoginDto(Username, Password, Recaptcha, RecaptchaVersion, TrustDevice);