using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveySystem.Data;
using SurveySystem.Dtos;
using SurveySystem.Models;
using SurveySystem.services.SurveyService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
public class SurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;
    private readonly ApplicationDbContext _dbContext;

    public SurveyController(ApplicationDbContext dbContext, ISurveyService surveyService)
    {
        _dbContext = dbContext;
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
            response = DefaultReponses.Error;
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
            response = DefaultReponses.Error;
        }

        return StatusCode(response.HttpStatusCode, response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(SurveyDto survey)
    {
        ApiResponse response;
        if (!ModelState.IsValid)
        {
            response = new ApiResponse(false, ModelState.Values, (int)HttpStatusCode.BadRequest);
            return StatusCode(response.HttpStatusCode, response);
        }

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
            response = DefaultReponses.Error;
        }
        
        return StatusCode(response.HttpStatusCode, response);
    }

    [HttpPut]
    [Authorize]
    public async Task<int> Update(Survey survey)
    {
        Survey? dbEntry = await _dbContext.Surveys.AsNoTracking().SingleOrDefaultAsync();
        if (!ModelState.IsValid || dbEntry is null)
        {
            return -1;
        }

        _dbContext.Surveys.Update(survey);
        return await _dbContext.SaveChangesAsync();
    }
}