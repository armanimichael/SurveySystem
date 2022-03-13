using Microsoft.AspNetCore.Identity;
using SurveySystem.Data.Models;
using SurveySystem.Data.Models.Dtos;

namespace SurveySystem.Services.JWTService;

public interface IJwtService
{
    public Task<AuthResult> GenerateJwtToken(IdentityUser user);
    public Task<RefreshToken?> GetRefreshToken(string refreshTokenKey);
}