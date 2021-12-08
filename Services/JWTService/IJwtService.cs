using System.Security.Claims;

namespace SurveySystem.services.JWTService;

public interface IJwtService
{
    public (string token, DateTime expiration) GenerateToken(IEnumerable<Claim> authClaims);
}