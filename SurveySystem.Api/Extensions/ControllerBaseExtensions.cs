using Microsoft.AspNetCore.Mvc;
using SurveySystem.Data.Models;

namespace SurveySystem.Extensions;

public static class ControllerBaseExtensions
{
    public static IActionResult CustomApiResponse(this ControllerBase controllerBase, ApiResponse response) =>
        controllerBase.StatusCode(response.HttpStatusCode, response);
}