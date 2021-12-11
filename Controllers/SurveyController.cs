using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveySystem.Dtos;
using SurveySystem.Models;
using SurveySystem.services.SurveyService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
public class SurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;

    public SurveyController(ISurveyService surveyService)
    {
        _surveyService = surveyService;
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
        catch (Exception)
        {
            response = DefaultReponses.GetError;
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
                ? DefaultReponses.NotFound
                : new ApiResponse(true, survey, (int)HttpStatusCode.OK);
        }
        catch (Exception)
        {
            response = DefaultReponses.GetError;
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
                ? DefaultReponses.NotUnique
                : new ApiResponse(true, surveyInDb, (int)HttpStatusCode.Created);
        }
        catch (Exception)
        {
            response = DefaultReponses.GetError;
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
            (bool updated, string? errorMessage) = await _surveyService.Update(newSurvey);

            response = updated
                ? new ApiResponse(true, null, (int)HttpStatusCode.OK)
                : new ApiResponse(false, errorMessage, (int)HttpStatusCode.Conflict);
        }
        catch (Exception)
        {
            response = DefaultReponses.UpdateError;
        }

        return StatusCode(response.HttpStatusCode, response);
    }
}