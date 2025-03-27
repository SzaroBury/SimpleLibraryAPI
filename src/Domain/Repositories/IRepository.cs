namespace SimpleLibrary.Domain.Repositories;

public interface IRepository<T> where T : class
{
    public Task<IEnumerable<T>> GetAllAsync();
    public IQueryable<T> GetQueryable();
    public Task<T?> GetByIdAsync(Guid id);
    public Task AddAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(Guid id);
}