using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using realtor_notes_backend.users.dtos;
using realtor_notes_backend.users.endpoints;
using realtor_notes_backend.users.repository;
using realtor_notes_backend.users.service;
using realtor_notes_backend.users.validators;

namespace realtor_notes_backend.users;

public static class UserExtension
{
    public static void UseUserModule(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEmailCodeService, EmailCodeService>();
        builder.Services.AddScoped<IJWTTokenProvider, JWTTokenProvider>();
        builder.Services.AddHttpClient<IRecaptchaValidator, RecaptchaValidator>();
        builder.Services.AddScoped<ISessionService, SessionService>();
        builder.Services.AddScoped<ITwoFaService, TwoFaService>();
        builder.Services.AddScoped<IUserAuthService, UserAuthService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
        
        builder.Services.AddScoped<IValidator<NewUserDto>, NewUserDtoValidator>();
        builder.Services.AddScoped<IValidator<UserLoginDto>, UserLoginDtoValidator>();

        builder.Services.AddScoped<ICodeRepository, MemoryCodeRepository>();
        builder.Services.AddScoped<ISessionRepository, EFSessionRepository>();
        builder.Services.AddScoped<ITwoFaRepository, MemoryTwoFaRepository>();
        builder.Services.AddScoped<IUserRepository, EFUsersRepository>();
        builder.Services.AddScoped<IUserSubscriptionRepository, EFUserSubscriptionRepository>();
        
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        
        builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer();
        builder.Services.AddAuthorization(); 
    }

    public static void UseUserEndpointModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapUserAuthEndpoints();
        endpoints.MapUserCodesEndpoints();
    }
}