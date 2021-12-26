using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SurveySystem.ApiResponses;
using SurveySystem.Models;
using SurveySystem.services.UserService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserService userService, ILogger<AccountController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegistrationModel registrationModel)
    {
        try
        {
            ApiResponse registrationResponse = await _userService.Register(registrationModel);
            return StatusCode(registrationResponse.HttpStatusCode, registrationResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error registering the user {Username}", registrationModel.Username);
            return StatusCode(AccountApiResponses.RegistrationError.HttpStatusCode, AccountApiResponses.RegistrationError);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginModel loginModel)
    {
        try
        {
            ApiResponse loginResponse = await _userService.Login(loginModel);
            return StatusCode(loginResponse.HttpStatusCode, loginResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error logging the user {Username}", loginModel.Username);
            return StatusCode(AccountApiResponses.LoginError.HttpStatusCode, AccountApiResponses.LoginError);
        }
    }

    [HttpGet("Verify")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify(string userId, string token)
    {
        userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId));
        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        ApiResponse verificationResponse = await _userService.VerifyEmail(token, userId);
        return StatusCode(verificationResponse.HttpStatusCode, verificationResponse);
    }
}