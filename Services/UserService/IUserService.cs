using Microsoft.AspNetCore.Mvc;
using SurveySystem.Models;

namespace SurveySystem.services.UserService;

public interface IUserService
{
    public Task<ApiResponse> Register(UserRegistrationModel registrationModel);
    public Task<ApiResponse> Login(UserLoginModel loginModel);
}