using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<RepositoryResult> AddAsync(TEntity entity);
    Task<RepositoryResult> DeleteAsync(Expression<Func<TEntity, bool>> expression);
    Task<RepositoryResult> ExistsAsync(Expression<Func<TEntity, bool>> expression);
    Task<RepositoryResult<IEnumerable<TEntity>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult> UpdateAsync(TEntity entity);
}

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected BaseRepository(DataContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public virtual async Task<RepositoryResult> ExistsAsync(Expression<Func<TEntity, bool>> expression)
    {
        return new RepositoryResult { Succeded = await _dbSet.AnyAsync(expression) };
    }

    public virtual async Task<RepositoryResult> AddAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult { Succeded = false };

        try
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult { Succeded = true };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult { Succeded = false };
        }
    }

    public virtual async Task<RepositoryResult> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(expression);
        if (entity == default)
            return new RepositoryResult { Succeded = false };

        try
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult { Succeded = await _dbSet.AnyAsync(expression) };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult { Succeded = false };
        }
    }

    public virtual async Task<RepositoryResult<IEnumerable<TEntity>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortBy = null,
        Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filterBy != null)
            query = query.Where(filterBy);

        if (includes != null && includes.Length > 0)
        {
            foreach (var i in includes)
                query = query.Include(i);
        }

        if (sortBy != null)
            query = orderByDescending ? query.OrderByDescending(sortBy) : query.OrderBy(sortBy);

        return new RepositoryResult<IEnumerable<TEntity>> { Succeded = true, Result = await query.ToListAsync() };
    }

    public virtual async Task<RepositoryResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        if (includes != null && includes.Length > 0)
        {
            foreach (var i in includes)
                query = query.Include(i);
        }

        var entity = await query.FirstOrDefaultAsync(findBy);
        return new RepositoryResult<TEntity> { Succeded = entity != null, Result = entity ?? null };
    }

    public virtual async Task<RepositoryResult> UpdateAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult { Succeded = false };

        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult { Succeded = true };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult { Succeded = false };
        }
    }
}
