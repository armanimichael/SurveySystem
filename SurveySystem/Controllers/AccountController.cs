using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SurveySystem.ApiResponses;
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
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserService userService, ILogger<AccountController> logger, IJwtService jwtService)
    {
        _userService = userService;
        _logger = logger;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegistrationModel registrationModel)
    {
        ApiResponse response;
        try
        {
            response = await _userService.Register(registrationModel);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error registering the user {Username}", registrationModel.Username);
            response = AccountApiResponses.RegistrationError;
        }

        return this.CustomApiResponse(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginModel loginModel)
    {
        ApiResponse response;
        try
        {
            response = await _userService.Login(loginModel);

            if (response.Success)
            {
                CreateRefreshTokenCookie((response.MetaData as AuthResult)!);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error logging the user {Username}", loginModel.Username);
            response = AccountApiResponses.LoginError;
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
        ApiResponse response;
        try
        {
            userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId));
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            response = await _userService.VerifyEmail(token, userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error verifying the user with ID {Id}", userId);
            response = AccountApiResponses.RegistrationTokenVerificationError;
        }

        return this.CustomApiResponse(response);
    }

    [HttpGet("RefreshToken")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken()
    {
        string? refreshToken = Request.Cookies.SingleOrDefault(c => c.Key == "refreshToken").Value;
        if (refreshToken == null) return Unauthorized();

        RefreshToken? refreshTokenInDb = await _jwtService.GetRefreshToken(refreshToken);
        if (refreshTokenInDb == null) return Unauthorized();

        AuthResult newToken = await _jwtService.GenerateJwtToken(refreshTokenInDb.User);
        return Ok(newToken);
    }

    [HttpGet("UserInfo")]
    [Authorize]
    public async Task<IActionResult> UserInfo()
    {
        ApiResponse response;
        try
        {
            IdentityUser user = await _userService.GetCurrentUser() ?? throw new NullReferenceException();
            
            var userInfo = new UserInfo(user.UserName, user.Email);
            response = new ApiResponse(true, userInfo, (int)HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            const string errMsg = "There was an error retriving the user informations";
            _logger.LogError(e, errMsg);
            response = new ApiResponse(false, null, (int)HttpStatusCode.InternalServerError);
            response.Message = errMsg;
        }

        return this.CustomApiResponse(response);
    }
}