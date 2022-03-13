using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SurveySystem.Extensions;

namespace SurveySystem.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    [Route("/error")]
    public IActionResult HandleError()
        => this.CustomApiResponse(new()
        {
            Success = false,
            HttpStatusCode = (int)HttpStatusCode.InternalServerError,
            Message = "There was an error with your request"
        });
}