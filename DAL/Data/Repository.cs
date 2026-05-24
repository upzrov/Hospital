using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class Repository<T>(HospitalContext context) : IRepository<T> where T : class
{
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }
    
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public virtual async Task CreateAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
    }
    
    public virtual async Task DeleteAsync(T entity)
    {
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public virtual IQueryable<T> Query()
    {
        return context.Set<T>().AsQueryable();
    }
}