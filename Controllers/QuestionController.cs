using System.Net;
using Microsoft.AspNetCore.Mvc;
using SurveySystem.ApiResponses;
using SurveySystem.Extensions;
using SurveySystem.Models;
using SurveySystem.services.QuestionService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;
    private readonly ILogger<QuestionController> _logger;

    public QuestionController(IQuestionService questionService, ILogger<QuestionController> logger)
    {
        _questionService = questionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        ApiResponse response;
        try
        {
            Question? question = await _questionService.Get(id);
            response = question == null
                ? QuestionApiResponses.NotFound
                : new ApiResponse(true, question, (int)HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error retriving the Question with ID = {Id}", id);
            response = QuestionApiResponses.GetError;
        }

        return this.CustomApiResponse(response);
    }
}