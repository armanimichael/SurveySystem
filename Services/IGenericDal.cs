namespace SurveySystem.services;

public interface IGenericDal<T>
{
    public Task<IList<T>> Get();
    public Task<T?> Get(Guid id);
    public Task<T?> Create(T item);
}