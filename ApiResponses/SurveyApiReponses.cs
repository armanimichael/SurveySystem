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
}