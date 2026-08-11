using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using realtor_notes_backend.users.service;

namespace realtor_notes_backend.users;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly string _secretKey;

    public ConfigureJwtBearerOptions(IConfiguration configuration)
    {
        _secretKey = configuration["JwtOptions:SecretKey"] ?? string.Empty;
    }
    
    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            Configure(options);
        }
    }
}