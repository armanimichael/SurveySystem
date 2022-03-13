using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveySystem.Data;
using SurveySystem.Dtos;
using SurveySystem.Models;

namespace SurveySystem.Services.JWTService;

public class JwtService : IJwtService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly IConfiguration _configuration;


    public JwtService(IConfiguration configuration, ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _configuration = configuration;
        _secretKey = configuration["JWT:SecretKey"];
        _issuer = configuration["JWT:ValidIssuer"];
        _audience = configuration["JWT:ValidAudience"];
    }

    private TimeSpan GetTokenLifespan(TokenType type)
    {
        string tokenType = type == TokenType.Jwt ? "TokenLifespan" : "RefreshTokenLifespan";
        int days = _configuration.GetValue<int>($"JWT:{tokenType}:Days");
        int hours = _configuration.GetValue<int>($"JWT:{tokenType}:Hours");
        int minutes = _configuration.GetValue<int>($"JWT:{tokenType}:Minutes");

        return new TimeSpan(days, hours, minutes, 0);
    }

    public async Task<AuthResult> GenerateJwtToken(IdentityUser user)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var expire = DateTime.UtcNow.Add(GetTokenLifespan(TokenType.Jwt));
        
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: expire,
            claims: await CreateUserClaims(user),
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        RefreshToken refreshToken = await CreateRefreshToken(user.Id);
        return new AuthResult(jwtToken, expire, refreshToken.Token, refreshToken.ExpiryDate);
    }

    private async Task<RefreshToken> CreateRefreshToken(string userId)
    {
        var refreshToken = new RefreshToken()
        {
            ExpiryDate = DateTime.UtcNow.Add(GetTokenLifespan(TokenType.Refresh)),
            Token = CreateRandomBase64Token(),
            UserId = userId
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

    private async Task<List<Claim>> CreateUserClaims(IdentityUser user)
    {
        IList<string>? userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
        return authClaims;
    }

    public async Task<RefreshToken?> GetRefreshToken(string refreshTokenKey) =>
        await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .Where(rt => DateTime.UtcNow < rt.ExpiryDate)
            .SingleOrDefaultAsync(rt => rt.Token == refreshTokenKey);
}