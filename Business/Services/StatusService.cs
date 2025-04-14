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

public class StatusService(IStatusRepository statusRepo) : IStatusService
{
    private readonly IStatusRepository _statusRepo = statusRepo;

    public async Task<ServiceResult<IEnumerable<StatusModel>>> GetAllStatusAsync()
    {
        var result = await _statusRepo.GetAllAsync();
        return result.MapTo<ServiceResult<IEnumerable<StatusModel>>>();
    }

    public async Task<ServiceResult<StatusModel>> GetStatusByNameAsync(string statusName)
    {
        var result = await _statusRepo.GetAsync(s => s.StatusName == statusName);
        return result.MapTo<ServiceResult<StatusModel>>();
    }

    public async Task<ServiceResult<StatusModel>> GetStatusByIdAsync(int statusId)
    {
        var result = await _statusRepo.GetAsync(s => s.Id == statusId);
        return result.MapTo<ServiceResult<StatusModel>>();
    }
}
