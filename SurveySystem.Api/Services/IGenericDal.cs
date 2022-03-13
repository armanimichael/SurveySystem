using SurveySystem.Data.Models;

namespace SurveySystem.Services;

public interface IGenericDal<T>
{
    public Task<IList<T>> Get();
    public Task<T?> Get(Guid id);
    public Task<T?> Create(T item);
    public Task<ApiResponse> Update(T item);
    public Task<ApiResponse> Delete(Guid id);
}