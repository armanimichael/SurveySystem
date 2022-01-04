using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SurveySystem.Data;
using SurveySystem.Models;

namespace SurveySystem.Services.JWTService;

public class JwtService : IJwtService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TokenValidationParameters _tokenValidationParams;


    public JwtService(IConfiguration configuration, TokenValidationParameters tokenValidationParams, ApplicationDbContext dbContext)
    {
        _tokenValidationParams = tokenValidationParams;
        _dbContext = dbContext;
        _secretKey = configuration["JWT:SecretKey"];
        _issuer = configuration["JWT:ValidIssuer"];
        _audience = configuration["JWT:ValidAudience"];
    }

    public (string token, DateTime expiration) GenerateToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: DateTime.UtcNow.AddSeconds(30),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
    }
    
    public async Task<AuthResult> GenerateJwtToken(IdentityUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim("Id", user.Id), 
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddSeconds(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        var refreshToken = new RefreshToken(){
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

        return new AuthResult(jwtToken, refreshToken.Token);
    }
}