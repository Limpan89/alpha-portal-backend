using Data.Contexts;
using Data.Entities;
using Data.Factories;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Data.Repositories;

public interface IProjectRepository : IBaseRepository<ProjectEntity, ProjectModel> { }

public class ProjectRepository(DataContext context, IMemoryCache cache) : BaseRepository<ProjectEntity, ProjectModel>(context, cache, new ProjectModelFactory()), IProjectRepository { }
