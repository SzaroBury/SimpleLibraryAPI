using Microsoft.EntityFrameworkCore;
using SimpleLibrary.Domain.Repositories;

namespace SimpleLibrary.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext context;
    private readonly DbSet<T> dbSet;

    Repository(DbContext context)
    {
        this.context = context;
        dbSet = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await dbSet.AsNoTracking().ToListAsync();
    }

    public IQueryable<T> GetQueryable()
    {
        return dbSet.AsQueryable();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await dbSet.FindAsync(id);
    }
    public async Task AddAsync(T entity)
    {
        await dbSet.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        dbSet.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}