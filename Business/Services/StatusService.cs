using Business.Handlers;
using Business.Models;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IStatusService
{
    Task<ServiceResult<IEnumerable<StatusModel>>> GetAllStatusAsync();
    Task<ServiceResult<StatusModel>> GetStatusByIdAsync(int statusId);
    Task<ServiceResult<StatusModel>> GetStatusByNameAsync(string statusName);
}

public class StatusService(IStatusRepository statusRepo, ICacheHandler<IEnumerable<StatusModel>> statusCache) : IStatusService
{
    private readonly IStatusRepository _statusRepo = statusRepo;
    private readonly ICacheHandler<IEnumerable<StatusModel>> _statusCache = statusCache;
    private const string _CACHE_KEY = "CACHE_KEY_ALL_STATUS";

    public async Task<ServiceResult<IEnumerable<StatusModel>>> GetAllStatusAsync()
    {
        var cached = _statusCache.Get(_CACHE_KEY);
        if (cached != null)
            return new ServiceResult<IEnumerable<StatusModel>> { Succeeded = true, StatusCode = 200, Result = cached };

        var models = await UpdateCache();
        if (models == null)
            return new ServiceResult<IEnumerable<StatusModel>> { Succeeded = false, StatusCode = 400 };

        return new ServiceResult<IEnumerable<StatusModel>> { Succeeded = true, StatusCode = 200, Result = models };
    }

    public async Task<ServiceResult<StatusModel>> GetStatusByNameAsync(string statusName)
    {
        StatusModel? match = null;
        var cached = _statusCache.Get(_CACHE_KEY);

        if (cached != null)
            match = cached.FirstOrDefault(s => s.StatusName == statusName);

        if (match == null)
        {
            var models = await UpdateCache();

            if (models == null)
                return new ServiceResult<StatusModel>
                {
                    Succeeded = false,
                    StatusCode = 400
                };

            match = models.FirstOrDefault(s => s.StatusName == statusName);
        }
        if (match == null)
            return new ServiceResult<StatusModel> { Succeeded = false, StatusCode = 404 };

        return new ServiceResult<StatusModel> { Succeeded = true, StatusCode = 200, Result = match };
    }

    public async Task<ServiceResult<StatusModel>> GetStatusByIdAsync(int statusId)
    {
        StatusModel? match = null;
        var cached = _statusCache.Get(_CACHE_KEY);

        if (cached != null)
            match = cached.FirstOrDefault(s => s.Id == statusId);

        if (match == null)
        {
            var models = await UpdateCache();

            if (models == null)
                return new ServiceResult<StatusModel>
                {
                    Succeeded = false,
                    StatusCode = 400
                };

            match = models.FirstOrDefault(s => s.Id == statusId);
        }
        if (match == null)
            return new ServiceResult<StatusModel> { Succeeded = false, StatusCode = 404 };

        return new ServiceResult<StatusModel> { Succeeded = true, StatusCode = 200, Result = match };
    }

    public async Task<IEnumerable<StatusModel>>? UpdateCache()
    {
        var result = await _statusRepo.GetAllAsync();

        if (!result.Succeeded)
            return null;

        _statusCache.Set(_CACHE_KEY, result.Result);
        return result.Result;
    }
}
