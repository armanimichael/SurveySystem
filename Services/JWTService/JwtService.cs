using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

    public JwtService(IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _secretKey = configuration["JWT:SecretKey"];
    }

    public async Task<AuthResult> GenerateJwtToken(IdentityUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = CreateTokenDescriptor(user);

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        RefreshToken refreshToken = await CreateRefreshToken();

        return new AuthResult(jwtToken, refreshToken.Token, refreshToken.ExpiryDate);
    }

    private async Task<RefreshToken> CreateRefreshToken()
    {
        var refreshToken = new RefreshToken()
        {
            ExpiryDate = DateTime.UtcNow.AddYears(1),
            Token = CreateRandomBase64Token()
        };
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();
        return refreshToken;
    }

    private static string CreateRandomBase64Token()
    {
        var randomToken = RandomNumberGenerator.Create();
        var randomBytes = new byte[256];
        randomToken.GetNonZeroBytes(randomBytes);
        string token = Convert.ToBase64String(randomBytes);
        return token;
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