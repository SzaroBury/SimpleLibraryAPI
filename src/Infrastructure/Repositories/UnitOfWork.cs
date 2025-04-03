using SimpleLibrary.Domain.Repositories;

namespace SimpleLibrary.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LibraryEFContext context;
    private readonly Dictionary<Type, object> repositories = new();

    public UnitOfWork(LibraryEFContext context)
    {
        this.context = context;
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        if (!repositories.ContainsKey(typeof(T)))
        {
            repositories[typeof(T)] = new Repository<T>(context);
        }
        return (IRepository<T>) repositories[typeof(T)];
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose() => context.Dispose();
}