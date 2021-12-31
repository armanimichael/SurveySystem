using SurveySystem.Dtos;
using SurveySystem.Models;

namespace SurveySystem.services.QuestionService;

public interface IQuestionService 
{
    public Task<Question?> Get(Guid id);
    public Task<Question?> Create(Question question);
    public Task<ApiResponse> Update(Question question);
    public Task<bool> IsCurrentUserOwner(Guid surveyId);
    public Question DtoToModel(QuestionDto questionDto);
}