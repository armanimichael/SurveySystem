namespace SurveySystem.services;

public interface IGenericDal<T>
{
    public Task<IList<T>> Get();
    public Task<T?> Get(Guid id);
    public Task<T?> Create(T item);
    public Task<(bool updated, string? errorMessage)> Update(T item);
}