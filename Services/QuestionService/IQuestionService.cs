using SurveySystem.Models;

namespace SurveySystem.services.QuestionService;

public interface IQuestionService 
{
    public Task<Question?> Get(Guid id);
    public Task<Question?> Create(Question question);
    public Task<ApiResponse> Update(Question question);   
}