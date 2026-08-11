using System.Text.Json;
using System.Text.Json.Serialization;

namespace realtor_notes_backend.users.service;

public interface IRecaptchaValidator
{
    public Task<bool> IsValidAsync(string? recaptcha, int recaptchaVersion);
}

public class RecaptchaValidator : IRecaptchaValidator
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RecaptchaValidator> _logger;
    private readonly string _captchaV2Key;
    private readonly string _captchaV3Key;
    private readonly bool _isDevelopment;

    public RecaptchaValidator(
        HttpClient httpClient, 
        IConfiguration configuration, 
        ILogger<RecaptchaValidator> logger,
        IHostEnvironment environment)
    {
        _httpClient = httpClient;
        _logger = logger;
        _isDevelopment = environment.IsDevelopment();
        
        
        _captchaV2Key = configuration["Captcha:V2Key"] ?? string.Empty;
        _captchaV3Key = configuration["Captcha:V3Key"] ?? string.Empty;
    }

    public async Task<bool> IsValidAsync(string? recaptcha, int recaptchaVersion)
    {
        if (_isDevelopment && recaptcha == "backdoor")
        {
            return true;
        }

        if (string.IsNullOrEmpty(recaptcha))
        {
            return false;
        }

        string secretKey = recaptchaVersion == 2 ? _captchaV2Key : _captchaV3Key;
        string verificationUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptcha}";

        try
        {
            var httpResponse = await _httpClient.GetAsync(verificationUrl);
            if (!httpResponse.IsSuccessStatusCode)
            {
                return false;
            }

            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<RecaptchaResponse>(jsonString);

            if (response == null || !response.Success)
            {
                return false;
            }

            if (recaptchaVersion == 3)
            {
                return response.Score >= 0.5;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке reCAPTCHA");
            return false;
        }
    }

    private class RecaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}