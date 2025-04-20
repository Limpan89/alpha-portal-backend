using Data.Contexts;
using Data.Factories;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public interface IBaseRepository<TEntity, TModel> where TEntity : class
{
    public Task<RepositoryResult> AddAsync(TEntity entity);
    public Task<RepositoryResult> DeleteAsync(Expression<Func<TEntity, bool>> expression);
    public Task<RepositoryResult> ExistsAsync(Expression<Func<TEntity, bool>> expression);
    public Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDesc = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] includes);
    public Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] includes);
    public Task<RepositoryResult> UpdateAsync(TEntity entity);
}

public abstract class BaseRepository<TEntity, TModel> : IBaseRepository<TEntity, TModel> where TEntity : class
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly IModelFactory<TEntity, TModel>? _modelFactory = null;

    protected BaseRepository(DataContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    protected BaseRepository(DataContext context, IModelFactory<TEntity, TModel> modelFactory)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
        _modelFactory = modelFactory;
    }

    public virtual async Task<RepositoryResult> ExistsAsync(Expression<Func<TEntity, bool>> findBy)
    {
        bool result = await _dbSet.AnyAsync(findBy);
        return result
            ? new RepositoryResult { Succeeded = true, StatusCode = 200 }
            : new RepositoryResult { Succeeded = false, StatusCode = 404 };

    }

    public virtual async Task<RepositoryResult> AddAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    public virtual async Task<RepositoryResult> UpdateAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    public virtual async Task<RepositoryResult> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(expression);
        if (entity == default)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    public virtual async Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDesc = false, Expression<Func<TEntity, object>>? sortBy = null,
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
            query = orderByDesc ? query.OrderByDescending(sortBy) : query.OrderBy(sortBy);

        var entities = await query.ToListAsync();
        var models = _modelFactory != null
            ? entities.Select(e => _modelFactory.MapEntityToModel(e))
            : entities.Select(e => e.MapTo<TModel>());

        return new RepositoryResult<IEnumerable<TModel>> { Succeeded = true, StatusCode = 200, Result = models };
    }

    public virtual async Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        if (includes != null && includes.Length > 0)
        {
            foreach (var i in includes)
                query = query.Include(i);
        }

        var entity = await query.FirstOrDefaultAsync(findBy);
        if (entity == null)
            return new RepositoryResult<TModel> { Succeeded = false, StatusCode = 404 };

        var model = _modelFactory != null
            ? _modelFactory.MapEntityToModel(entity)
            : entity.MapTo<TModel>();

        return new RepositoryResult<TModel> { Succeeded = true, StatusCode = 200, Result = model! };
    }
}
