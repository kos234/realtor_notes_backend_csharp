namespace realtor_notes_backend.users.dtos;

public record NewUserDto(
    string Login,
    string Email,
    string Password,
    string Recaptcha,
    int RecaptchaVersion = 3,
    bool IsTwoFa = false,
    bool TrustDevice = false
);