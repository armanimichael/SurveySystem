using SurveySystem.Models;

namespace SurveySystem.services.QuestionService;

public class QuestionService : IQuestionService
{
    public Task<IList<Question>> Get()
    {
        throw new NotImplementedException();
    }

    public Task<Question?> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Question?> Create(Question item)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> Update(Question item)
    {
        throw new NotImplementedException();
    }
}