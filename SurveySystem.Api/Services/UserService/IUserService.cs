using Microsoft.AspNetCore.Identity;
using SurveySystem.Data.Models;

namespace SurveySystem.Services.UserService;

public interface IUserService
{
    public Task<ApiResponse> Register(UserRegistrationModel registrationModel);
    public Task<ApiResponse> Login(UserLoginModel loginModel);
    public Task<ApiResponse> VerifyEmail(string userId, string token);
    public Task<string?> GetCurrentUserId();
    public Task<IdentityUser?> GetCurrentUser();
}