using Microsoft.EntityFrameworkCore;
using SurveySystem.Data;
using SurveySystem.Models;
using SurveySystem.services.UserService;

namespace SurveySystem.services.QuestionService;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserService _userService;

    public QuestionService(ApplicationDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<Question?> Get(Guid id)
    {
        string userId = await _userService.GetCurrentUserId() ?? "";

        return await _dbContext.Questions
            .Where(q => userId == q.Survey.UserId || (bool)q.Survey.IsVisible!)
            .SingleOrDefaultAsync(q => q.Id.Equals(id));
    }

    public Task<Question?> Create(Question question)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> Update(Question question)
    {
        throw new NotImplementedException();
    }
}