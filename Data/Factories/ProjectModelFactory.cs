using Data.Entities;
using Domain.Extensions;
using Domain.Models;

namespace Data.Factories;

public interface IProjectModelFactory : IModelFactory<ProjectEntity, ProjectModel> { }

public class ProjectModelFactory : IProjectModelFactory
{
    public ProjectModel MapEntityToModel(ProjectEntity entity)
    {
        var model = new ProjectModel
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            Description = entity.Description,
            Image = entity.Image,

            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Created = entity.Created,

            Budget = entity.Budget,
            Client = new ClientModelFactory().MapEntityToModel(entity.Client),
            Owner = new UserModelFactory().MapEntityToModel(entity.Owner),
            Status = entity.Status.MapTo<StatusModel>()
        };
        return model;
    }
}
