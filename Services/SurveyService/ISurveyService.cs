using SurveySystem.Models;

namespace SurveySystem.services.SurveyService;

public interface ISurveyService
{
    public Task<IList<Survey>> Get();
    public Task<Survey?> Get(Guid id);
    public Task<Survey?> Create(Survey item);
    public Task<ApiResponse> Update(Survey item);
    public Task<bool> IsCurrentUserOwner(string surveyUserId);
}