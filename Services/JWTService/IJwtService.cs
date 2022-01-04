using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SurveySystem.Models;

namespace SurveySystem.Services.JWTService;

public interface IJwtService
{
    public Task<AuthResult> GenerateJwtToken(IdentityUser user);
}