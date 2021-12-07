using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveySystem.Data;
using SurveySystem.Models;

namespace SurveySystem.Controllers;

[ApiController]
[Route("[controller]")]
public class SurveyController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public SurveyController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IEnumerable<Survey>> Get()
    {
        return await _dbContext.Surveys.ToListAsync();
    }
    
    [HttpGet("{id:int}")]
    public async Task<Survey?> Get(int id)
    {
        return await _dbContext.Surveys.SingleOrDefaultAsync(s => s.Id == id);
    }

    [HttpPost]
    [Authorize]
    public async Task<int> Create(Survey survey)
    {
        if (!ModelState.IsValid) return -1;

        survey.Id = 0;
        await _dbContext.AddAsync(survey);
        await _dbContext.SaveChangesAsync();
        return survey.Id;
    }

    [HttpPut]
    [Authorize]
    public async Task<int> Update(Survey survey)
    {
        Survey? dbEntry = await _dbContext.Surveys.AsNoTracking().SingleOrDefaultAsync();
        if (!ModelState.IsValid || dbEntry is null || survey.Id <= 0)
        {
            return -1;
        }
        
        _dbContext.Surveys.Update(survey);
        return await _dbContext.SaveChangesAsync();
    }
}