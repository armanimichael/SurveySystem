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
}