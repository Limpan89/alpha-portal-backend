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
    protected readonly IMemoryCache _cache;
    protected readonly List<string> _cachedKeys;
    protected readonly IModelFactory<TEntity, TModel>? _modelFactory = null;

    protected BaseRepository(DataContext context, IMemoryCache cache)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
        _cache = cache;
        _cachedKeys = new List<string>();
    }

    protected BaseRepository(DataContext context, IMemoryCache cache, IModelFactory<TEntity, TModel> modelFactory)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
        _cache = cache;
        _cachedKeys = new List<string>();
        _modelFactory = modelFactory;
    }

    public virtual async Task<RepositoryResult> ExistsAsync(Expression<Func<TEntity, bool>> findBy)
    {
        var result = await _dbSet.AnyAsync(findBy);
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
            ClearCache();
            return new RepositoryResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
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
            ClearCache();
            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
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
            ClearCache();
            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    public virtual async Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDesc = false, Expression<Func<TEntity, object>>? sortBy = null,
        Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] includes)
    {
        string cacheKey = GenerateCacheKey(orderByDesc, sortBy, filterBy, includes);
        IEnumerable<TModel>? models;
        if (_cache.TryGetValue(cacheKey, out models))
            return new RepositoryResult<IEnumerable<TModel>> { Succeeded = true, StatusCode = 200, Result = models };

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
        models = _modelFactory != null
            ? entities.Select(e => _modelFactory.MapEntityToModel(e))
            : entities.Select(e => e.MapTo<TModel>());
        AddToCache<IEnumerable<TModel>>(cacheKey, models);

        return new RepositoryResult<IEnumerable<TModel>> { Succeeded = true, StatusCode = 200, Result = models };
    }

    public virtual async Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] includes)
    {
        TModel? model;
        string cacheKey = GenerateCacheKey(findBy, includes);
        if (_cache.TryGetValue<TModel>(cacheKey, out model))
            return new RepositoryResult<TModel> { Succeeded = true, StatusCode = 200, Result = model };

        IQueryable<TEntity> query = _dbSet;

        if (includes != null && includes.Length > 0)
        {
            foreach (var i in includes)
                query = query.Include(i);
        }

        var entity = await query.FirstOrDefaultAsync(findBy);
        if (entity == null)
            return new RepositoryResult<TModel> { Succeeded = false, StatusCode = 404 };

        model = _modelFactory != null
            ? _modelFactory.MapEntityToModel(entity)
            : entity.MapTo<TModel>();
        AddToCache<TModel>(cacheKey, model);

        return new RepositoryResult<TModel> { Succeeded = true, StatusCode = 200, Result = model! };
    }

    #region "Cache"
    public string GenerateCacheKey(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] includes)
    {
        string includeFragment = (includes != null && includes.Length != 0 ? string.Join("_", includes.Select(i => i.ToString())) : "");
        return $"{typeof(TEntity).Name}_{findBy.ToString()}_{includeFragment}";
    }

    public string GenerateCacheKey(bool orderByDesc, Expression<Func<TEntity, object>>? sortBy, Expression<Func<TEntity, bool>>? filterBy, params Expression<Func<TEntity, object>>[] includes)
    {
        string orderFragment = orderByDesc ? "_descending" : "_ascending";
        string sortFragment = sortBy != null ? $"_{sortBy.ToString()}" : "";
        string filterFragment = filterBy != null ? $"_{filterBy.ToString()}" : "";
        string includeFragment = includes != null && includes.Length != 0 ? string.Join("_", includes.Select(i => i.ToString())) : "";
        return $"{typeof(TEntity).Name}_All{orderFragment}{sortFragment}{filterFragment}_{includeFragment}";
    }

    public void ClearCache()
    {
        foreach (var key in _cachedKeys)
            _cache.Remove(key);
        _cachedKeys.Clear();
    }

    public void AddToCache<T>(string key, T value)
    {
        _cache.CreateEntry(key);
        _cache.Set<T>(key, value);
        _cachedKeys.Add(key);
    }
    #endregion
}
