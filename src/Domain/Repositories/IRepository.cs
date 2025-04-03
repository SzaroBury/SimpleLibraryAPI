namespace SimpleLibrary.Domain.Repositories;

public interface IRepository<T> where T : class
{
    public Task<IEnumerable<T>> GetAllAsync();
    public IQueryable<T> GetQueryable();
    public Task<List<T>> ToListAsync(IQueryable<T> query);
    public Task<T?> GetByIdAsync(Guid id);
    public Task AddAsync(T entity);
    public void Update(T entity);
    public Task DeleteAsync(Guid id);
}