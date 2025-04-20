using Business.Handlers;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IProjectService
{
    Task<ServiceResult> AddProjectAsync(AddProjectFormDto form);
    Task<ServiceResult> DeleteProjectAsync(string projectId);
    Task<ServiceResult<IEnumerable<ProjectModel>>> GetAllProjectsAsync();
    Task<ServiceResult<ProjectModel>> GetProjectByIdAsync(string projectId);
    Task<ServiceResult> UpdateProjectAsync(EditProjectFormDto form);
}

public class ProjectService(IProjectRepository projectRepo, IStatusService statusService, IFileHandler fileHandler, ICacheHandler<IEnumerable<ProjectModel>> projectCache) : IProjectService
{
    private readonly IProjectRepository _projectRepo = projectRepo;
    private readonly IStatusService _statusService = statusService;
    private readonly IFileHandler _fileHandler = fileHandler;

    private readonly ICacheHandler<IEnumerable<ProjectModel>> _projectCache = projectCache;
    private const string _CACHE_KEY = "CACHE_KEY_ALL_PROJECTS";

    public async Task<ServiceResult<IEnumerable<ProjectModel>>> GetAllProjectsAsync()
    {
        var cached = _projectCache.Get(_CACHE_KEY);
        if (cached != null)
            return new ServiceResult<IEnumerable<ProjectModel>> { Succeeded = true, StatusCode = 200, Result = cached };

        var models = await UpdateCache();
        if (models == null)
            return new ServiceResult<IEnumerable<ProjectModel>> { Succeeded = false, StatusCode = 400 };

        return new ServiceResult<IEnumerable<ProjectModel>> { Succeeded = true, StatusCode = 200, Result = models };
    }

    public async Task<ServiceResult<IEnumerable<ProjectModel>>> GetAllProjectsWithStatusAsync(string statusName="Completed")
    {
        var result = await _projectRepo.GetAllAsync(orderByDesc: true, sortBy: p => p.Created, filterBy: p => p.Status.StatusName.ToLower() == statusName.ToLower(), i => i.Owner, i => i.Owner.Profile, i => i.Owner.Address, i => i.Owner.Address.PostalAddress, i => i.Status, i => i.Client, i => i.Client.Billing, i => i.Client.Billing.PostalAddress);
        return result.MapTo<ServiceResult<IEnumerable<ProjectModel>>>();
    }

    public async Task<ServiceResult<ProjectModel>> GetProjectByIdAsync(string projectId)
    {
        ProjectModel? match = null;
        var cached = _projectCache.Get(_CACHE_KEY);

        if (cached != null)
            match = cached.FirstOrDefault(p => p.Id == projectId);

        if (match == null)
        {
            var models = await UpdateCache();

            if (models == null)
                return new ServiceResult<ProjectModel>
                {
                    Succeeded = false,
                    StatusCode = 400
                };

            match = models.FirstOrDefault(p => p.Id == projectId);
        }
        if (match == null)
            return new ServiceResult<ProjectModel> { Succeeded = false, StatusCode = 404 };

        return new ServiceResult<ProjectModel> { Succeeded = true, StatusCode = 200, Result = match };
    }

    public async Task<ServiceResult> AddProjectAsync(AddProjectFormDto form)
    {
        if (form == null)
            return new ServiceResult { Succeeded = false, StatusCode = 400 };

        var entity = form.MapTo<ProjectEntity>();
        if (form.NewImage != null)
            entity.Image = await _fileHandler.UploadFileAsync(form.NewImage);

        var statusResult = await _statusService.GetStatusByNameAsync("Started");
        if (statusResult.Succeeded)
            entity.StatusId = statusResult.Result!.Id;

        var result = await _projectRepo.AddAsync(entity);

        await UpdateCache();
        return result.MapTo<ServiceResult>();
    }

    public async Task<ServiceResult> UpdateProjectAsync(EditProjectFormDto form)
    {
        if (form == null)
            return new ServiceResult { Succeeded = false, StatusCode = 400 };


        var entity = form.MapTo<ProjectEntity>();

        var oldEntity = await GetProjectByIdAsync(form.Id);
        entity.Created = oldEntity.Result.Created; // Quick dirty fix

        if (form.NewImage != null)
            entity.Image = await _fileHandler.UploadFileAsync(form.NewImage);
        else if (form.Image != null)
            entity.Image = form.Image;

        var result = await _projectRepo.UpdateAsync(entity);

        await UpdateCache();
        return result.MapTo<ServiceResult>();
    }

    public async Task<ServiceResult> DeleteProjectAsync(string projectId)
    {
        var result = await _projectRepo.DeleteAsync(p => p.Id == projectId);

        await UpdateCache();
        return result.MapTo<ServiceResult>();
    }

    public async Task<IEnumerable<ProjectModel>>? UpdateCache()
    {
        var result = await _projectRepo.GetAllAsync(orderByDesc: true, sortBy: p => p.Created, filterBy: null, i => i.Owner, i => i.Status, i => i.Client);

        if (!result.Succeeded)
            return null;

        _projectCache.Set(_CACHE_KEY, result.Result);
        return result.Result;
    }
}
