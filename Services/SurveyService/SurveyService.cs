using Microsoft.EntityFrameworkCore;
using SurveySystem.Data;
using SurveySystem.Models;

namespace SurveySystem.services.SurveyService;

public class SurveyService : ISurveyService
{
    private readonly ApplicationDbContext _dbContext;

    public SurveyService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<Survey>> Get()
    {
        return await _dbContext.Surveys.ToListAsync();
    }

    public async Task<Survey?> Get(Guid id)
    {
        return await _dbContext.Surveys.SingleOrDefaultAsync(s => s.Id.Equals(id));
    }
}