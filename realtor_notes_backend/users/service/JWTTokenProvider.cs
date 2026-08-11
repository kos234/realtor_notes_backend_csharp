using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using realtor_notes_backend.users.consts;
using realtor_notes_backend.users.model;

namespace realtor_notes_backend.users.service;

public interface IJWTTokenProvider
{
    public Task<string> GetAccessToken(int userId, long sessionId, bool isFullAccess, AuthActions[] nextActions);
}

public class JWTTokenProvider(IUserSubscriptionService userSubscriptionService) : IJWTTokenProvider
{
    private readonly string _jwtSecret = "";

    public JWTTokenProvider(IUserSubscriptionService userSubscriptionService, IConfiguration configuration) : this(userSubscriptionService)
    {
        _jwtSecret = configuration["JwtOptions:SecretKey"] ?? string.Empty;
    }

    private SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
    }

    public async Task<string> GetAccessToken(int userId, long sessionId, bool isFullAccess, AuthActions[] nextActions)
    {
        var userSubscription = await userSubscriptionService.GetActiveSubscription(userId);
        var key = GetSymmetricSecurityKey();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userId.ToString()),
            new Claim("IsFullAccess", isFullAccess.ToString()),
            new Claim("SessionId", sessionId.ToString()),
            new Claim("AuthActions", AuthActionsToString(nextActions)),
            new Claim("Features",
                string.Join(";", userSubscription?.Subscription?.Features.Select(vl => vl.ToString()) ?? [])),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string AuthActionsToString(AuthActions[] actions)
    {
        return string.Join(";", actions.Select(vl => (int)vl));
    }
}