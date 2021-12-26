using Microsoft.AspNetCore.Mvc;
using SurveySystem.Models;

namespace SurveySystem.Extensions;

public static class ControllerBaseExtensions
{
    public static IActionResult CustomApiResponse(this ControllerBase controllerBase, ApiResponse reponse) =>
        controllerBase.StatusCode(reponse.HttpStatusCode, reponse);
}