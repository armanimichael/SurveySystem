﻿using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using SurveySystem.ApiResponses;
using SurveySystem.Models;
using SurveySystem.Services.JWTService;
using SurveySystem.Services.MailService;

namespace SurveySystem.Services.UserService;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IMailService _mailService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(UserManager<IdentityUser> userManager, IJwtService jwtService,
        SignInManager<IdentityUser> signInManager, IMailService mailService, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _signInManager = signInManager;
        _mailService = mailService;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<bool> IsUserAlreadyRegistered(string? username, string? email)
    {
        bool usernameExists = !string.IsNullOrEmpty(username) && await _userManager.FindByNameAsync(username) != null;
        bool emailExists = !string.IsNullOrEmpty(email) && await _userManager.FindByEmailAsync(email) != null;

        return usernameExists || emailExists;
    }

    private async Task<bool> SendConfirmationMail(IdentityUser user)
    {
        string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        try
        {
            _mailService.SendConfirmationToken(user.Email!, user.Id, confirmationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<ApiResponse> CreateUser(string? email, string username, string password)
    {
        var newUser = new IdentityUser() { Email = email, UserName = username };

        IdentityResult? result = await _userManager.CreateAsync(newUser, password);
        if (result.Succeeded && !await SendConfirmationMail(newUser))
        {
            return AccountApiResponses.ConfirmationEmailError;
        }

        return !result.Succeeded
            ? AccountApiResponses.GenerateRegistrationErrorResponse(result)
            : AccountApiResponses.UserRegisteredResponse;
    }

    private ApiResponse GetJwtToken(IEnumerable<Claim> authClaims)
    {
        (string token, DateTime expiration) = _jwtService.GenerateToken(authClaims);

        ApiResponse successResponse = AccountApiResponses.UserLoggedInResponse;
        successResponse.MetaData = new
        {
            token,
            expiration
        };
        return successResponse;
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

    public async Task<ApiResponse> Register(UserRegistrationModel registrationModel)
    {
        string username = registrationModel.Username;
        string email = registrationModel.Email;
        string password = registrationModel.Password;
        if (await IsUserAlreadyRegistered(username, email))
        {
            return AccountApiResponses.UserExistsResponse;
        }

        return await CreateUser(email, username, password);
    }

    public async Task<ApiResponse> Login(UserLoginModel loginModel)
    {
        bool userExists = await IsUserAlreadyRegistered(loginModel.Username, null);
        if (!userExists)
        {
            return AccountApiResponses.IncorrectLoginData;
        }

        IdentityUser user = await _userManager.FindByNameAsync(loginModel.Username);

        // Check password and verified account
        SignInResult result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
        if (!result.Succeeded)
        {
            return result.IsNotAllowed ? AccountApiResponses.UnverifiedEmail : AccountApiResponses.IncorrectLoginData;
        }

        // Get JWT Token
        List<Claim> authClaims = await CreateUserClaims(user);
        return GetJwtToken(authClaims);
    }

    public async Task<ApiResponse> VerifyEmail(string userId, string token)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return AccountApiResponses.ConfirmationError;
        }

        IdentityResult? result = await _userManager.ConfirmEmailAsync(user, token);
        return result is { Succeeded: true }
            ? AccountApiResponses.ConfirmationSuccess
            : AccountApiResponses.ConfirmationError;
    }

    public async Task<string?> GetCurrentUserId()
    {
        IdentityUser? user = await GetCurrentUser(); 
        return (user)?.Id ?? null;
    }
    
    private async Task<IdentityUser?> GetCurrentUser()
    {
        string username = _httpContextAccessor.HttpContext!.User.Identity?.Name ?? string.Empty;
        return await _userManager.FindByNameAsync(username);
    }
}