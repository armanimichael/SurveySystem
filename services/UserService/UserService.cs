using System.Net;
using Microsoft.AspNetCore.Identity;
using SurveySystem.Models;

namespace SurveySystem.services.UserService;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    private async Task<bool> IsUserAlreadyRegistered(string username, string email)
    {
        bool usernameExists = await _userManager.FindByNameAsync(username) != null;
        bool emailExists = await _userManager.FindByEmailAsync(email) != null;
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

    private async Task<ApiResponse> CreateUser(string email, string username, string password)
    {
        var newUser = new IdentityUser() { Email = email, UserName = username };
        
        IdentityResult? result = await _userManager.CreateAsync(newUser, password);
        return !result.Succeeded
            ? RegistrationErrorResponse(result)
            : DefaultResponses.UserRegisteredResponse;
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

    public Task<ApiResponse> Login(UserLoginModel loginModel)
    {
        throw new NotImplementedException();
    }
}