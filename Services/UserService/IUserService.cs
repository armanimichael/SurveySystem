using Microsoft.AspNetCore.Identity;
using SurveySystem.Models;

namespace SurveySystem.services.UserService;

public interface IUserService
{
    public Task<ApiResponse> Register(UserRegistrationModel registrationModel);
    public Task<ApiResponse> Login(UserLoginModel loginModel);
    public Task<ApiResponse> VerifyEmail(string userId, string token);
    public Task<IdentityUser> GetCurrentUser();
}