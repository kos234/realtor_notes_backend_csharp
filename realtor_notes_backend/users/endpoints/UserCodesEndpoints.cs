using System.Security.Claims;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using realtor_notes_backend.users.consts;
using realtor_notes_backend.users.dtos;
using realtor_notes_backend.users.service;
using realtor_notes_backend.utils;

namespace realtor_notes_backend.users.endpoints;

public static class UserCodesEndpoints
{
    public static void MapUserCodesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/user/codes");
        
        group.MapPost("/email/register/refresh",  [Authorize] async (IEmailCodeService emailCodeService, ClaimsPrincipal user) =>
        {
            var (userId, _, authActions) = ParseRequest(user);
            
            var response = await emailCodeService.SendEmailConfirmCode(userId, authActions);
            if (response.IsError)
                return response.ToProblemDetails();
            return Results.Json(new {TextTimeSend = response.Value});
        }); 
        
        group.MapPost("/email/register/confirm",  [Authorize] async (CodeRequestDTO code, IEmailCodeService emailCodeService, ISessionService sessionService, IJWTTokenProvider jwtProvider, ClaimsPrincipal user) =>
        {
            var (userId, sessionId, authActions) = ParseRequest(user);
            
            var response = await emailCodeService.CheckEmailConfirmCode(userId, authActions, code.Code);
            if (response.IsError)
                return response.ToProblemDetails();
            
            await sessionService.SetSessionEntered(userId, sessionId);
            return Results.Ok();
        });
        
        group.MapPost("/email/login/refresh",  [Authorize] async (IEmailCodeService emailCodeService, ClaimsPrincipal user) =>
        {
            var (userId, _, authActions) = ParseRequest(user);
            
            var response = await emailCodeService.SendLoginCode(userId, authActions);
            if (response.IsError)
                return response.ToProblemDetails();
            return Results.Json(new {TextTimeSend = response.Value});
        }); 
        
        group.MapPost("/email/login/confirm",  [Authorize] async (CodeRequestDTO code, IEmailCodeService emailCodeService, ISessionService sessionService, IJWTTokenProvider jwtProvider, ClaimsPrincipal user) =>
        {
            var (userId, sessionId, authActions) = ParseRequest(user);
            
            var response = await emailCodeService.CheckLoginCode(userId, authActions, code.Code);
            if (response.IsError)
                return response.ToProblemDetails();
            
            await sessionService.SetSessionEntered(userId, sessionId);
            return Results.Ok();
        });
        
        group.MapPost("/twofa/register/confirm",  [Authorize] async (CodeRequestDTO code, ITwoFaService twoFaService, ClaimsPrincipal user) =>
        {
            var (userId, _, authActions) = ParseRequest(user);
            var res = await twoFaService.CheckConfirmKeyCode(userId, code.Code, authActions);
            if (res.IsError)
                return res.ToProblemDetails();
            return Results.Ok();
        });
        
        group.MapPost("/twofa/register/get",  [Authorize] async (ITwoFaService twoFaService, ClaimsPrincipal user) =>
        {
            var (userId, _, _) = ParseRequest(user);
            var dto = await twoFaService.GetSecretKey(userId);

            return Results.Ok(dto);
        });
        
        group.MapPost("/twofa/login/confirm",  [Authorize] async (CodeRequestDTO code, ITwoFaService twoFaService, ISessionService sessionService, ClaimsPrincipal user) =>
        {
            var (userId, sessionId, authActions) = ParseRequest(user);
            var res = await twoFaService.CheckLoginCode(userId, code.Code, authActions);
            if (res.IsError)
                return res.ToProblemDetails();
            
            await sessionService.SetSessionEntered(userId, sessionId);

            return Results.Ok();
        });
        
        group.MapPost("/twofa/register/cancel", [Authorize] async (ITwoFaService twoFaService, ClaimsPrincipal user) =>
        {
            var (userId, _, _) = ParseRequest(user);
            await twoFaService.CancelTwoFaSetup(userId);
            return Results.Ok();
        });
    }

    private static (int userId, long sessionId, AuthActions[] authActions) ParseRequest(ClaimsPrincipal user)
    {
        var userId = int.Parse(user.FindFirst(ClaimTypes.Name)!.Value);
        var sessionId = long.Parse(user.FindFirst("SessionId")!.Value);
        var authActions = ParseActions(user.FindFirst("AuthActions")?.Value);
        return (userId, sessionId, authActions);
    }
    
    private static AuthActions[] ParseActions(string? authActionsString)
    {
        if (string.IsNullOrEmpty(authActionsString))
            return [];
        var arr = authActionsString.Split(";");
        AuthActions[] actions = new AuthActions[arr.Length];
        for (var index = 0; index < arr.Length; index++)
        {
            actions[index] = Enum.Parse<AuthActions>(arr[index]);
        }
        return actions;
    }
}