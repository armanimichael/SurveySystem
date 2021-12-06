using Microsoft.AspNetCore.Mvc;
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
    public IEnumerable<Survey> Get()
    {
        return _dbContext.Surveys.ToList();
    }
    
    [HttpGet("{id:int}")]
    public Survey? Get(int id)
    {
        return _dbContext.Surveys.SingleOrDefault(s => s.Id == id);
    }
}