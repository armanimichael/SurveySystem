using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SurveySystem.Models;
using SurveySystem.services.JWTService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtService _jwtService;

    public AccountController(UserManager<IdentityUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }


    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegistrationModel registrationModel)
    {
        bool usernameExists = await _userManager.FindByNameAsync(registrationModel.Username) != null;
        bool emailExists = await _userManager.FindByEmailAsync(registrationModel.Email) != null;
        if (usernameExists || emailExists)
        {
            return Conflict(new { message = "User already exists" });
        }

        var newUser = new IdentityUser()
        {
            Email = registrationModel.Email,
            UserName = registrationModel.Username
        };
        IdentityResult? result = await _userManager.CreateAsync(newUser, registrationModel.Password);
        return !result.Succeeded
            ? StatusCode(StatusCodes.Status500InternalServerError, new { errors = result.Errors })
            : Ok();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        IdentityUser? user = await _userManager.FindByNameAsync(loginModel.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password)) return Unauthorized();

        IList<string>? userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        (string token, DateTime expiration) jwt = _jwtService.GenerateToken(authClaims);
        return Ok(new
        {
            jwt.token,
            jwt.expiration
        });
    }
}