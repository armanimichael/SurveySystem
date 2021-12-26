using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveySystem.ApiResponses;
using SurveySystem.Dtos;
using SurveySystem.Models;
using SurveySystem.services.SurveyService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
public class SurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;
    private readonly ILogger<SurveyController> _logger;

    public SurveyController(ISurveyService surveyService, ILogger<SurveyController> logger)
    {
        _surveyService = surveyService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        ApiResponse response;

        try
        {
            IList<Survey> survey = await _surveyService.Get();
            response = new ApiResponse(true, survey, (int)HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error getting the Surveys");
            response = SurveyApiReponses.GetError;
        }

        return StatusCode(response.HttpStatusCode, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        ApiResponse response;

        try
        {
            Survey? survey = await _surveyService.Get(id);
            response = survey == null
                ? SurveyApiReponses.NotFound
                : new ApiResponse(true, survey, (int)HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error getting the Survey with ID = {Id}", id);
            response = SurveyApiReponses.GetError;
        }

        return StatusCode(response.HttpStatusCode, response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(SurveyDto survey)
    {
        ApiResponse response;
        try
        {
            var newSurvey = new Survey(survey.Name, survey.Description, survey.IsVisible);
            Survey? surveyInDb = await _surveyService.Create(newSurvey);
            response = surveyInDb == null
                ? SurveyApiReponses.NotUnique
                : new ApiResponse(true, surveyInDb, (int)HttpStatusCode.Created);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error creating the Survey");
            response = SurveyApiReponses.GetError;
        }

        return StatusCode(response.HttpStatusCode, response);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, SurveyDto survey)
    {
        ApiResponse response;
        try
        {
            var newSurvey = new Survey(id, survey.Name, survey.Description, survey.IsVisible);
            response = await _surveyService.Update(newSurvey);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an error updating the Survey width ID = {Id}", id);
            response = SurveyApiReponses.UpdateError;
        }

        return StatusCode(response.HttpStatusCode, response);
    }
}