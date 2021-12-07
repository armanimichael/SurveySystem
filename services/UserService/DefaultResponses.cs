using System.Net;
using SurveySystem.Models;

namespace SurveySystem.services.UserService;

public static class DefaultResponses
{
    public static readonly ApiResponse UserExistsResponse = new()
    {
        Message = "User already exists.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.Conflict
    };

    public static readonly ApiResponse UserRegisteredResponse = new()
    {
        Message = "User registered successfully.",
        Success = true,
        HttpStatusCode = (int)HttpStatusCode.OK
    };
    
    public static readonly ApiResponse UserLoggedInResponse = new()
    {
        Message = "User logged in successfully.",
        Success = true,
        HttpStatusCode = (int)HttpStatusCode.OK
    };
    
    public static readonly ApiResponse IncorrectLoginData = new()
    {
        Message = "Incorrect user or password.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.Unauthorized
    };
}