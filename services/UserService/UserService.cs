using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using SurveySystem.Models;
using SurveySystem.services.JWTService;

namespace SurveySystem.services.UserService;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtService _jwtService;

    public UserService(UserManager<IdentityUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    private async Task<bool> IsUserAlreadyRegistered(string? username, string? email)
    {
        bool usernameExists = !string.IsNullOrEmpty(username) && await _userManager.FindByNameAsync(username) != null;
        bool emailExists = !string.IsNullOrEmpty(email) && await _userManager.FindByEmailAsync(email) != null;

        return usernameExists || emailExists;
    }

    private static ApiResponse RegistrationErrorResponse(IdentityResult result)
    {
        return new ApiResponse()
        {
            Message = "Registration error.",
            Success = false,
            Errors = result.Errors.Select(err => $"{err.Code}: {err.Description}"),
            HttpStatusCode = (int)HttpStatusCode.InternalServerError
        };
    }

    private async Task<ApiResponse> CreateUser(string? email, string username, string password)
    {
        var newUser = new IdentityUser() { Email = email, UserName = username };

        IdentityResult? result = await _userManager.CreateAsync(newUser, password);
        return !result.Succeeded
            ? RegistrationErrorResponse(result)
            : DefaultResponses.UserRegisteredResponse;
    }

    private ApiResponse GetJwtToken(IEnumerable<Claim> authClaims)
    {
        (string token, DateTime expiration) = _jwtService.GenerateToken(authClaims);

        ApiResponse successResponse = DefaultResponses.UserLoggedInResponse;
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
            return DefaultResponses.UserExistsResponse;
        }

        return await CreateUser(email, username, password);
    }

    public async Task<ApiResponse> Login(UserLoginModel loginModel)
    {
        bool userExists = await IsUserAlreadyRegistered(loginModel.Username, null);
        if (!userExists)
        {
            return DefaultResponses.IncorrectLoginData;
        }

        IdentityUser user = await _userManager.FindByNameAsync(loginModel.Username);
        bool validPassword = await _userManager.CheckPasswordAsync(user, loginModel.Password);
        if (!validPassword)
        {
            return DefaultResponses.IncorrectLoginData;
        }
        
        List<Claim> authClaims = await CreateUserClaims(user);
        return GetJwtToken(authClaims);
    }
}