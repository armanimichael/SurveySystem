using Microsoft.AspNetCore.Identity;
using SurveySystem.Dtos;
using SurveySystem.Models;

namespace SurveySystem.Services.JWTService;

public interface IJwtService
{
    public Task<AuthResult> GenerateJwtToken(IdentityUser user);
    public Task<RefreshToken?> GetRefreshToken(string refreshTokenKey);
}