﻿using System.Text;
using Microsoft.AspNetCore.Authorization;
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
                CreateRefreshTokenCookie((AuthResult)response.MetaData);
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
        var (_, refreshToken, expire) = authResult;
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions()
        {
            Expires = expire
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
        string? refreshToken = Request.Cookies.SingleOrDefault(c => c.Key == "refreshTokend").Value;
        if (refreshToken == null) return Unauthorized();

        RefreshToken? refreshTokenInDb = await _jwtService.GetRefreshToken(refreshToken);
        if (refreshTokenInDb == null) return Unauthorized();

        AuthResult newToken = await _jwtService.GenerateJwtToken(refreshTokenInDb.User);
        return Ok(newToken);
    }
}