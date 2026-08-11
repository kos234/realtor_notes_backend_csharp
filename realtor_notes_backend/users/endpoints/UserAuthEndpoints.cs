using ErrorOr;
using MyCSharp.HttpUserAgentParser;
using realtor_notes_backend.users.consts;
using realtor_notes_backend.users.dtos;
using realtor_notes_backend.users.model;
using realtor_notes_backend.users.service;
using realtor_notes_backend.utils;

namespace realtor_notes_backend.users.endpoints;

public static class UserAuthEndpoints
{
    public static void MapUserAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/user");

        group.MapPost("/register/web",
            async (NewUserDto dto, HttpContext context, IUserAuthService authService, IJWTTokenProvider jwtProvider) =>
            {
                var deviceId = EnsureWebDeviceId(context);
                var platformInfo = GetPlatformInfo(context, deviceId, "web");

                var result = await authService.CreateNewUserAsync(dto, platformInfo);
                if (result.IsError) return result.ToProblemDetails();
                var regRes = result.Value;
                return await HandleWebSuccessAsync(context, jwtProvider, regRes.User.Id, regRes.Session,
                    regRes.IsFullAccess, result.Value.NextActions);
            });

        group.MapPost("/login/web",
            async (UserLoginDto dto, HttpContext context, IUserAuthService authService,
                IJWTTokenProvider jwtProvider) =>
            {
                var deviceId = EnsureWebDeviceId(context);
                var platformInfo = GetPlatformInfo(context, deviceId, "web");

                var result = await authService.LoginUserAsync(dto, platformInfo);
                if (result.IsError) return result.ToProblemDetails();

                var loginRes = result.Value;
                AuthActions[] nextActions = loginRes.NextActions.HasValue ? [loginRes.NextActions.Value] : [];
                return await HandleWebSuccessAsync(context, jwtProvider, loginRes.User.Id, loginRes.Session,
                    loginRes.IsFullAccess, nextActions);
            });

        group.MapPost("/register/mobile", async (MobileNewUserDto dto, HttpContext context,
            IUserAuthService authService, IJWTTokenProvider jwtProvider) =>
        {
            var deviceId = dto.DeviceId ?? Utils.Random16Bytes();
            var platformInfo = GetPlatformInfo(context, deviceId, "mobile");

            var result = await authService.CreateNewUserAsync(dto, platformInfo);
            if (result.IsError) return result.ToProblemDetails();
            var regRes = result.Value;

            return await HandleMobileSuccessAsync(jwtProvider, deviceId, regRes.User.Id, regRes.Session,
                regRes.IsFullAccess, result.Value.NextActions);
        });

        group.MapPost("/login/mobile", async (MobileUserLoginDto dto, HttpContext context, IUserAuthService authService,
            IJWTTokenProvider jwtProvider) =>
        {
            var deviceId = dto.DeviceId ?? Utils.Random16Bytes();
            var platformInfo = GetPlatformInfo(context, deviceId, "mobile");

            var result = await authService.LoginUserAsync(dto, platformInfo);
            if (result.IsError) return result.ToProblemDetails();
            var loginRes = result.Value;
            AuthActions[] nextActions = loginRes.NextActions.HasValue ? [loginRes.NextActions.Value] : [];
            return await HandleMobileSuccessAsync(jwtProvider, deviceId, loginRes.User.Id, loginRes.Session,
                loginRes.IsFullAccess, nextActions);
        });

        app.MapPost("/token/refresh/web", async (ISessionService sessionService, IUserAuthService userAuthService,
            IJWTTokenProvider jwtTokenProvider, HttpContext context) =>
        {
            var deviceId = EnsureWebDeviceId(context);
            var platformInfo = GetPlatformInfo(context, deviceId, "web");

            if (context.Request.Cookies.TryGetValue("Refresh", out string? refresh) &&
                !string.IsNullOrWhiteSpace(refresh))
                return Results.BadRequest("Не выдан refresh токен");

            var oldSession = await sessionService.GetByRefresh(refresh!);
            if (oldSession == null)
                return Results.BadRequest("Refresh токен отозван");
            
            var newSession = await sessionService.CreateNewSessionFromSession(oldSession, platformInfo);
            var authActions = await userAuthService.GetAuthActions(newSession.UserId, newSession.Id);
            return await HandleWebSuccessAsync(context, jwtTokenProvider, newSession.UserId, newSession,
                authActions.Length == 0,
                authActions);
        });

        app.MapPost("/token/refresh/mobile", async (RefreshTokenRequestMobile request, ISessionService sessionService, IUserAuthService userAuthService,
            IJWTTokenProvider jwtTokenProvider, HttpContext context) =>
        {
            var deviceId = request.DeviceId;
            var refresh = request.RefreshToken;
            var platformInfo = GetPlatformInfo(context, deviceId, "web");

            var oldSession = await sessionService.GetByRefresh(refresh!);
            if (oldSession == null)
                return Results.BadRequest("Refresh токен отозван");
            
            var newSession = await sessionService.CreateNewSessionFromSession(oldSession, platformInfo);
            var authActions = await userAuthService.GetAuthActions(newSession.UserId, newSession.Id);
            return await HandleWebSuccessAsync(context, jwtTokenProvider, newSession.UserId, newSession,
                authActions.Length == 0,
                authActions);
        });
    }

    // --- Вспомогательные методы ---

    private static string EnsureWebDeviceId(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("DeviceId", out string? deviceId) &&
            !string.IsNullOrWhiteSpace(deviceId))
            return deviceId;

        deviceId = Utils.Random16Bytes();
        context.Response.Cookies.Append("DeviceId", deviceId, new CookieOptions
        {
            HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax, Expires = DateTimeOffset.UtcNow.AddYears(10)
        });
        return deviceId;
    }

    private static ClientPlatformInfo GetPlatformInfo(HttpContext context, string deviceId, string defaultDeviceType)
    {
        string userAgentStr = context.Request.Headers.UserAgent.ToString();
        var info = HttpUserAgentParser.Parse(userAgentStr);

        return new ClientPlatformInfo(
            DeviceId: deviceId,
            DeviceName: info.MobileDeviceType ?? defaultDeviceType,
            Platform: info.Platform?.ToString() ?? string.Empty
        );
    }

    private static async Task<IResult> HandleWebSuccessAsync(HttpContext context, IJWTTokenProvider jwtProvider,
        int userId, Session session, bool isFullAccess, AuthActions[] nextActions)
    {
        context.Response.Cookies.Append("Refresh", session.Refresh, new CookieOptions
        {
            HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax, Expires = DateTimeOffset.UtcNow.AddYears(1)
        });

        var accessToken = await jwtProvider.GetAccessToken(userId, session.Id, isFullAccess, nextActions);
        return Results.Ok(new OnAuthResponse(accessToken, nextActions));
    }

    private static async Task<IResult> HandleMobileSuccessAsync(IJWTTokenProvider jwtProvider, string deviceId,
        int userId, Session session, bool isFullAccess, AuthActions[] nextActions)
    {
        var accessToken = await jwtProvider.GetAccessToken(userId, session.Id, isFullAccess, nextActions);
        return Results.Ok(new MobileOnAuthResponse(accessToken, session.Refresh, deviceId, nextActions));
    }
}