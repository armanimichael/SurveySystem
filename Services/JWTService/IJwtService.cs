using Microsoft.AspNetCore.Identity;
using SurveySystem.Dtos;

namespace SurveySystem.Services.JWTService;

public interface IJwtService
{
    public Task<AuthResult> GenerateJwtToken(IdentityUser user);
}