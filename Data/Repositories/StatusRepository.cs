using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Data.Repositories;

public interface IStatusRepository : IBaseRepository<StatusModel, StatusEntity> { }

public class StatusRepository(DataContext context, IMemoryCache cache) : BaseRepository<StatusModel, StatusEntity>(context, cache), IStatusRepository { }
