using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SurveySystem.Dtos;
using SurveySystem.Models;
using SurveySystem.Services.UserService;
using SurveySystem.Extensions;
using SurveySystem.Services.JWTService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public AccountController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegistrationModel registrationModel)
    {
        ApiResponse response = await _userService.Register(registrationModel);

        return this.CustomApiResponse(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginModel loginModel)
    {
        ApiResponse response = await _userService.Login(loginModel);
        if (response.Success)
        {
            CreateRefreshTokenCookie((response.MetaData as AuthResult)!);
        }

        return this.CustomApiResponse(response);
    }

    private void CreateRefreshTokenCookie(AuthResult authResult)
    {
        var (_, _, refreshToken, refreshTokenExpire) = authResult;
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions()
        {
            Expires = refreshTokenExpire
        });
    }

    [HttpGet("Verify")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify(string userId, string token)
    {
        userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId));
        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        ApiResponse response = await _userService.VerifyEmail(token, userId);

        return this.CustomApiResponse(response);
    }

    [HttpGet("RefreshToken")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken()
    {
        string? refreshToken = Request.Cookies.SingleOrDefault(c => c.Key == "refreshToken").Value;
        if (refreshToken is null)
        {
            return Unauthorized();
        }

        RefreshToken? refreshTokenInDb = await _jwtService.GetRefreshToken(refreshToken);
        if (refreshTokenInDb == null)
        {
            return Unauthorized();
        }

        AuthResult newToken = await _jwtService.GenerateJwtToken(refreshTokenInDb.User);
        return Ok(newToken);
    }

    [HttpGet("UserInfo")]
    [Authorize]
    public async Task<IActionResult> UserInfo()
    {
        IdentityUser user = await _userService.GetCurrentUser() ?? throw new NullReferenceException();

        var userInfo = new UserInfo(user.UserName, user.Email);
        var response = new ApiResponse(true, userInfo, (int)HttpStatusCode.OK);

        return this.CustomApiResponse(response);
    }
}