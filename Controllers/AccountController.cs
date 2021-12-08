﻿using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SurveySystem.Models;
using SurveySystem.services.UserService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegistrationModel registrationModel)
    {
        ApiResponse registrationResponse = await _userService.Register(registrationModel);
        return StatusCode(registrationResponse.HttpStatusCode, registrationResponse);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginModel loginModel)
    {
        ApiResponse loginResponse = await _userService.Login(loginModel);
        return StatusCode(loginResponse.HttpStatusCode, loginResponse);
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