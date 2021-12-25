using Microsoft.EntityFrameworkCore;
using SurveySystem.ApiResponses;
using SurveySystem.Data;
using SurveySystem.Models;
using SurveySystem.services.UserService;

namespace SurveySystem.services.SurveyService;

public class SurveyService : ISurveyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserService _userService;

    public SurveyService(ApplicationDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<IList<Survey>> Get()
    {
        string userId = await _userService.GetCurrentUserId() ?? "";

        return await _dbContext.Surveys
            .Where(s => userId == s.UserId || (bool)s.IsVisible!)
            .ToListAsync();
    }

    public async Task<Survey?> Get(Guid id)
    {
        string userId = await _userService.GetCurrentUserId() ?? "";

        return await _dbContext.Surveys
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id.Equals(id) && (userId == s.UserId || (bool)s.IsVisible!));
    }

    public async Task<Survey?> Create(Survey survey)
    {
        if (!await IsUnique(survey))
        {
            return null;
        }

        survey.UserId = (await _userService.GetCurrentUserId())!;
        await _dbContext.AddAsync(survey);
        await _dbContext.SaveChangesAsync();
        return survey;
    }

    private async Task<bool> IsUnique(Survey survey)
    {
        return !await _dbContext.Surveys.AnyAsync(s => s.Name == survey.Name && survey.Id != s.Id);
    }

    public async Task<(bool updated, string? errorMessage)> Update(Survey survey)
    {
        Survey? surveyInDb = await Get(survey.Id);
        survey.UserId = surveyInDb?.UserId ?? string.Empty;
        
        // Not existing
        if (surveyInDb == null) 
            return (false, SurveyApiReponses.NotFound.Message);
        
        // No permission
        if (!await IsCurrentUserOwner(survey.UserId))
            return (false, "You don't have the permission to update this Survey.");

        // Not unique
        if (!await IsUnique(survey))
            return (false, SurveyApiReponses.NotUnique.Message);

        // Update
        survey.UserId = (await _userService.GetCurrentUserId())!;
        _dbContext.Surveys.Update(survey);
        bool updated = await _dbContext.SaveChangesAsync() > 0;
        string? updateError = updated ? null : SurveyApiReponses.UpdateError.Message;
        return (updated, updateError);
    }

    private async Task<bool> IsCurrentUserOwner(string surveyUserId)
    {
        string userId = (await _userService.GetCurrentUserId())!;
        return surveyUserId == userId;
    }
}