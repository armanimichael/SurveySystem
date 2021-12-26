using System.Net;
using Microsoft.AspNetCore.Identity;
using SurveySystem.Models;

namespace SurveySystem.ApiResponses;

public static class AccountApiResponses
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
    
    public static readonly ApiResponse UnverifiedEmail = new()
    {
        Message = "You must verify your account, check your email.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.Unauthorized
    };
    
    public static readonly ApiResponse ConfirmationEmailError = new()
    {
        Message = "There was an error while sending the confirmation email.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.InternalServerError
    };
    
    public static readonly ApiResponse ConfirmationError = new()
    {
        Message = "Could not confirm user.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.Unauthorized
    };
    
    public static readonly ApiResponse ConfirmationSuccess = new()
    {
        Message = "User email confirmed successfully.",
        Success = true,
        HttpStatusCode = (int)HttpStatusCode.OK
    };

    public static readonly ApiResponse LoginError = new()
    {
        Message = "There was an error loggin the user in.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.InternalServerError
    };
    
    public static readonly ApiResponse RegistrationError = new()
    {
        Message = "There was an error registering the user.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.InternalServerError
    };
    
    public static ApiResponse GenerateRegistrationErrorResponse(IdentityResult result)
    {
        return new ApiResponse()
        {
            Message = "Registration error.",
            Success = false,
            Errors = result.Errors.Select(err => $"{err.Code}: {err.Description}"),
            HttpStatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
}