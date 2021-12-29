using System.Security.Claims;

namespace SurveySystem.Services.JWTService;

public interface IJwtService
{
    public (string token, DateTime expiration) GenerateToken(IEnumerable<Claim> authClaims);
}