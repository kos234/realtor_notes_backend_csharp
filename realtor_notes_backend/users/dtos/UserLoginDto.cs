namespace realtor_notes_backend.users.dtos;

public record UserLoginDto(
    string Username,
    string Password,
    string Recaptcha,
    int RecaptchaVersion = 3,
    bool TrustDevice = false
);