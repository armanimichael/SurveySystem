using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SurveySystem.Data;
using SurveySystem.Dtos;
using SurveySystem.Models;

namespace SurveySystem.Services.JWTService;

public class JwtService : IJwtService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _secretKey;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JwtService(IConfiguration configuration, ApplicationDbContext dbContext,
        TokenValidationParameters tokenValidationParameters)
    {
        _dbContext = dbContext;
        _tokenValidationParameters = tokenValidationParameters;
        _secretKey = configuration["JWT:SecretKey"];
    }

    public async Task<AuthResult> GenerateJwtToken(IdentityUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = CreateTokenDescriptor(user);

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        RefreshToken refreshToken = await CreateRefreshToken(user, token);

        return new AuthResult(jwtToken, refreshToken.Token);
    }

    private async Task<RefreshToken> CreateRefreshToken(IdentityUser user, SecurityToken token)
    {
        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            IsUsed = false,
            UserId = user.Id,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddYears(1),
            IsRevoked = false,
            Token = Guid.NewGuid().ToString()
        };
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();
        return refreshToken;
    }

    private SecurityTokenDescriptor CreateTokenDescriptor(IdentityUser user)
    {
        var key = Encoding.ASCII.GetBytes(_secretKey);
        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddSeconds(30),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
    }
}