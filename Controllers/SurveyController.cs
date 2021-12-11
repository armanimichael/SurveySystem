using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveySystem.Data;
using SurveySystem.Dtos;
using SurveySystem.Models;
using SurveySystem.services.UserService;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
public class SurveyController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserService _userService;

    public SurveyController(ApplicationDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IEnumerable<Survey>> Get()
    {
        return await _dbContext.Surveys.ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<Survey?> Get(Guid id)
    {
        return await _dbContext.Surveys.SingleOrDefaultAsync(s => s.Id == id);
    }

    [HttpPost]
    [Authorize]
    public async Task<Guid> Create(SurveyDto survey)
    {
        if (!ModelState.IsValid || await _dbContext.Surveys.AnyAsync(s => s.Name == survey.Name))
        {
            return Guid.Empty;
        }

        var surveyInDb = new Survey()
        {
            Name = survey.Name,
            Description = survey.Description,
            UserId = (await _userService.GetCurrentUser()).Id
        };

        await _dbContext.AddAsync(surveyInDb);

        await _dbContext.SaveChangesAsync();
        return surveyInDb.Id;
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