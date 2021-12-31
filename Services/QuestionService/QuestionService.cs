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
            .AsNoTracking()
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

    public async Task<ApiResponse> Update(Question question)
    {
        Question? questionInDb = await Get(question.Id);

        // Not existing
        if (questionInDb == null)
            return QuestionApiResponses.NotFound;

        // Force same Survey ID
        question.SurveyId = questionInDb.SurveyId;

        // No permission
        if (!await IsCurrentUserOwner(questionInDb.SurveyId))
            return QuestionApiResponses.NoPermission;

        // Not unique
        if (!await IsUnique(question))
            return QuestionApiResponses.NotUnique;

        // Update
        _dbContext.Questions.Update(question);
        bool updated = await _dbContext.SaveChangesAsync() > 0;
        return updated ? QuestionApiResponses.UpdateSuccess : QuestionApiResponses.UpdateError;
    }

    private async Task<bool> IsUnique(Question question)
    {
        var surveyId = question.SurveyId;
        var title = question.Title;

        return await _dbContext
            .Questions
            .SingleOrDefaultAsync(q => q.SurveyId == surveyId && q.Title == title) == null;
    }

    public async Task<bool> IsCurrentUserOwner(Guid surveyId)
    {
        string userId = (await _surveyService.Get(surveyId))?.UserId ?? string.Empty;
        return await _surveyService.IsCurrentUserOwner(userId);
    }

    public Question DtoToModel(QuestionDto questionDto) =>
        new()
        {
            Title = questionDto.Title,
            Description = questionDto.Description,
            SurveyId = questionDto.SurveyId,
            IsMultipleChoices = questionDto.IsMultipleChoices
        };
}