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

public class ProjectService(IProjectRepository projectRepo, IStatusService statusService, IFileHandler fileHandler) : IProjectService
{
    private readonly IProjectRepository _projectRepo = projectRepo;
    private readonly IStatusService _statusService = statusService;
    private readonly IFileHandler _fileHandler = fileHandler;

    public async Task<ServiceResult<IEnumerable<ProjectModel>>> GetAllProjectsAsync()
    {
        var result = await _projectRepo.GetAllAsync(orderByDesc: true, sortBy: p => p.Created, filterBy: null, i => i.Owner, i => i.Status, i => i.Client);
        return result.MapTo<ServiceResult<IEnumerable<ProjectModel>>>();
    }

    public async Task<ServiceResult<IEnumerable<ProjectModel>>> GetAllProjectsWithStatusAsync(string statusName="Completed")
    {
        var result = await _projectRepo.GetAllAsync(orderByDesc: true, sortBy: p => p.Created, filterBy: p => p.Status.StatusName.ToLower() == statusName.ToLower(), i => i.Owner, i => i.Status, i => i.Client);
        return result.MapTo<ServiceResult<IEnumerable<ProjectModel>>>();
    }

    public async Task<ServiceResult<ProjectModel>> GetProjectByIdAsync(string projectId)
    {
        var result = await _projectRepo.GetAsync(findBy: p => p.Id == projectId);
        return result.MapTo<ServiceResult<ProjectModel>>();
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
        return result.MapTo<ServiceResult>();
    }

    public async Task<ServiceResult> UpdateProjectAsync(EditProjectFormDto form)
    {
        if (form == null)
            return new ServiceResult { Succeeded = false, StatusCode = 400 };

        var entity = form.MapTo<ProjectEntity>();

        if (form.NewImage != null)
            entity.Image = await _fileHandler.UploadFileAsync(form.NewImage);
        else if (form.Image != null)
            entity.Image = form.Image;

        var result = await _projectRepo.UpdateAsync(entity);
        return result.MapTo<ServiceResult>();
    }

    public async Task<ServiceResult> DeleteProjectAsync(string projectId)
    {
        var result = await _projectRepo.DeleteAsync(p => p.Id == projectId);
        return result.MapTo<ServiceResult>();
    }
}
