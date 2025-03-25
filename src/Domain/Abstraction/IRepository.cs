namespace SimpleLibrary.Domain.Abstraction;

public interface IRepository<T> where T : class
{
    public List<T> GetAll();
    public IQueryable<T> GetQueryable();
    public T Get(int id);
    public void Create(T element);
    public void Update(T author);
    public void Delete(int id);
}