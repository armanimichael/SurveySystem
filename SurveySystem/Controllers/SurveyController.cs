using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveySystem.ApiResponses;
using SurveySystem.Dtos;
using SurveySystem.Extensions;
using SurveySystem.Models;
using SurveySystem.Services.SurveyService;

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
        IList<Survey> survey = await _surveyService.Get();
        ApiResponse response = new ApiResponse(true, survey, (int)HttpStatusCode.OK);

        return this.CustomApiResponse(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        Survey? survey = await _surveyService.Get(id);

        ApiResponse response = survey == null
            ? SurveyApiReponses.NotFound
            : new ApiResponse(true, survey, (int)HttpStatusCode.OK);

        return this.CustomApiResponse(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(SurveyDto survey)
    {
        var newSurvey = new Survey(survey.Name, survey.Description, survey.IsVisible);
        Survey? surveyInDb = await _surveyService.Create(newSurvey);

        ApiResponse response = surveyInDb == null
            ? SurveyApiReponses.NotUnique
            : new ApiResponse(true, surveyInDb, (int)HttpStatusCode.Created);

        return this.CustomApiResponse(response);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, SurveyDto survey)
    {
        var newSurvey = new Survey(id, survey.Name, survey.Description, survey.IsVisible);
        ApiResponse response = await _surveyService.Update(newSurvey);

        return this.CustomApiResponse(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        ApiResponse response = await _surveyService.Delete(id);

        return this.CustomApiResponse(response);
    }
}