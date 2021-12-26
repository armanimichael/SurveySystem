using SurveySystem.Data;
using SurveySystem.Models;

namespace SurveySystem.services.QuestionService;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _dbContext;

    public QuestionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Question?> Get(Guid id)
    {
        throw new NotImplementedException();
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