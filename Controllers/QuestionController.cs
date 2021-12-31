using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveySystem.ApiResponses;
using SurveySystem.Dtos;
using SurveySystem.Extensions;
using SurveySystem.Models;
using SurveySystem.services.QuestionService;
using SurveySystem.services.SurveyService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;
    private readonly ISurveyService _surveyService;
    private readonly ILogger<QuestionController> _logger;

    public QuestionController(IQuestionService questionService, ILogger<QuestionController> logger, ISurveyService surveyService)
    {
        _questionService = questionService;
        _logger = logger;
        _surveyService = surveyService;
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
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(QuestionDto question)
    {
        ApiResponse response;
        try
        {
            // No Permissions
            if (!await IsCurrentUserOwner(question.SurveyId))
                return this.CustomApiResponse(QuestionApiResponses.NoPermission);

            // Try to create
            response = await TryCreateQuestion(question);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error creating the Question for Survey with ID = {Id}", question.SurveyId);
            response = QuestionApiResponses.GetError;
        }

        return this.CustomApiResponse(response);
    }

    private async Task<ApiResponse> TryCreateQuestion(QuestionDto question)
    {
        var newQuestion = new Question(question.Title, question.Description, question.SurveyId, question.IsMultipleChoices);
        Question? questionInDb = await _questionService.Create(newQuestion);
        
        return questionInDb == null
            ? SurveyApiReponses.NotUnique
            : new ApiResponse(true, questionInDb, (int)HttpStatusCode.Created);
    }

    private async Task<bool> IsCurrentUserOwner(Guid surveyId)
    {
        string userId = (await _surveyService.Get(surveyId))?.UserId ?? string.Empty;
        return await _surveyService.IsCurrentUserOwner(userId);
    }
}