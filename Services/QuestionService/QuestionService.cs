using Microsoft.EntityFrameworkCore;
using SurveySystem.ApiResponses;
using SurveySystem.Data;
using SurveySystem.Dtos;
using SurveySystem.Models;
using SurveySystem.services.SurveyService;
using SurveySystem.services.UserService;

namespace SurveySystem.services.QuestionService;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly ISurveyService _surveyService;

    public QuestionService(ApplicationDbContext dbContext, IUserService userService, ISurveyService surveyService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _surveyService = surveyService;
    }

    public async Task<Question?> Get(Guid id)
    {
        string userId = await _userService.GetCurrentUserId() ?? "";

        return await _dbContext.Questions
            .Where(q => userId == q.Survey.UserId || (bool)q.Survey.IsVisible!)
            .SingleOrDefaultAsync(q => q.Id.Equals(id));
    }

    public async Task<Question?> Create(Question question)
    {
        if (!await IsUnique(question))
            return null;

        await _dbContext.Questions.AddAsync(question);
        await _dbContext.SaveChangesAsync();
        return question;
    }

    public Task<ApiResponse> Update(Question question)
    {
        throw new NotImplementedException();
    }

    private async Task<bool> IsUnique(Question question)
    {
        var surveyId = question.SurveyId;
        var title = question.Title; 
        
        return await _dbContext
            .Questions
            .SingleOrDefaultAsync(q => q.SurveyId == surveyId && q.Title == title) == null;
    }
}