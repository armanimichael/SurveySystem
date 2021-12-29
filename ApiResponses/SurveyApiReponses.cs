using System.Net;
using SurveySystem.Models;

namespace SurveySystem.ApiResponses;

public static class SurveyApiReponses
{
    public static readonly ApiResponse GetError = new()
    {
        Message = "There was an error retriving the selected survey.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.InternalServerError,
    };
    
    public static readonly ApiResponse DeleteError = new()
    {
        Message = "There was an error deleting the selected survey.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.InternalServerError,
    };

    public static readonly ApiResponse NotFound = new()
    {
        Message = "Could not find any survey.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.NotFound
    };

    public static readonly ApiResponse NotUnique = new()
    {
        Message = "There's another survey with this name.",
        Success = true,
        HttpStatusCode = (int)HttpStatusCode.Conflict
    };
    
    public static readonly ApiResponse UpdateError = new()
    {
        Message = "There was an error updating the selected survey.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.InternalServerError,
    };
    
    public static readonly ApiResponse UpdateSuccess = new()
    {
        Message = "Survey updated successfully!",
        Success = true,
        HttpStatusCode = (int)HttpStatusCode.OK,
    };
    
    public static readonly ApiResponse NoPermissions = new()
    {
        Message = "You don't have the permission to update this survey.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.Unauthorized,
    };
}